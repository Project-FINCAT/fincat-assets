using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Text;
using System.IO;

// --- 데이터 직렬화 클래스들 ---
[Serializable] public class NewsPost_Data { public string news_content; }
[Serializable] public class Gemini_Analysis_Part { public string agent_name; public string summary; public string analysis; public string prediction; }
[Serializable] public class Server_Response_All { public Gemini_Analysis_Part growth; public Gemini_Analysis_Part value; public Gemini_Analysis_Part technical; }


public class NewsAnalyzer : MonoBehaviour
{
    public static NewsAnalyzer Instance { get; private set; }

    [Header("설정")]
    [SerializeField] private string serverUrl = "http://127.0.0.1:8000/api/analyze-news";
    private string activeNPCType; 

    [Header("연결 요소")]
    public NewsDatabase newsDatabase;
    //public GameObject parentDialoguePanel;
    //public GameObject aiDialoguePanel;
    public TMP_Text contentText; 

    // 캐시 데이터 변수 추가
    private Server_Response_All cachedResponse; // 서버 답변 저장
    private int cachedNewsID = -1; // 어떤 뉴스 ID 였는지 저장
    private bool isDataCached = false; // 데이터가 있는지 확인


    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }
    
    public void ResetCache()
    {
        cachedResponse = null;
        cachedNewsID = -1;
        isDataCached = false;
        Debug.Log("AI 분석 캐시가 초기화되었습니다.");
    }

    // NPC가 호출하는 함수. 이제 id는 NewsManager를 통해 내부에서 가져옵니다.
    public void StartAIAnalysis(string npcType)
    {
        activeNPCType = npcType;
        // 패널 활성화
        //if (parentDialoguePanel != null) parentDialoguePanel.SetActive(true);

        // [핵심] NewsManager에서 오늘 확정된 뉴스의 ID를 가져옴
        int todayID = NewsManager.Instance.CurrentNewsID;

        if (isDataCached && cachedNewsID == todayID)
        {
            Debug.Log("<color=green>[캐시 사용]</color> 이미 분석된 뉴스 이므로 저장된 답변 불러오기");
            UpdateUI(cachedResponse);
            return;
        }
        if (contentText != null) contentText.text = "AI 투자 전문가가 분석 중입니다...";

        string newsContent = NewsManager.Instance.GetNewsTextByID(todayID);
        
        if (!string.IsNullOrEmpty(newsContent))
            StartCoroutine(PostNewsAnalysis(newsContent));
        else
            if (contentText != null) contentText.text = "오늘의 뉴스를 가져오지 못했습니다.";
    }

    // NewsAnalyzer.cs 내부에 추가
    public Server_Response_All GetCachedData()
    {
        if (isDataCached) 
        {
            return cachedResponse;
        }
        return null;
    }

    // NewsAnalyzer.cs
    public int GetCachedNewsID() 
    {
        return cachedNewsID; // private 변수인 cachedNewsID를 안전하게 전달
    }

    // NewsManager.cs 내부에 추가
    public string GetCurrentNewsText()
    {
        if (newsDatabase == null || newsDatabase.newsList == null) return "";

        // 현재 관리 중인 ID와 일치하는 뉴스를 찾습니다.
        var news = newsDatabase.newsList.Find(n => n.newsID == cachedNewsID);
        
        return news != null ? news.newsText : "";
    }


    // ID를 매개변수로 추가하여 저장할 때 사용
    IEnumerator PostNewsAnalysis(string newsContent)
    {
        // [단계 1] 전송 전 유효성 검사
        if (string.IsNullOrEmpty(newsContent))
        {
            Debug.LogError("서버로 보낼 뉴스 내용이 비어있습니다.");
            if (contentText != null) contentText.text = "<color=red>뉴스를 읽어오지 못했습니다.</color>";
            yield break;
        }

        // [단계 2] 로딩 피드백 (유저가 기다림을 인지하게 함)
        if (contentText != null) 
            contentText.text = "<color=#AAAAAA>AI 전문가가 분석을 시작했습니다...\n(예상 대기 시간: 3~5초)</color>";

        // [단계 3] JSON 데이터 준비
        NewsPost_Data myRequest = new NewsPost_Data { news_content = newsContent };
        string jsonPayload = JsonUtility.ToJson(myRequest);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);

        // [단계 4] 서버 통신 설정
        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // 전송 시도 로그
            Debug.Log($"[서버 전송] URL: {serverUrl}");

            // 서버 응답 대기
            yield return request.SendWebRequest();

            // [단계 5] 결과 처리
            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("<color=cyan>[서버 응답 성공]</color>");
                
                try 
                {
                    // JSON 파싱 및 UI 업데이트
                    Server_Response_All response = JsonUtility.FromJson<Server_Response_All>(request.downloadHandler.text);
                    // [캐싱] 성공적으로 받은 데이터를 저장
                    cachedResponse = response;
                    cachedNewsID = NewsManager.Instance.CurrentNewsID;
                    isDataCached = true;

                    // JSON 파일 저장
                    SaveAnalysisToJson(response, cachedNewsID);

                    // UI 업데이트
                    UpdateUI(response);
                }
                catch (Exception e)
                {
                    Debug.LogError($"JSON 파싱 에러: {e.Message}");
                    if (contentText != null) contentText.text = "<color=red>데이터 해석 중 오류가 발생했습니다.</color>";
                }
            }
            else
            {
                // [단계 6] 에러 케이스별 대응
                long responseCode = request.responseCode;
                Debug.LogError($"[서버 전송 실패] 에러: {request.error} / 코드: {responseCode}");

                if (responseCode == 429) // 구글 API 할당량 초과 에러
                {
                    if (contentText != null) 
                        contentText.text = "<b><color=red>전문가들이 현재 과부하 상태입니다.</color></b>\n\n약 30초 후에 다시 대화를 시도해 주세요.";
                }
                else if (responseCode == 0) // 서버가 꺼져 있거나 주소가 틀림
                {
                    if (contentText != null) 
                        contentText.text = "<color=red>분석 서버에 연결할 수 없습니다.\n(파이썬 서버 실행 여부를 확인하세요)</color>";
                }
                else
                {
                    if (contentText != null) 
                        contentText.text = $"<color=red>통신 오류가 발생했습니다.\n(Error: {responseCode})</color>";
                }
            }
        }
    }
    private void UpdateUI(Server_Response_All data)
    {
        if (contentText == null || data == null) return;

        string finalDisplay = "";
        switch (activeNPCType)
        {
            case "Aggressive": finalDisplay = FormatMessage(data.growth, "#FF4500"); break;
            case "Neutral": finalDisplay = FormatMessage(data.value, "#22C55E"); break;
            case "Stable": finalDisplay = FormatMessage(data.technical, "#1E90FF"); break;
            default: finalDisplay = FormatMessage(data.growth, "#000000"); break;
        }
        contentText.text = finalDisplay;
    }

    private string FormatMessage(Gemini_Analysis_Part agent, string colorCode)
    {
        return $"<b><color={colorCode}>[{agent.agent_name}]</color></b>\n\n{agent.analysis}\n\n<b>";
    }

    private void SaveAnalysisToJson(Server_Response_All data, int newsID)
    {
        try
        {
            // 저장 폴더 생성
            string folderPath = Path.Combine(Application.persistentDataPath, "AnalysisCache");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // 파일 이름 생성
            string filePath = Path.Combine(folderPath, $"news_{newsID}.json");

            // JSON 변환
            string json = JsonUtility.ToJson(data, true);

            // 파일 저장
            File.WriteAllText(filePath, json, Encoding.UTF8);

            Debug.Log($"<color=green>JSON 저장 완료</color>\n{filePath}");
        }
        catch (Exception e)
        {
            Debug.LogError($"JSON 저장 실패: {e.Message}");
        }
    }

}
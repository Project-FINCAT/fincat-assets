using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System.Text;
using UnityEngine.InputSystem;

[Serializable]
public class DebateRequest
{
    public string news_content;
    public string growth_opinion;
    public string value_opinion;
    public string technical_opinion;
    public string player_opinion;
}

[Serializable]
public class DebateResponse
{
    public string stable_argument;
    public string aggressive_argument;
    public string neutral_argument;
    public string final_summary;
}

public class DebateManager : MonoBehaviour
{
    public static DebateManager Instance;

    public string debateServerUrl = "http://127.0.0.1:8000/api/debate";

    [Header("UI 연결")]
    public TMP_InputField playerInput;

    public TMP_Text stableText;
    public TMP_Text aggressiveText;
    public TMP_Text neutralText;
    public TMP_Text resultText;

    private bool isProcessing = false;

    private void Awake()
    {
        Instance = this;

        if (playerInput != null)
        {
            playerInput.onEndEdit.AddListener(OnInputSubmit);
        }
    }

    // 엔터 입력 시 실행
    private void OnInputSubmit(string text)
    {
        if (isProcessing || string.IsNullOrWhiteSpace(text))
            return;

        StartDebateProcess();
    }

    public void StartDebateProcess()
    {
        if (isProcessing)
            return;

        var cache = NewsAnalyzer.Instance.GetCachedData();
        int targetID = NewsAnalyzer.Instance.GetCachedNewsID();

        if (cache == null || targetID == -1)
        {
            Debug.LogError("분석된 데이터가 없습니다. 에이전트와 먼저 대화하세요!");
            return;
        }

        DebateRequest data = new DebateRequest
        {
            news_content = NewsManager.Instance.GetNewsTextByID(targetID),
            growth_opinion = cache.growth.analysis,
            value_opinion = cache.value.analysis,
            technical_opinion = cache.technical.analysis,
            player_opinion = playerInput.text
        };

        isProcessing = true;

        StartCoroutine(PostDebate(data));
    }

    IEnumerator PostDebate(DebateRequest data)
    {
        string json = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(debateServerUrl, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                DebateResponse response =
                    JsonUtility.FromJson<DebateResponse>(request.downloadHandler.text);

                StartCoroutine(ShowDebateSequence(response));

                playerInput.text = "";
            }
            else
            {
                Debug.LogError("서버 통신 실패: " + request.error);

                isProcessing = false;
            }
        }
    }

    IEnumerator ShowDebateSequence(DebateResponse response)
    {
        stableText.text = "";
        neutralText.text = "";
        aggressiveText.text = "";
        resultText.text = "";

        yield return new WaitForSeconds(1f);

        // 안정형
        yield return StartCoroutine(TypeText(
            stableText,
            "<color=#1E90FF><b>[안정형]</b></color>",
            response.stable_argument
        ));

        yield return new WaitForSeconds(1f);

        // 중립형
        yield return StartCoroutine(TypeText(
            neutralText,
            "<color=#FFD700><b>[중립형]</b></color>",
            response.neutral_argument
        ));

        yield return new WaitForSeconds(1f);

        // 공격형
        yield return StartCoroutine(TypeText(
            aggressiveText,
            "<color=#22C55E><b>[공격형]</b></color>",
            response.aggressive_argument
        ));

        yield return new WaitForSeconds(1f);

        // 판사
        resultText.text =
            "<color=#000000><b>[판사]</b></color>\n" +
            response.final_summary;

        isProcessing = false;
    }

    IEnumerator TypeText(
        TMP_Text target,
        string title,
        string message,
        float delay = 0.02f
    )
    {
        // 제목은 즉시 출력
        target.text = title + "\n";

        // 내용만 타이핑
        foreach (char c in message)
        {
            target.text += c;

            yield return new WaitForSeconds(delay);
        }
    }
}
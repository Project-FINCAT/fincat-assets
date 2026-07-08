using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;

    [Serializable]
    public class RequestData
    {
        public string npc_id;
        public string message;
    }

    [Serializable]
    public class ResponseData
    {
        public string response;
    }

    [Header("Server Settings")]
    [SerializeField] private string serverUrl = "http://localhost:8000/chat";
    [SerializeField] private int timeoutSeconds = 10;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// AI 서버 요청
    /// </summary>
    public IEnumerator GetAIResponse(string npcId, string playerInput, Action<string> callback)
    {
        if (string.IsNullOrEmpty(playerInput))
        {
            callback?.Invoke("입력이 비어있습니다.");
            yield break;
        }

        RequestData requestData = new RequestData
        {
            npc_id = npcId,
            message = playerInput
        };

        string json = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest request = new UnityWebRequest(serverUrl, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            request.timeout = timeoutSeconds;

            //Debug.Log($"[AI 요청] NPC: {npcId}, 메시지: {playerInput}");

            yield return request.SendWebRequest();

            // 실패 처리
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError($"[AI 오류] 코드: {request.responseCode}");
                Debug.LogError($"[AI 오류] 메시지: {request.error}");

                if (!string.IsNullOrEmpty(request.downloadHandler.text))
                {
                    Debug.LogError($"[서버 응답] {request.downloadHandler.text}");
                }

                callback?.Invoke("서버 연결 실패");
                yield break;
            }

            // 성공 처리
            try
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log("[AI 응답] " + jsonResponse);

                ResponseData data = JsonUtility.FromJson<ResponseData>(jsonResponse);

                if (data == null || string.IsNullOrEmpty(data.response))
                {
                    callback?.Invoke("응답이 비어있습니다.");
                }
                else
                {
                    callback?.Invoke(data.response);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[JSON 파싱 오류] " + e.Message);
                callback?.Invoke("응답 처리 실패");
            }
        }
    }
}
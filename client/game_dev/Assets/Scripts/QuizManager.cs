using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizManager : MonoBehaviour
{
    public static QuizManager Instance { get; private set; }

    [Header("Quiz UI Panels")]
    public GameObject quizPanel; // 전체 퀴즈 부모 패널

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        
        // 시작할 때는 퀴즈 창을 꺼둡니다.
        if (quizPanel != null) quizPanel.SetActive(false);
    }

    // QuestPanelUI의 버튼을 눌렀을 때 호출될 함수
    public void StartQuiz(QuestData quest)
    {
        Debug.Log($"{quest.questName} 퀴즈를 시작합니다.");
        quizPanel.SetActive(true);
        
        // 여기서 퀘스트 데이터를 기반으로 문제를 세팅하는 로직을 추가합니다.
        SetupQuiz(quest);
    }

    private void SetupQuiz(QuestData quest)
    {
        // 퀴즈 텍스트 세팅 등의 로직...
    }
}
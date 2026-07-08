using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuestionManager : MonoBehaviour
{
    // 어디서든 접근 가능하게 싱글톤 설정
    public static QuestionManager Instance { get; private set; }

    [Header("UI 연결")]
    public GameObject questionPanel;      // 전체 질문창 부모 오브젝트
    public TMP_Text questionText;         // 질문 내용이 뜰 텍스트
    public Transform choiceParent;        // 선택지 버튼들이 생성될 부모(Content)
    public GameObject choiceButtonPrefab; // 선택지 버튼 프리팹

    [Header("진행 데이터 (확인용)")]
    private List<QuestionData> currentQuestions; // 현재 진행 중인 질문들
    private int currentIndex = 0;                // 현재 몇 번째 질문인지
    private int totalScore = 0;                  // 누적 점수 (퀴즈 정답수 또는 투자 점수)
    private QuestType currentType;               // 현재 퀘스트의 타입

    private QuestData activeQuest; // 현재 진행 중인 퀘스트 데이터 (결과 처리 시 필요)

    private void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // 시작할 때 창 꺼두기
        if (questionPanel != null) questionPanel.SetActive(false);
    }

    // 퀘스트 UI에서 "시작" 버튼을 눌렀을 때 호출될 함수
    public void StartSequence(QuestData quest)
    {
        activeQuest = quest; // 현재 퀘스트 데이터 저장
        if (quest.questions == null || quest.questions.Count == 0)
        {
            Debug.LogError($"{quest.questName}에 설정된 질문 데이터가 없습니다!");
            return;
        }

        // 데이터 초기화
        currentQuestions = quest.questions;
        currentType = quest.type;
        currentIndex = 0;
        totalScore = 0;

        questionPanel.SetActive(true);
        ShowQuestion();
    }

    // 질문 하나를 화면에 그리는 함수
    private void ShowQuestion()
    {
        // 1. 이전 질문의 버튼들 싹 지우기
        foreach (Transform child in choiceParent)
        {
            Destroy(child.gameObject);
        }

        // 2. 현재 질문 데이터 가져오기
        QuestionData data = currentQuestions[currentIndex];
        questionText.text = data.questionText;

        // 3. 선택지 버튼 생성
        foreach (var choice in data.choices)
        {
            GameObject btnObj = Instantiate(choiceButtonPrefab, choiceParent);
            
            // 버튼 텍스트 설정
            TMP_Text btnText = btnObj.GetComponentInChildren<TMP_Text>();
            if (btnText != null) btnText.text = choice.text;

            // 버튼 클릭 이벤트 연결
            Button btn = btnObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnSelectChoice(choice));
        }
    }

    // 버튼을 눌렀을 때 실행될 로직
    private void OnSelectChoice(Choice choice)
    {
        // 점수 합산 (퀴즈면 정답=1, 투자성향이면 점수 가중치)
        totalScore += choice.score;
        
        currentIndex++;

        // 다음 질문이 있으면 보여주고, 없으면 종료
        if (currentIndex < currentQuestions.Count)
        {
            ShowQuestion();
        }
        else
        {
            CompleteSequence();
        }
    }

    // 모든 질문이 끝났을 때
private void CompleteSequence()
    {
        questionPanel.SetActive(false);
        Debug.Log($"[테스트 완료] 타입: {currentType}, 최종 점수: {totalScore}");

        // 1. 퀴즈 타입일 때의 결과 처리
        if (currentType == QuestType.Quiz)
        {
            // 모든 문제를 다 맞췄을 때만 성공 처리
            if (totalScore >= currentQuestions.Count)
            {
                Debug.Log("<color=green>모든 문제를 맞췄습니다! 퀘스트 성공!</color>");
                FinishQuest();
            }
            else
            {
                Debug.Log($"<color=red>{totalScore}문제만 맞췄네요. 다시 도전해 보세요!</color>");
            }
        }
        // 2. 투자성향 테스트 타입일 때의 결과 처리
        else if (currentType == QuestType.InvestmentTest)
        {
            string investmentStyle = "";

            // 점수 구간 판정 (5문항, 문항당 1~4점 기준 = 총 5~20점)
            if (totalScore <= 8) investmentStyle = "안정형";
            else if (totalScore <= 11) investmentStyle = "안정추구형";
            else if (totalScore <= 14) investmentStyle = "위험중립형";
            else if (totalScore <= 17) investmentStyle = "적극투자형";
            else investmentStyle = "공격투자형";

            Debug.Log($"당신의 투자 성향은 <color=yellow>[{investmentStyle}]</color> 입니다!");
            
            // UI에 성향 결과를 띄워주고 싶다면 여기에 추가 (예: 결과 팝업 창)
            // ResultUI.Instance.ShowResult(investmentStyle);

            // FinanceManager에 성향 저장 (나중에 주식창에서 활용)
            if (FinanceManager.Instance != null)
            {
                FinanceManager.Instance.userInvestmentStyle = investmentStyle;
            }

            // 테스트를 끝냈으니 이 퀘스트도 완료 처리
            FinishQuest();
        }

        PauseController.SetPause(false);
    }

    private void FinishQuest()
    {
        activeQuest.isCompleted = true;
        
        if (activeQuest.unlockFeature != UnlockType.None)
        {
            FinanceManager.Instance.UnlockFinanceFeature(activeQuest.unlockFeature);
        }
    }
}
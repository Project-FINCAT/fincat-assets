using System.Collections.Generic;
using UnityEngine;

// 1. 금융 해금 종류 정의
public enum UnlockType 
{ 
    None, 
    Savings, 
    Loan, 
    Stock, 
    Bond, 
    ISA 
}

// 2. 퀘스트 종류 정의
public enum QuestType { Quiz, InvestmentTest, StockPurchase, General }

[CreateAssetMenu(fileName = "NewQuest", menuName = "Quest System/Quest Data")]
public class QuestData : ScriptableObject
{
    [Header("기본 정보")]
    public string questName;         // 퀘스트 이름
    [TextArea]
    public string description;       // 퀘스트 설명
    public QuestType type;           // 퀘스트 종류
    public List<QuestionData> questions; // 퀴즈 문제 리스트 (Quiz 타입일 때 사용)

    [Header("해금 설정")]
    // --- 이 줄이 추가되어야 QuestionManager의 에러가 사라집니다! ---
    public UnlockType unlockFeature;

    [Header("선행 조건")]
    // 이 퀘스트를 하기 위해 먼저 클리어해야 하는 퀘스트를 드래그해서 넣어야함
    public QuestData prerequisiteQuest; // 선행 퀘스트

    [Header("진행 상태")]
    public bool isAccepted;          // 수락 여부
    public bool isCompleted;         // 완료 여부 (UI에 '해결'로 뜸)

    [Header("보상")]
    public int rewardMoney;          // 보상 금액
    
    // 퀘스트 초기화 (게임 시작 시나 테스트용)
    public void ResetQuest()
    {
        isAccepted = false;
        isCompleted = false;
    }
}
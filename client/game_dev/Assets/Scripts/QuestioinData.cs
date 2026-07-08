using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Choice
{
    public string text;          // 선택지 텍스트 (예: "주식", "예금")
    public int score;           // 투자성향 점수 또는 정답 여부 (정답=1, 오답=0)
    public string nextDescription; // 선택 후 나올 피드백 (생략 가능)
}

[CreateAssetMenu(fileName = "NewQuestion", menuName = "Quest System/Question Data")]
public class QuestionData : ScriptableObject
{
    [TextArea] public string questionText; // 질문 내용
    public List<Choice> choices = new List<Choice>(); // 선택지 리스트
}
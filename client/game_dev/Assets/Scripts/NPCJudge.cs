using UnityEngine;

// NPCJudge가 기존 NPC 클래스를 상속받도록 설정
public class NPCJudge : NPC 
{
    [Header("Judge 전용 UI")]
    public GameObject debateInputPanel;

    // 기존 NPC.cs의 상호작용 함수를 재정의(Override)
    public override void Interact() 
    {
        Debug.Log("판사 NPC와 상호작용 시도");

        // 1. 에이전트 3명의 분석 데이터가 있는지 확인
        if (NewsAnalyzer.Instance.GetCachedData() != null)
        {
            if (debateInputPanel != null)
            {
                debateInputPanel.SetActive(true); // 입력창 띄우기
                Debug.Log("판사 NPC와 토론 시작!");
            }
        }
        else
        {
            // 아직 대화 안 했을 때 처리 (기존 다이얼로그 시스템 활용 가능)
            base.Interact(); // 기존 NPC가 하던 일반 대화창 띄우기 등
            Debug.Log("아직 전문가를 다 만나지 않았습니다.");
        }
    }
}
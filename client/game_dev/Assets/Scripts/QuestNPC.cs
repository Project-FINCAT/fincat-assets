using UnityEngine;
using System.Collections.Generic;

public class QuestNPC : NPC
{
    [Header("퀘스트 설정")]
    public List<QuestData> myQuests = new List<QuestData>();

    protected override void ChooseOption(int nextIndex)
    {
        // -10번 인덱스를 "예(패널 열기)"로 약속합니다.
        if (nextIndex == -10)
        {
            OpenQuestPanel();
            return;
        }

        // -3번 인덱스는 "아니오(대화 종료)"로 처리합니다.
        if (nextIndex == -3)
        {
            EndDialogue();
            return;
        }

        base.ChooseOption(nextIndex);
    }

    private void OpenQuestPanel()
    {
        if (QuestPanelUI.Instance != null)
        {
            // 대화창은 닫고 퀘스트 패널을 활성화
            dialogueUI.ShowDialogueUI(false);
            QuestPanelUI.Instance.OpenPanel(myQuests);
            
            // 상호작용 상태는 유지하여 플레이어 이동 방지
            isDialogueActive = true; 
            PauseController.SetPause(true);
        }
    }
    
    public override void EndDialogue()
    {
        base.EndDialogue();
        // 패널이 열려있다면 닫아줍니다.
        if (QuestPanelUI.Instance != null)
        {
            QuestPanelUI.Instance.ClosePanel();
        }
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class NPCBank : NPC, IInteractable
{
    public NpcDialogue lockedDialogue;
    public NpcDialogue unlockedDialogue;
    public NpcDialogue afterQuestDialogue;
    public DialogueController dialogueController;
    public GameObject bankPanel;
    public bool isBankUnlocked = false;
    public GameObject questPanel;
    
    void Update()
    {
        if (bankPanel.activeSelf && Keyboard.current.zKey.wasPressedThisFrame)
        {
            CloseBank();
        }
        else if(questPanel.activeSelf && Keyboard.current.zKey.wasPressedThisFrame)
        {
            CloseEvent();
        }
    }

    void CloseBank()
    {
        bankPanel.SetActive(false);
        PauseController.SetPause(false);
    }

    void CloseEvent()
    {
        questPanel.SetActive(false);
        PauseController.SetPause(false);
    }

    protected override void ChooseOption(int nextIndex)
    {
        // 은행 해금 안 된 상태
        if (!isBankUnlocked)
        {
            if (nextIndex == -100) // 퀘스트 풀기
            {
                EndDialogue();
                questPanel.SetActive(true);
                PauseController.SetPause(true);
            }
            else if (nextIndex == 1) // 그만두기
            {
                EndDialogue();
            }

            return;
        }

        // 은행 해금된 상태
        if (nextIndex == -2) // Banking
        {
            EndDialogue();
            bankPanel.SetActive(true);
            PauseController.SetPause(true);
            return;
        }
        else if (nextIndex == 2) // Quit
        {
            EndDialogue();
            return;
        }

        base.ChooseOption(nextIndex);
    }

    public void UnlockBank()
    {
        isBankUnlocked = true;
        Debug.Log("은행이 해금되었습니다!");
    }
    
    public override void Interact()
    {
        if (!isBankUnlocked)
        {
            dialogueData = lockedDialogue;
        }
        else
        {
            dialogueData = unlockedDialogue;
        }

        base.Interact(); // NPC StartDialogue() 호출
    }

    public void OnQuestFinished()
    {
        dialogueData = afterQuestDialogue;
        base.Interact();
    }
    
}
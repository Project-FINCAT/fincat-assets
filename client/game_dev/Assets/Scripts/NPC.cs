using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NPC : MonoBehaviour, IInteractable
{
    public NpcDialogue dialogueData;
    
    // 자식(StockBrokerNPC)이 쓸 수 있도록 private을 protected로 변경
    protected DialogueController dialogueUI;
    protected int dialogueIndex;
    protected bool isTyping, isDialogueActive;
    
    //public Item itemForSale; // 추가
    public int itemID; // 추가
    protected InventoryController inventory; // private -> protected 변경
    public GameObject itemInWorld; // 추가 
    protected bool isSold = false; // private -> protected 변경
    public int soldOutDialogueIndex = 3; // 추가

    private FinanceManager playerMoney;

    // 자식이 덮어쓸 수 있도록 virtual 추가
    protected virtual void Start()
    {
        dialogueUI = DialogueController.Instance;
        inventory = FindAnyObjectByType<InventoryController>(); // 추가
    }

    public virtual bool CanInteract()
    {
        return !isDialogueActive;
    }

    public virtual void Interact()
    {
        if(dialogueData == null || (PauseController.IsGamePaused && !isDialogueActive))
            return;

        if(isDialogueActive)
        {
            NextLine();
        }
        else
        {
            StartDialogue();
        }
    }

    protected virtual void StartDialogue()
    {

        Debug.Log("StartDialogue 호출");
        isDialogueActive = true;

        // 아이템을 가지고 있는 경우만 3번 대사
        if (inventory != null && inventory.HasItem(itemID))
        {
            dialogueIndex = soldOutDialogueIndex;
        }
        else
        {
            dialogueIndex = 0;
        }

        dialogueUI.SetNPCInfo(dialogueData.npcName, dialogueData.npcPortrait);
        dialogueUI.ShowDialogueUI(true);
        PauseController.SetPause(true);
        Debug.Log("Pause 상태: " + PauseController.IsGamePaused);

        dialogueUI.onForceCloseCallback = EndDialogue;

        DisplayCurrentLine();
    }

    protected virtual void NextLine()
    {
        if(isTyping)
        {
            StopAllCoroutines();
            dialogueUI.SetDialogueText(dialogueData.dialogueLines[dialogueIndex]);
            isTyping = false;
        }

        dialogueUI.ClearChoices();

        if(dialogueData.endDialogueLines.Length > dialogueIndex && dialogueData.endDialogueLines[dialogueIndex])
        {
            EndDialogue();
            return;
        }

        foreach(DialogueChoice dialogueChoice in dialogueData.choices)
        {
            if(dialogueChoice.dialogueIndex == dialogueIndex)
            {
                DisplayChoices(dialogueChoice);
                return;
            }
        }

        if(++dialogueIndex < dialogueData.dialogueLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    protected virtual IEnumerator TypeLine()
    {
        isTyping = true;
        dialogueUI.SetDialogueText("");

        foreach(char letter in dialogueData.dialogueLines[dialogueIndex])
        {
            dialogueUI.SetDialogueText(dialogueUI.dialogueText.text += letter);
            yield return new WaitForSeconds(dialogueData.typingSpeed);
        }

        isTyping = false;

        if(dialogueData.autoProgressLines.Length > dialogueIndex && dialogueData.autoProgressLines[dialogueIndex])
        {
            yield return new WaitForSeconds(dialogueData.autoProgressDelay);
            NextLine();
        }
    }

    protected virtual void DisplayChoices(DialogueChoice choice)
    {
        for(int i = 0; i < choice.choices.Length; i++)
        {
            int nextIndex = choice.nextDialogueIndexs[i];
            dialogueUI.CreatChoiceButton(choice.choices[i], () => ChooseOption(nextIndex));
        }
    }

    protected virtual void ChooseOption(int nextIndex)
    {
        // 현재 대사 0번에서 "네" 선택 시
        if (dialogueIndex == 0 && nextIndex == 1)
        {
            // 돈이 충분한 경우
            if (FinanceManager.Instance.playerMoney >= 100000000)
            {
                if (inventory != null)
                {
                    bool added = inventory.AddItem(itemID);

                    if (added && itemInWorld != null)
                    {
                        Destroy(itemInWorld);
                        Debug.Log("츄르 제거");
                    }

                    isSold = true;
                }

                dialogueIndex = 1; // 고맙다...
            }
            else
            {
                dialogueIndex = 4; // 돈이 없으니...
            }

            dialogueUI.ClearChoices();
            DisplayCurrentLine();
            return;
        }

        // 일반 선택지 처리
        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }

    protected virtual void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeLine());
    }

    public virtual void EndDialogue() //X 누르면 상호작용 안되는거 같은데 이거 고쳐야할듯
    {
        StopAllCoroutines();
        isDialogueActive = false;
        dialogueUI.ClearChoices(); // 추가
        dialogueUI.SetDialogueText("");
        dialogueUI.ShowDialogueUI(false);
        PauseController.SetPause(false);

        // 🔥 추가
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
        
    }
}
using UnityEngine;
using System.Collections;
using TMPro;

public class DebateController : NPC, IInteractable
{
    [Header("Quest UI")]
    public GameObject questPanel;
    public TMP_Text newsText;

    [Header("Quest 후 상태")]
    public NpcDialogue afterQuestDialogue;

    [Header("외부 시스템")]
    public InventoryController inventoryController;

    private bool questStarted = false;
    private bool questCompleted = false;

    private NpcDialogue defaultDialogue;

    private bool hasShownAfterDialogue = false;    

    [Header("토론 페이지")]
    public GameObject[] debatePages;

    protected override void Start()
    {
        base.Start();

        defaultDialogue = dialogueData;
    }

    // NPC 선택 처리
    protected override void ChooseOption(int nextIndex)
    {
        // 퀘스트 시작 선택
        if (nextIndex == -100)
        {
            StartQuest();
            return;
        }

        dialogueIndex = nextIndex;
        dialogueUI.ClearChoices();
        DisplayCurrentLine();
    }

    // 퀘스트 시작
    void StartQuest()
    {
        questStarted = true;
        ResetDebatePages();

        string news = NewsManager.Instance.GetTodayNewsText();
        newsText.text = news;

        EndDialogue();

        questPanel.SetActive(true);
        PauseController.SetPause(true);
    }

    // 퀘스트 종료 (UI 버튼에서 호출)
    public void CloseQuestPanel()
    {
        questPanel.SetActive(false);
        PauseController.SetPause(false);

        // 보상 지급
        if (inventoryController != null)
        {
            inventoryController.AddItem(5);
            Debug.Log("item4 지급 완료");
        }

        QuestManager.Instance.CompleteQuest("Forum");

        questCompleted = true;

        OnQuestFinished();
    }

    // 퀘스트 완료 후 상태 전환
    public void OnQuestFinished()
    {
        // afterDialogue를 아직 안 보여줬을 때만 실행
        if (!hasShownAfterDialogue && afterQuestDialogue != null)
        {
            dialogueData = afterQuestDialogue;

            dialogueIndex = 0;
            isDialogueActive = false;

            hasShownAfterDialogue = true;

            StartDialogue();
        }
        else
        {
            // 이후에는 그냥 종료만
            EndDialogue();
        }
    }

    public override void Interact()
    {
        // 퀘스트 진행 중이면 NPC 대화 막음
        if (questPanel.activeSelf)
            return;

        base.Interact();
    }

        private void OnEnable()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnTurnChanged += OnTurnChanged;
        }
    }

    private void OnDisable()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnTurnChanged -= OnTurnChanged;
        }
    }

    private void OnTurnChanged(int turn)
    {
        dialogueData = defaultDialogue;

        dialogueIndex = 0;
        isDialogueActive = false;

        Debug.Log("기본 대사로 복귀");
    }

    private void ResetDebatePages()
    {
        for (int i = 0; i < debatePages.Length; i++)
        {
            debatePages[i].SetActive(i == 0);
        }
    }
}
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class NPCOwl : NPC
{
    public string npcId;

    [Header("AI Input UI")]
    public TMP_InputField inputField;
    public Button sendButton;

    [Header("Dialogue")]
    public NpcDialogue lockedDialogue;

    public NpcDialogue unlockedDialogue;
    public NpcDialogue afterQuestDialogue;
    public DialogueController dialogueController;

    [Header("UI Panels")]
    public GameObject aiPanel;
    public GameObject questPanel;


    [Header("Lock State")]
    public bool isAIUnlocked = false;

    private bool isWaitingResponse = false;
    public bool isAIMode = false;

    public static NPCOwl Instance;

    void Awake()
    {
        Instance = this;
    }

    protected override void Start()
    {
        base.Start();

        aiPanel.SetActive(false);
        questPanel.SetActive(false);

        sendButton.onClick.AddListener(OnSendClicked);
    }

    void Update()
    {
        if (aiPanel.activeSelf && Keyboard.current.zKey.wasPressedThisFrame)
        {
            CloseAI();
        }
        else if (questPanel.activeSelf && Keyboard.current.zKey.wasPressedThisFrame)
        {
            CloseQuest();
        }

        if (isDialogueActive && isAIMode)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                OnSendClicked();
            }

            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                EndDialogue();
            }
        }
    }

    // ================================
    // 대화 선택
    // ================================
    protected override void ChooseOption(int nextIndex)
    {
        // 해금 안 된 상태
        if (!isAIUnlocked)
        {
            if (nextIndex == -2) // 퀘스트
            {
                EndDialogue();
                questPanel.SetActive(true);
                PauseController.SetPause(true);
                return;
            }
            else if (nextIndex == -1) // 종료
            {
                EndDialogue();
                return;
            }

            return;
        }

        // 해금된 상태
        if (nextIndex == -100)
        {
            StartAIMode();
            return;
        }

        base.ChooseOption(nextIndex);
    }

    // ================================
    // Unlock (NPCBank 방식)
    // ================================
    public void UnlockAI()
    {
        isAIUnlocked = true;
        Debug.Log("AI 기능이 해금되었습니다!");
    }

    // ================================
    // Interact (locked/unlocked dialogue 적용)
    // ================================
    public override void Interact()
    {
        if (!isAIUnlocked)
        {
            dialogueData = lockedDialogue;
        }
        else
        {
            dialogueData = unlockedDialogue;
        }

        base.Interact();
    }

    public void OnQuestFinished()
    {
        dialogueData = afterQuestDialogue;
        base.Interact();
    }

    // ================================
    // AI 모드
    // ================================
    private void StartAIMode()
    {
        isAIMode = true;

        dialogueUI.ClearChoices();
        dialogueUI.SetDialogueText("어떤 금융지식이 궁금하신가요?");

        aiPanel.SetActive(true);

        inputField.text = "";
        inputField.ActivateInputField();
    }

    private void OnSendClicked()
    {
        if (isWaitingResponse || !isAIMode) return;

        string playerInput = inputField.text;

        if (string.IsNullOrEmpty(playerInput))
            return;

        inputField.text = "";

        StartCoroutine(SendToAI(playerInput));
    }

    private IEnumerator SendToAI(string playerInput)
    {
        isWaitingResponse = true;

        dialogueUI.SetDialogueText("Thinking...");

        yield return StartCoroutine(
            AIManager.Instance.GetAIResponse(
                npcId,
                playerInput,
                (response) =>
                {
                    dialogueUI.SetDialogueText(response);
                }
            )
        );

        isWaitingResponse = false;
        inputField.ActivateInputField();
    }

    // ================================
    // 닫기
    // ================================
    void CloseAI()
    {
        aiPanel.SetActive(false);
        PauseController.SetPause(false);
    }

    void CloseQuest()
    {
        questPanel.SetActive(false);
        PauseController.SetPause(false);
    }

    public override void EndDialogue()
    {
        base.EndDialogue();

        isAIMode = false;
        isWaitingResponse = false;

        aiPanel.SetActive(false);

        inputField.text = "";

        EventSystem.current.SetSelectedGameObject(null);
    }
}
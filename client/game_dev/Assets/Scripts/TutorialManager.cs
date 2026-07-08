using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class TutorialManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text tutorialText;
    [SerializeField] private GameObject hintTextObject;
    [SerializeField] private GameObject riskTestPanel;

    [Header("World")]
    [SerializeField] private WorldMapController worldMap;

    [Header("Player")]
    [SerializeField] private PlayerMovement player;
    [SerializeField] private TabController tabController;

    private int currentStep = 0;
    private bool case2Triggered = false;

    private string playerRiskType;

    private Coroutine typingCoroutine;
    private string currentText = "";
    private bool isTyping = false;

    private Queue<string> dialogueQueue = new Queue<string>();
    private bool isSequenceMode = false;
    private bool step11Triggered = false;

    private int spaceLimitIndex = -1;
    private int currentDialogueIndex = 0;

    void Start()
    {
        player.canMove = false;

        dialoguePanel.SetActive(false);
        riskTestPanel.SetActive(false);

        worldMap.worldMapUI.SetActive(false);

        ShowStep(0);
    }

    public void OnRiskTestComplete(string riskType)
    {
        riskTestPanel.SetActive(false);

        playerRiskType = riskType;

        worldMap.worldMapUI.SetActive(true);

        currentStep = 2;
        dialoguePanel.SetActive(true);
        dialoguePanel.transform.SetAsLastSibling();

        ShowStep(currentStep);
    }

    void Update()
    {
        if (!dialoguePanel.activeSelf) return;

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (hintTextObject != null && !hintTextObject.activeSelf)
                return;

            HandleSpace();
            return;
        }

        if (currentStep == 3 && !case2Triggered && player.gameObject.activeSelf && Vector2.Distance(player.transform.position, new Vector2(38, -28)) < 0.6f)
        {
            case2Triggered = true;
            NextStep();
        }

        if (currentStep == 4 && player.IsMoving)
            NextStep();

        if (currentStep == 5 && Keyboard.current.tabKey.wasPressedThisFrame)
        NextStep();

        if (currentStep == 6 && tabController.CurrentTab == 1)
            NextStep(); // Inventory

        if (currentStep == 7 && tabController.CurrentTab == 2)
            NextStep(); // Map

        if (currentStep == 8 && tabController.CurrentTab == 3)
            NextStep(); // Quest

        if (currentStep == 9 && Keyboard.current.tabKey.wasPressedThisFrame)
        NextStep();

        if (currentStep == 10 && Vector2.Distance(player.transform.position, new Vector2(35, -23)) < 1f)
        NextStep();

        else if (currentStep == 11 && Keyboard.current.zKey.wasPressedThisFrame)
        NextStep();

    }

    void HandleSpace()
    {
        if (isTyping)
        {
            SkipTyping();
            return;
        }

        if (isSequenceMode)
        {
            ShowNextDialogue();
            return;
        }

        NextStep();
    }

    void ShowStep(int step)
    {
        dialoguePanel.SetActive(true);
        dialoguePanel.transform.SetAsLastSibling();

        switch (step)
        {
            case 0:
                SetTextTyping("본격적인 시작 전에 자네의 성향을 알아보는 테스트를 진행하겠네.\n몇 가지 질문을 던질 테니, 솔직하게 답해 보게나.");
                hintTextObject.SetActive(false);
                ShowRiskTest();
                break;

            case 2:
                hintTextObject.SetActive(true);
                dialogueQueue.Clear();

                dialogueQueue.Enqueue("음, 성향 테스트가 모두 끝났구먼.");
                dialogueQueue.Enqueue("자네의 성향을 분석해 보니... 결과는 " + playerRiskType + "라네. 흥미롭군!");
                dialogueQueue.Enqueue("자, 이제 세계를 가볍게 둘러보도록 함세.");
                dialogueQueue.Enqueue("먼저 집이라네. 고단한 하루를 보내고 침대에 누워 쉬다 보면, 자연스레 다음 날을 맞이할 수 있지.");
                dialogueQueue.Enqueue("상점은 자네의 최종 목표인 곳이야. 1억을 모아 보게나.");
                dialogueQueue.Enqueue("은행에선 돈을 관리하는 법을 배우게 될 걸세.");
                dialogueQueue.Enqueue("증권사는 자산을 불릴 기회의 장소라네.");
                dialogueQueue.Enqueue("교육소에서는 금융 지식을 배울 수 있다네.");
                dialogueQueue.Enqueue("토론장에서는 다른 투자자들과 의견을 나눌 수 있을 걸세.");
                dialogueQueue.Enqueue("지금은 집과 상점만 이용 가능하다네.");

                isSequenceMode = true;
                spaceLimitIndex = 8;
                ShowNextDialogue();

                worldMap.canEnterHome = true;
                worldMap.canEnterStore = true;
                
                worldMap.canEnterEducation = false;
                worldMap.canEnterBank = false;
                worldMap.canEnterSecurities = false;
                worldMap.canEnterForum = false;
                break;

            case 3:
                hintTextObject.SetActive(false);
                SetTextTyping("준비가 되었다면 집을 눌러 보게나.");
                break;

            case 4:
                hintTextObject.SetActive(false);
                player.canMove = true;
                SetTextTyping("이동 키는 W/A/S/D 일세. 이동해 보게나.");
                break;

            case 5:
                SetTextTyping("Tab 버튼을 눌러 메뉴를 확인해 보게.\n이곳에서는 여러 가지 정보를 살펴볼 수 있지.");
                break;

            case 6:
                SetTextTyping("Inventory에서는 획득한 카드를 확인할 수 있지.");
                break;
            
            case 7:
                SetTextTyping("지도에서는 원하는 장소로 이동할 수 있다네.");
                break;

            case 8:
                SetTextTyping("퀘스트 페이지에서는 해야 할 일을 확인할 수 있네.");
                break;

            case 9:
                SetTextTyping("이제 Tab 버튼을 다시 눌러 닫아 보게.");
                break;

            case 10:
                SetTextTyping("이제 상호작용하는 방법을 알려 주겠네.\n침대 가까이로 가 보게.\n느낌표가 뜨면 상호작용이 가능하다네.");
                break;

            case 11:
                SetTextTyping("이제 Z 키를 눌러 보게. 상호작용이 될거야.");
                break;

            case 12:
                SetTextTyping("X버튼을 누르면 창을 닫을 수 있지");
                break;

            case 13:
                hintTextObject.SetActive(true);
                dialogueQueue.Clear();

                dialogueQueue.Enqueue("기본적인 조작은 충분히 익혔군.");
                dialogueQueue.Enqueue("보상으로 학교를 개방해 주겠네.");
                dialogueQueue.Enqueue("이제 네 가지 퀘스트를 수행해 보게.");
                dialogueQueue.Enqueue("학교부터가 시작일세.");

                worldMap.canEnterEducation = true;

                isSequenceMode = true;
                ShowNextDialogue();
                break;
        }
    }

    void ShowNextDialogue()
    {
        if (dialogueQueue.Count == 0)
        {
            isSequenceMode = false;

            if (currentStep == 13)
                StartCoroutine(EndTutorial());

            return;
        }

        SetTextTyping(dialogueQueue.Dequeue());
    }

    void ShowRiskTest()
    {
        riskTestPanel.SetActive(true);
        StartCoroutine(HideDialoguePanelAfterDelay(3f));
    }

    IEnumerator HideDialoguePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        dialoguePanel.SetActive(false);
    }

    void NextStep()
    {
        currentStep++;
        ShowStep(currentStep);
    }

    void SetTextTyping(string text)
    {
        currentText = text;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        tutorialText.text = "";

        foreach (char c in text)
        {
            tutorialText.text += c;
            yield return new WaitForSeconds(0.03f);
        }

        isTyping = false;
    }

    void SkipTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        tutorialText.text = currentText;
        isTyping = false;
    }

    IEnumerator EndTutorial()
    {
        yield return new WaitForSeconds(1.5f);

        player.canMove = true;
        dialoguePanel.SetActive(false);
    }

    public void OnCloseButtonClicked()
    {
        if (currentStep == 12 && !step11Triggered)
        {
            step11Triggered = true;
            NextStep();
        }
    }
}
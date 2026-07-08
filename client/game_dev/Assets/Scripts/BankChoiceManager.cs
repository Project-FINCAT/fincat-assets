using UnityEngine;
using UnityEngine.UI;

public class BankChoiceManager : MonoBehaviour
{
    public NPCBank npcBank; // Inspector에서 연결
    [Header("선택지 X 표시 이미지")]
    public GameObject choice1_X;
    public GameObject choice2_X;

    [Header("제출 버튼")]
    public Button submitButton;

    public InventoryController inventoryController;

    [SerializeField] private WorldMapController worldMap;
    public GameObject q3_2_Panel;
    private int selectedChoice = 0; // 0: 없음, 1: 1번, 2: 2번
    private bool isSubmitted = false;
    public StockBrokerNPC stockNPC;

    void Start()
    {
        // 처음엔 제출 버튼 비활성화
        submitButton.interactable = false;

        // X 표시도 초기화
        choice1_X.SetActive(false);
        choice2_X.SetActive(false);
    }

    // ================================
    // 선택지 1 클릭
    // ================================
    public void OnClickChoice1()
    {
        if (isSubmitted) return;

        if (selectedChoice == 1)
        {
            selectedChoice = 0;
            choice1_X.SetActive(false);
        }
        else
        {
            selectedChoice = 1;
            choice1_X.SetActive(true);
            choice2_X.SetActive(false);
        }

        UpdateSubmitButton();
    }

    // ================================
    // 선택지 2 클릭
    // ================================
    public void OnClickChoice2()
    {
        if (isSubmitted) return;

        if (selectedChoice == 2)
        {
            selectedChoice = 0;
            choice2_X.SetActive(false);
        }
        else
        {
            selectedChoice = 2;
            choice2_X.SetActive(true);
            choice1_X.SetActive(false);
        }

        UpdateSubmitButton();
    }

    // ================================
    // 제출 버튼 클릭
    // ================================
    public void OnClickSubmit()
{
    if (selectedChoice == 0)
    {
        Debug.Log("선택지를 먼저 선택하세요.");
        return;
    }

    int correctAnswer = 2; // 정답 번호

    isSubmitted = true;
    submitButton.interactable = false;

    if (selectedChoice == correctAnswer)
    {
        Debug.Log("정답입니다!");
        if (stockNPC != null)
        {
            stockNPC.UnlockStock();
        }

        if (inventoryController != null)
        {
            inventoryController.AddItem(4); // 원하는 아이템 ID
        }

        // 퀘스트 완료
        QuestManager.Instance.CompleteQuest("Securities");
        q3_2_Panel.SetActive(false);

        PauseController.SetPause(false);

        if (worldMap != null)
        {
            worldMap.canEnterForum = true;
            Debug.Log("토론장 입장 가능!");
        }
        else
        {
            Debug.LogError("WorldMap 연결 안됨!");
        }

        if (stockNPC != null)
        {
            stockNPC.OnQuestFinished();
        }
    }

    else
    {
        Debug.Log("오답입니다!");
        submitButton.interactable = true;
        isSubmitted = false;
    }
}
    // ================================
    // Submit 버튼 상태 업데이트
    // ================================
    void UpdateSubmitButton()
    {
        submitButton.interactable = (selectedChoice != 0);
    }
}
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuestPanelUI : MonoBehaviour
{
    public static QuestPanelUI Instance { get; private set; }

    [Header("UI References")]
    public GameObject panelObject;      // 실제 QuestPanel (이미지 등이 있는 오브젝트)
    public Transform contentParent;    // QuestEntry들이 생성될 부모 (Content)
    public GameObject questEntryPrefab; // QuestEntry 프리팹
    public List<QuestData> allQuests; // 모든 퀘스트 데이터 리스트 (인스펙터에서 할당)

    private void Awake()
    {
       if (Instance == null) 
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // 필요하다면 유지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        foreach (var quest in allQuests)
        {
            quest.isCompleted = false;
            quest.isAccepted = false;
            // FinanceManager의 해금 상태도 초기화하고 싶다면 여기서!
        }
        
        // 주식/채권 해금 변수들도 false로 초기화 (FinanceManager에서 담당)
        if (FinanceManager.Instance != null)
        {
            FinanceManager.Instance.isStockUnlocked = false;
            FinanceManager.Instance.isBondUnlocked = false;
        }
        // 시작할 때 패널은 꺼둡니다.
        if (panelObject != null) panelObject.SetActive(false);
    }

    public void OpenPanel(List<QuestData> quests)
    {
        if (panelObject != null)
        {
            panelObject.SetActive(true);
            RefreshList(quests);
        }
    }

    public void ClosePanel()
    {
        if (panelObject != null)
        {
            panelObject.SetActive(false);
            PauseController.SetPause(false); // 일시정지 해제
        }
    }

    public void RefreshList(List<QuestData> quests)
    {
        foreach (Transform child in contentParent) Destroy(child.gameObject);

        foreach (var quest in quests)
        {
            if (quest == null) continue;

            GameObject entry = Instantiate(questEntryPrefab, contentParent);
            
            // --- 경로 재설정 (하이러키 구조와 1:1 매칭 필수) ---
            // 1. LeftContent 내부의 텍스트들
            Transform titleTr = entry.transform.Find("LeftContent/TitleText");
            Transform descTr = entry.transform.Find("LeftContent/DescriptionText");
            
            // 2. QuestEntry 바로 아래에 있는 항목들
            Transform statusTr = entry.transform.Find("StatusText");
            Transform buttonTr = entry.transform.Find("ActionButton");

            // 데이터 적용 전 체크
            if (titleTr != null) titleTr.GetComponent<TMP_Text>().text = quest.questName;
            if (descTr != null) descTr.GetComponent<TMP_Text>().text = quest.description;
            
            if (statusTr != null) {
                statusTr.GetComponent<TMP_Text>().text = quest.isCompleted ? "<color=green>해결</color>" : "<color=red>미해결</color>";
            }

            // 버튼 처리
            if (buttonTr != null) {
                Button btn = buttonTr.GetComponent<Button>();
                TMP_Text btnText = btn.GetComponentInChildren<TMP_Text>();
                
                btn.interactable = !quest.isCompleted;
                btnText.text = quest.isCompleted ? "완료됨" : "시작";
                
                btn.onClick.RemoveAllListeners(); // 중복 리스너 방지
                btn.onClick.AddListener(() => OnClickStartQuest(quest));
            }
            else {
                // 버튼을 못 찾으면 하이러키의 모든 자식 이름을 출력해서 범인을 찾습니다.
                string children = "";
                foreach(Transform t in entry.transform) children += t.name + ", ";
                Debug.LogError($"[QuestPanelUI] 'ActionButton'을 못 찾음. 현재 자식들: {children}");
            }
        }
    }
    // 버튼 클릭 시 실행될 함수
    private void OnClickStartQuest(QuestData quest)
    {
        if (DialogueController.Instance != null)
        {
            DialogueController.Instance.ForceCloseDialogue();
        }

        if (quest.prerequisiteQuest != null && !quest.prerequisiteQuest.isCompleted)
        {
            // 선행 퀘스트가 미완료라면 실행 차단
            string preName = quest.prerequisiteQuest.questName;
            Debug.LogWarning($"잠겨 있음: '{preName}'을(를) 먼저 완료해야 합니다.");
            
            // (팁) UI의 안내 텍스트가 있다면 여기에 표시하세요.
            // if (statusText != null) statusText.text = $"'{preName}' 선행 필요";
            return; // 여기서 함수를 끝내버려서 패널이 닫히지 않게 함
        }
        Debug.Log($"{quest.questName} 수락됨. 타입: {quest.type}");

        // 1. 현재 퀘스트 목록 패널 닫기 (panelObject가 있다면 그것을 끕니다)
        if (panelObject != null) panelObject.SetActive(false);
        else gameObject.SetActive(false);

        // 2. 퀘스트 타입에 따라 분기
        switch (quest.type)
        {
            case QuestType.Quiz:
            case QuestType.InvestmentTest: // 둘 다 동일한 질문 시스템 사용
                if (QuestionManager.Instance != null)
                {
                    QuestionManager.Instance.StartSequence(quest);
                }
                else
                {
                    Debug.LogError("QuestionManager Instance를 찾을 수 없습니다!");
                }
                break;

            case QuestType.StockPurchase:
                Debug.Log("주식 구매 퀘스트입니다. 상점 UI로 안내하세요.");
                PauseController.SetPause(false); // 일단 일시정지 해제
                break;
                
            default:
                PauseController.SetPause(false);
                break;
        }
    }
}
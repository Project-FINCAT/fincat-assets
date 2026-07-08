using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections; // 코루틴 사용을 위해 추가

public class StockBrokerNPC : NPC, IInteractable
{
    [Header("Dialogue")]
    public NpcDialogue lockedDialogue;       
    public NpcDialogue unlockedDialogue;     
    public NpcDialogue afterQuestDialogue;   

    [Header("UI")]
    public StockMarketUI stockMarketUI;
    public GameObject questPanel;
    public GameObject marketPanel;  

    [Header("State")]
    public bool isStockUnlocked = false;     

    void Update()
    {
        if (Keyboard.current == null) return;

        if (questPanel != null && questPanel.activeSelf &&
            Keyboard.current.zKey.wasPressedThisFrame)
        {
            CloseQuest();
        }

        if (stockMarketUI != null && stockMarketUI.gameObject.activeSelf &&
            Keyboard.current.zKey.wasPressedThisFrame)
        {
            CloseStock();
        }
    }

    private void CloseStock()
    {
        if (stockMarketUI != null) stockMarketUI.gameObject.SetActive(false);
        PauseController.SetPause(false);
    }

    private void CloseQuest()
    {
        if (questPanel != null) questPanel.SetActive(false);
        PauseController.SetPause(false);
    }

    private void OpenStockUI()
    {
        if (dialogueUI != null)
        {
            // 대화 UI의 상태를 하이어라키와 컴포넌트 둘 다 체크
            dialogueUI.gameObject.SetActive(false); 
        }

        if (marketPanel != null)
        {
            marketPanel.SetActive(true);
            
            CanvasGroup canvasGroup = marketPanel.GetComponent<CanvasGroup>();
            float alpha = canvasGroup != null ? canvasGroup.alpha : 1.0f;
        

            if (stockMarketUI != null)
            {
                stockMarketUI.ExecuteOpen();
            }
            
            PauseController.SetPause(true);
        }
    }

    protected override void ChooseOption(int nextIndex)
    {
        Debug.Log($"[StockBrokerNPC] ChooseOption 호출 - nextIndex: {nextIndex}, 해금상태: {isStockUnlocked}");
        
        // 플레이어가 주식창 열기(-2)를 선택한 경우
        if (nextIndex == -2)
        {
            if (!isStockUnlocked)
            {
                UnlockStock();
            }

            Debug.Log("[StockBrokerNPC] 주식창 열기 프로세스 작동");
            
            // 기존 대화 종료 처리
            EndDialogue();
            
            StartCoroutine(WaitAndOpenStockUI());
            return;
        }

        if (nextIndex == -100)
        {
            EndDialogue();
            if (questPanel != null) questPanel.SetActive(true);
            PauseController.SetPause(true);
            return;
        }

        if (nextIndex == 1 || nextIndex == 2)
        {
            if (dialogueData == afterQuestDialogue && !isStockUnlocked)
            {
                UnlockStock();
            }
            EndDialogue();
            return;
        }

        base.ChooseOption(nextIndex);
    }
    private IEnumerator WaitAndOpenStockUI()
    {
        yield return new WaitForSeconds(0.2f); 
        
        OpenStockUI();
        
        // 확인 사살용 렌더링 유지 체크
        yield return new WaitForSeconds(0.3f);
        if (marketPanel != null && !marketPanel.activeSelf)
        {
            marketPanel.SetActive(true);
            if (stockMarketUI != null) stockMarketUI.UpdateUI();
        }
    }

    public void UnlockStock()
    {
        isStockUnlocked = true;
        if (FinanceManager.Instance != null)
        {
            FinanceManager.Instance.isStockUnlocked = true;
        }
    }

    public override void Interact()
    {
        // 대화창이 수동으로 다시 열릴 때를 대비해 오브젝트를 다시 켜줍니다.
        if (dialogueUI != null && !dialogueUI.gameObject.activeSelf)
        {
            dialogueUI.gameObject.SetActive(true);
        }

        dialogueData = isStockUnlocked ? unlockedDialogue : lockedDialogue;
        base.Interact();
    }

    public void OnQuestFinished()
    {
        if (questPanel != null)
        {
            questPanel.SetActive(false);
        }

        dialogueData = afterQuestDialogue;
        base.Interact(); 
    }
}
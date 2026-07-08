using UnityEngine;
//using System.Collections;
using TMPro;
using System.Collections.Generic;

using System.Linq;


public class StockMarketUI : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject marketPanel;


    [Header("슬롯 설정 (주식, 채권)")]
    public List<StockItem> stockSlots = new List<StockItem>();
    public List<StockItem> bondSlots = new List<StockItem>();


    [Header("Player Info")]
    public TMP_Text playerMoneyText;
    public TMP_Text infoMessageText;

    private void Start()
    {
        if (marketPanel != null)
            marketPanel.SetActive(false);
    }

    public void OpenUI() // 이 함수 없어도 되지 않나..? 
    { // BrokerNPC에서 바로 ExecuteOpen()호출하는데 근데 예전에 이거땜에 안됐던 기억이 또 있음 일단 냅둬
        if (FinanceManager.Instance.isStockUnlocked)
        {
            Invoke("ExecuteOpen", 0.05f); // 주식 퀘스트를 깼다면, 0.05초 후에 ExecuteOpen 함수 실행
        }
        else
        {
            Debug.LogWarning("주식 거래 자격이 없습니다! '주식 기초' 퀘스트를 완료하세요.");
        }
        
    }

    public void ExecuteOpen()
    {
        Debug.Log($"[StockMarketUI] ExecuteOpen 호출 - FinanceManager.isStockUnlocked: {FinanceManager.Instance.isStockUnlocked}");
        
        // NPC 단에서 이미 true로 만들었겠지만, 확실하게 한 번 더 밀어줍니다.
        if (FinanceManager.Instance != null)
        {
            FinanceManager.Instance.isStockUnlocked = true;
        }

        if (marketPanel != null)
        {
            marketPanel.SetActive(true);
            UpdateUI();
            PauseController.SetPause(true);
            Debug.Log($"[StockMarketUI] marketPanel이 최종적으로 확실하게 화면에 켜졌습니다! (activeSelf: {marketPanel.activeSelf})");
        }
    }

    public void CloseUI()
    {
        marketPanel.SetActive(false);
        PauseController.SetPause(false);
    }

    public void UpdateUI()
    {
        playerMoneyText.text = $"내 잔고: {FinanceManager.Instance.playerMoney}원";

        // 1. 주식 데이터만 필터링해서 주식 슬롯에 배치
        var stocksOnly = StockManager.Instance.stocks.Where(s => !s.isBond).ToList();
        for (int i = 0; i < stockSlots.Count; i++)
        {
            if (i < stocksOnly.Count)
            {
                stockSlots[i].gameObject.SetActive(true);
                stockSlots[i].Setup(stocksOnly[i], this);
                stockSlots[i].UpdateDisplay();
            }
            else
            {
                stockSlots[i].gameObject.SetActive(false); // 데이터가 없으면 슬롯 숨기기
            }
        }

        // 2. 채권 데이터만 필터링해서 채권 슬롯에 배치
        var bondsOnly = StockManager.Instance.stocks.Where(s => s.isBond).ToList();
        for (int i = 0; i < bondSlots.Count; i++)
        {
            if (i < bondsOnly.Count)
            {
                bondSlots[i].gameObject.SetActive(true);
                bondSlots[i].Setup(bondsOnly[i], this);
                bondSlots[i].UpdateDisplay();
            }
            else
            {
                bondSlots[i].gameObject.SetActive(false);

            }
        }
    }

    public void BuyStock(StockData stock)
    {
        // 1. 공통으로 돈을 먼저 뺍니다.
        if (FinanceManager.Instance.SpendMoney(stock.currentPrice))
        {
            // 2. [핵심] 채권인지 주식인지 판단해서 다른 함수를 호출합니다!
            if (stock.isBond)
            {
                // 채권이면: myBonds 리스트에 추가하는 로직 포함
                FinanceManager.Instance.BuyBond(stock, 1);
            }
            else
            {
                // 주식이면: 기존 방식대로 수량만 증가
                FinanceManager.Instance.AddStock(stock.stockName, 1, stock.currentPrice);
            }


            infoMessageText.text = $"{stock.stockName} 매수 완료!";
            UpdateUI();
        }
        else
        {
            infoMessageText.text = "잔고가 부족합니다!";
        }
    }

    public void SellStock(StockData stock)
    {
        if (FinanceManager.Instance.GetStockCount(stock.stockName) > 0)
        {
            // 세금 계산 추가
            int totalTax = FinanceManager.Instance.CalculateSellTax(stock.stockName, stock.currentPrice);
            int finalPrice = stock.currentPrice - totalTax;


            FinanceManager.Instance.AddStock(stock.stockName, -1, stock.currentPrice);
            FinanceManager.Instance.EarnMoney(finalPrice);

            string taxMsg = totalTax > 0 ? $"(세금 {totalTax}원 차감)" : "(비과세 혜택 적용!)";
            infoMessageText.text = $"{stock.stockName} 매도 완료! {taxMsg}";

            UpdateUI();
        }
        else
        {
            infoMessageText.text = "팔 수 있는 자산이 없습니다!";

        }
    }
}
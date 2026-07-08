using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StockItem : MonoBehaviour
{
    [Header("UI 연결")]
    public TMP_Text stockNameText;  // 종목명 텍스트
    public TMP_Text priceText;      // 가격 텍스트
    public TMP_Text ownedText;      // 보유 수량 텍스트
    public Button buyButton;        // 매수 버튼
    public Button sellButton;        // 매도 버튼

    private StockData targetData;

    private StockMarketUI parentUI;

    // StockMarketUI에서 이 함수를 호출하여 데이터를 꽂아줍니다. 
    public void Setup(StockData data, StockMarketUI ui)
    {

        targetData = data;

        parentUI = ui;

        UpdateDisplay();

        // 버튼 클릭 이벤트 연결 (중복 방지를 위해 기존 리스너 제거 후 추가)
        buyButton.onClick.RemoveAllListeners();

        buyButton.onClick.AddListener(() => parentUI.BuyStock(targetData));

        sellButton.onClick.RemoveAllListeners();
        sellButton.onClick.AddListener(() => parentUI.SellStock(targetData));

    }

    // 텍스트 정보 갱신
    public void UpdateDisplay()
    {

        if (targetData == null) return;

        // 이름 표시: 채권인 경우 이름 뒤에 (채권) 표시를 추가하여 구분
        stockNameText.text = targetData.isBond ? $"{targetData.stockName} (Bond)" : targetData.stockName;
        
        // 가격 표시
        priceText.text = $"{targetData.currentPrice}원";
        
        // 보유 수량 표시: FinanceManager의 딕셔너리에서 내 종목 이름으로 수량을 가져옴
        int myCount = FinanceManager.Instance.GetStockCount(targetData.stockName);

        ownedText.text = $"{myCount}주";
    }
}
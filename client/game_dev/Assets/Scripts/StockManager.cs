using System.Collections.Generic;
using UnityEngine;

public class StockManager : MonoBehaviour
{
    public static StockManager Instance { get; private set; }


    [Header("주식 및 채권 리스트")]
    // 이 리스트에 주식과 채권 3개를 모두 삽입해야함 (통합관리)
    public List<StockData> stocks = new List<StockData>();


    [Header("참조")]
    public NewsDatabase newsDatabase;

    private NewsData selectedNewsThisTurn; // 이번 턴에 선정된 뉴스 데이터

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            InitializePrices(); // 게임 시작시 가격 초기화
        } 

        else Destroy(gameObject);
    }

    private void Start()
    {
        if (TurnManager.Instance != null)
        {
            // 턴 변경 시 주가 업데이트 및 이자 지급 이벤트 구독
            TurnManager.Instance.OnStockPriceChanged += UpdateMarketStatus;
        }
    }


    private void InitializePrices()
    {
        foreach (var asset in stocks)
        {
            if (asset != null)
            {
                asset.currentPrice = asset.startPrice;
            }
        }
    }

 
    // [통합] 주가 업데이트와 이자 지급을 한 번에 처리
   private void UpdateMarketStatus()
    {
        if (NewsManager.Instance != null)
        {
            NewsManager.Instance.SelectNewsForToday(); // 이번 턴 뉴스 선정 (StockManager에서 딱 한 번만 호출)
            selectedNewsThisTurn = NewsManager.Instance.GetTodayNewsData();
        }

        // 2. [시장 업데이트] 모든 자산을 순회하며 가격 변동
        foreach (var asset in stocks)
        {
            if (asset == null) continue;

            if (asset.isBond) 
                UpdateBondPrice(asset); // 채권 로직 (뉴스 영향 X)
            else 
                UpdateStockPrice(asset); // 주식 로직 (여기서 selectedNewsThisTurn을 사용함!)
        }

        // 3. [채권 정산] 만기 처리
        ProcessBondMaturity(); 

        // 4. [UI 갱신]
        StockMarketUI marketUI = Object.FindAnyObjectByType<StockMarketUI>();
        if (marketUI != null) marketUI.UpdateUI();


        int currentTurn = TurnManager.Instance != null ? TurnManager.Instance.GetCurrentTurn() : 0;
        Debug.Log($"[턴 {currentTurn}] 뉴스 선정 및 가격 반영 완료");
    }

    private void UpdateStockPrice(StockData stock)
    {
        float newsImpact = 0f;

        // 아까 NewsManager에서 받아온 오늘의 뉴스가 있다면 그 안의 Impacts를 뒤짐
        if (selectedNewsThisTurn != null && selectedNewsThisTurn.impacts != null)
        {
            foreach (var impact in selectedNewsThisTurn.impacts)
            {
                if (impact.targetStockName == stock.stockName)
                {
                    newsImpact = impact.fluctuationRate;
                    break;
                }
            }
        }

        // 주가 계산 (뉴스 영향 + 미세 랜덤 변동)
        float randomNoise = Random.Range(-0.01f, 0.01f);
        stock.currentPrice = Mathf.RoundToInt(stock.currentPrice * (1 + newsImpact + randomNoise));

        if (stock.currentPrice < 100) stock.currentPrice = 100;
    }


    // 채권 가격이 만기에 가까울수록 원금(Par Value)에 다가가는 로직
    private void UpdateBondPrice(StockData bond)
    {
        int parValue = 10000; // 설정하신 원금(액면가)
        
        // 1. 변동폭 최소화 (주식보다 훨씬 작게 설정: 0.1% 내외)
        float marketFluc = Random.Range(-0.002f, 0.002f); 
        bond.currentPrice = Mathf.RoundToInt(bond.currentPrice * (1 + marketFluc));

        // 2. Pulling-to-Par (원금 수렴)
        // 현재 보유 중인 채권들 중 이 종목과 일치하는 가장 가까운 만기일을 찾습니다.
        var myBond = FinanceManager.Instance.myBonds.Find(b => b.bondName == bond.stockName);
        
        if (myBond != null)
        {
            // 남은 기간이 짧을수록 수렴 강도를 강하게 설정 (Lerp 비율 조정)
            // 만기가 많이 남았으면 천천히, 가까우면 빠르게 원금에 붙음
            float t = 1f - ((float)myBond.remainingTurns / myBond.totalDuration);
            float convergenceStrength = Mathf.Lerp(0.01f, 0.1f, t); // 1% ~ 10% 사이의 강도로 수렴
            
            bond.currentPrice = Mathf.RoundToInt(Mathf.Lerp(bond.currentPrice, parValue, convergenceStrength));
        }
    }

    private void ProcessBondMaturity()
    {
        // 1. 리스트 및 인스턴스 접근 확인
        if (FinanceManager.Instance == null || FinanceManager.Instance.myBonds == null)
        {
            Debug.LogError("<color=red>🚨 FinanceManager 또는 myBonds 리스트가 Null입니다!</color>");
            return;
        }

        var myBonds = FinanceManager.Instance.myBonds;
        if (myBonds.Count > 0)
            Debug.Log($"<color=white>🔍 [정산 시작] 현재 보유 채권 뭉치: {myBonds.Count}개</color>");

        for (int i = myBonds.Count - 1; i >= 0; i--)
        {
            var b = myBonds[i];
            
            // 2. 턴 차감
            b.remainingTurns--;
            Debug.Log($"<color=yellow>👉 {b.bondName} 검사 중 (남은 턴: {b.remainingTurns + 1} -> {b.remainingTurns})</color>");

            // 3. 만기 조건 검사
            if (b.remainingTurns <= 0)
            {
                Debug.Log($"<color=cyan>🎊 {b.bondName} 만기 도래! 세금 계산 및 정산을 시작합니다.</color>");

                StockData data = stocks.Find(s => s.stockName == b.bondName);
                if (data == null)
                {
                    Debug.LogError($"<color=red>❌ 에러: {b.bondName}의 StockData를 찾지 못해 정산을 중단합니다.</color>");
                    continue;
                }

                // --- [세금 및 정산 로직 시작] ---

                // A. 원금 (세금 대상 아님)
                int refund = b.parValue * b.amount;

                // B. 이자 (세전)
                int grossInterest = Mathf.RoundToInt(b.parValue * data.interestRate * b.amount);

                // C. 세금 계산
                int tax = 0;
                if (FinanceManager.Instance.hasISAAccount)
                {
                    // ISA 계좌가 있으면 비과세 (0원)
                    tax = 0;
                    Debug.Log($"<color=lime>[ISA 혜택 적용] {b.bondName} 이자 소득세가 면제되었습니다!</color>");
                }
                else
                {
                    // 일반 이자소득세 15.4% 적용
                    tax = Mathf.RoundToInt(grossInterest * 0.154f);
                }

                // D. 최종 수령액 계산
                int netInterest = grossInterest - tax;
                int totalPayout = refund + netInterest;

                // --- [데이터 반영] ---

                // 입금 및 인벤토리 제거
                FinanceManager.Instance.EarnMoney(totalPayout);
                FinanceManager.Instance.AddStock(b.bondName, -b.amount, 0); 

                Debug.Log($"<color=green>💰 정산 완료: {b.bondName}</color>\n" +
                        $"<color=white>- 원금 환급: {refund}원\n" +
                        $"- 이자 수익(세후): {netInterest}원 (세금: {tax}원)\n" +
                        $"- <b>최종 입금액: {totalPayout}원</b></color>");

                myBonds.RemoveAt(i);
            }
        }
    }


    public NewsData GetCurrentNews() => selectedNewsThisTurn;

    private void OnDestroy()
    {
        if (TurnManager.Instance != null)
        {

            TurnManager.Instance.OnStockPriceChanged -= UpdateMarketStatus;

        }
    }
}
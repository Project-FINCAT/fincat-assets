using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class BondInventory // 채권 보유 정보를 담는 클래스
{
    public string bondName;
    public int purchasePrice;  // 산 가격
    public int parValue;       // 액면가 (만기 시 돌려받을 원금)
    public int remainingTurns; // 남은 기간
    public int totalDuration;  // 전체 기간
    public int amount;
}
// 플레이어의 자산(돈, 보유 주식 수)과 퀘스트 상태를 관리하는 매니저
public class FinanceManager : MonoBehaviour
{
    public static FinanceManager Instance { get; private set; }

    [Header("Player Finance")]
    public int playerMoney = 0;
    public int bankMoney = 0; // 은행 계좌 잔액
    
    public string userInvestmentStyle = "미정"; // 플레이어의 투자 성향 저장용

    [Header("Bank Interest")]
    public float savingInterestRate = 0.02f; // 적금 2%
    public float depositInterestRate = 0.01f; // 예금 1%
    public float loanInterestRate = 0.03f; // 대출이자 1%

    [Header("Salary System")]
    public int salaryPerTurn = 2500000;


    [Header("Quest Status")]
    public bool isStockQuestCompleted = true; // 주식 기초 퀘스트 완료 여부

    // --- 신규 금융 해금 상태 변수들 ---
    [Header("Finance Unlock Status")]
    public bool isSavingsUnlocked = false;    // 예적금 해금
    public bool isLoanUnlocked = false;       // 대출 해금
    public bool isStockUnlocked = true;      // 주식 해금
    public bool isBondUnlocked = true;       // 채권 해금
    public bool isISAUnlocked = false;        // ISA 계좌 해금

    [Header("Bank Account")]
    public List<BankAccountData> accounts = new List<BankAccountData>();
    public int maxAccounts = 3;

        // [중요] 외부에서 인스펙터로 확인 가능하도록 public 유지
    public List<BondInventory> myBonds = new List<BondInventory>(); 

    [Header("ISA계좌 및 세금 설정")]
    public bool hasISAAccount = false;
    public int taxFreeLimit = 3000000;
    public float tradingTaxRate = 0.002f;
    public float capitalGainsTax = 0.22f;

    private Dictionary<string, int> ownedStocks = new Dictionary<string, int>(); 
    private Dictionary<string, int> averagePurchasePrice = new Dictionary<string, int>();
    private int accumulatedProfit = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

        private void Start()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnBankInterestApplied += ApplyInterestAndSalary;
        }
    }

    public void AddStock(string stockName, int amount)
    {
        if (!ownedStocks.ContainsKey(stockName))
            ownedStocks[stockName] = 0;

        ownedStocks[stockName] += amount;
        
        // 수량이 0 아래로 내려가지 않게 방어
        if (ownedStocks[stockName] < 0) ownedStocks[stockName] = 0;
    }

    // 돈 쓰기 (성공하면 true 반환)

    public bool SpendMoney(int amount)
    {
        if (playerMoney >= amount)
        {
            playerMoney -= amount;
            return true;
        }
        return false;
    }


    // 돈 벌기
    public void EarnMoney(int amount)
    {
        playerMoney += amount;
    }

    // 은행에 입금
    public bool DepositMoney(int amount, int accountIndex)
    {
        if(playerMoney < amount) return false;

        playerMoney -= amount;
        accounts[accountIndex].balance += amount;

        return true;
    }

    // 은행에서 출금
    public bool WithdrawMoney(int amount, int accountIndex)
    {
        if(accounts[accountIndex].balance < amount) return false;

        accounts[accountIndex].balance -= amount;
        playerMoney += amount;

        return true;
    }
    //계좌 개설
    public void CreateAccount(string type, string password)
    {
        if(accounts.Count >= maxAccounts)
        {
            Debug.Log("계좌는 최대 3개까지 생성할 수 있습니다.");
            return;
        }
        BankAccountData newAccount = new BankAccountData();

        newAccount.accountType = type;
        newAccount.password = password;
        newAccount.balance = 0;

        accounts.Add(newAccount);

        Debug.Log("계좌 생성 완료 : " + type);
    }

        private void ApplyInterestAndSalary()
    {
        ApplyInterest();
        ReceiveSalary();
    }
    
    // 이자 적용
    private void ApplyInterest()
    {
        if (accounts.Count == 0) return;

        foreach (BankAccountData account in accounts)
        {
            float rate = 0f;

            if (account.accountType == "Saving")
                rate = savingInterestRate;

            else if (account.accountType == "Deposit")
                rate = depositInterestRate;

            if (rate == 0f) continue;

            int interest = Mathf.FloorToInt(account.balance * rate);

            account.balance += interest;

            Debug.Log($"[은행] {account.accountType} 계좌 이자 지급: {interest}");
        }
    }

    private void ReceiveSalary()
    {
        playerMoney += salaryPerTurn;

        Debug.Log($"[월급] 지급: {salaryPerTurn}");
    }
    
    // 계좌생성 제한
    public void AccountRestrict(string type, string password)
    {
        if(accounts.Count >= maxAccounts)
        {
            Debug.Log("계좌는 최대 3개까지 생성할 수 있습니다.");
            return;
        }

        BankAccountData newAccount = new BankAccountData();
        newAccount.accountType = type;
        newAccount.password = password;
        newAccount.balance = 0;

        accounts.Add(newAccount);
    }

    // [핵심 수정] 채권 구매 시 리스트에 확실히 추가
    public void BuyBond(StockData bond, int amount)
    {
        // BondInventory 클래스의 변수들에 StockData의 값을 매칭합니다.
        BondInventory newBond = new BondInventory {
            bondName = bond.stockName,
            purchasePrice = bond.currentPrice,
            
            // 1. parValue: StockData에 없으므로 시작가를 원금으로 보거나 100,000으로 고정
            parValue = bond.startPrice > 0 ? bond.startPrice : 100000, 
            
            // 2. [수정] maturityTurns 대신 durationTurns 사용
            remainingTurns = bond.durationTurns,
            totalDuration = bond.durationTurns,
            
            amount = amount
        };

        myBonds.Add(newBond);
        
        // 인벤토리 수량 갱신 (UI용)
        AddStock(bond.stockName, amount, bond.currentPrice);

        Debug.Log($"<color=green>채권 구매 및 리스트 추가: {bond.stockName} (만기: {bond.durationTurns}턴)</color>");
    }

    // [참고] 만기 정산은 이제 StockManager.cs의 ProcessBondMaturity에서 전담합니다.
    // FinanceManager의 UpdateBondTurns는 중복 방지를 위해 삭제하거나 호출하지 마세요.

    public void AddStock(string stockName, int amount, int currentPrice)
    {
        if (!ownedStocks.ContainsKey(stockName))
        {
            ownedStocks[stockName] = 0;
            averagePurchasePrice[stockName] = 0;
        }

        if (amount > 0) 
        {
            int totalCost = (ownedStocks[stockName] * averagePurchasePrice[stockName]) + (amount * currentPrice);
            ownedStocks[stockName] += amount;
            if (ownedStocks[stockName] > 0)
                averagePurchasePrice[stockName] = totalCost / ownedStocks[stockName];
        }
        else 
        {
            ownedStocks[stockName] += amount;
            if (ownedStocks[stockName] <= 0)
            {
                ownedStocks[stockName] = 0;
                averagePurchasePrice[stockName] = 0;
            }
        }
    }

    public int GetStockCount(string stockName)
    {
        return ownedStocks.ContainsKey(stockName) ? ownedStocks[stockName] : 0;
    }

    public int CalculateSellTax(string stockName, int currentPrice)
    {
        if (!averagePurchasePrice.ContainsKey(stockName)) return 0;

        int tradingTax = Mathf.RoundToInt(currentPrice * tradingTaxRate);
        int profit = currentPrice - averagePurchasePrice[stockName];
        int capitalTax = 0;

        if (profit > 0)
        {
            if (hasISAAccount && accumulatedProfit < taxFreeLimit)
            {
                accumulatedProfit += profit;
                capitalTax = 0;
            }
            else capitalTax = Mathf.RoundToInt(profit * capitalGainsTax);
        }
        return tradingTax + capitalTax;

    }

    public void UnlockFinanceFeature(UnlockType type)
    {
        switch (type)
        {
            case UnlockType.Savings: isSavingsUnlocked = true; break;
            case UnlockType.Loan: isLoanUnlocked = true; break;
            case UnlockType.Stock: isStockUnlocked = true; break;
            case UnlockType.Bond: isBondUnlocked = true; break;
            case UnlockType.ISA: isISAUnlocked = true; break;
        }
        Debug.Log($"<color=cyan>[금융 해금] {type} 기능이 활성화되었습니다!</color>");
    }
}
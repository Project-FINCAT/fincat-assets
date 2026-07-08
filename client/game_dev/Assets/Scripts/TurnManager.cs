using UnityEngine;
using System;

// 게임의 턴(날짜) 시스템을 관리
// 침대에서 자면 턴이 넘어가고, 턴이 넘어갈 때마다 은행 이자 지급, 주가 변동 등 일어날 일들을 관리한다
public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    private int currentTurn = 0; // 현재 턴 (날짜)

    // 턴이 넘어갈 때 호출되는 이벤트
    public event Action<int> OnTurnChanged; // 턴이 변경됐을 때 (매개변수: 현재 턴)

    public event Action OnBankInterestApplied; // 은행 이자가 적용됐을 때
    public event Action OnStockPriceChanged; // 주가가 변동되도록 할 때

    private void Awake()
    {
        Instance = this;
    }

    // 턴 넘기기 (침대에서 자기)
    public void PassTurn()
    {
        currentTurn++;
        Debug.Log($"=== 턴 {currentTurn} 시작 ===");

        // 턴 변경 이벤트 발생
        OnTurnChanged?.Invoke(currentTurn);

        // 턴이 넘어갈 때 일어날 일들

        ApplyBankInterest(); // 은행 이자
        ChangeStockPrice(); // 주가 변동
        UpdateFinances(); // 자산 갱신 등
    }

    // 은행 이자 적용
    private void ApplyBankInterest()
    {

        Debug.Log("[은행] 이자가 입금되었습니다.");
        OnBankInterestApplied?.Invoke();
    }

    // 주가 변동
    private void ChangeStockPrice()
    {
        Debug.Log("[주식] 주가가 변동되었습니다.");
        OnStockPriceChanged?.Invoke();
    }

    // 자산 시스템 갱신 (향후 확장)
    private void UpdateFinances()
    {
        // 여기에 추가 자금 운용 로직이 들어갑니다
        // 예: 투자 수익률 계산, 대출금 이자, 부채 계산 등
    }

    // 현재 턴 반환
    public int GetCurrentTurn()
    {
        return currentTurn;
    }
}

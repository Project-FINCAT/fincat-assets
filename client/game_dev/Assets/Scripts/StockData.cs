using UnityEngine;


[CreateAssetMenu(fileName = "NewFinanceData", menuName = "Finance/Finance Data")]
public class StockData : ScriptableObject
{
    public string stockName;       // 종목명/채권명
    public int currentPrice;       // 현재가
    public int startPrice;         // 시작가 (게임 재시작 시 초기값으로 사용)
    
    [Header("채권 판단 및 이자/배당률")]
    public bool isBond;            // 체크하면 채권, 해제하면 주식
    public float interestRate;     // 채권의 이자율 또는 주식의 배당률
    public int durationTurns; //채권의 만기 기간 (턴 수)


    [TextArea]
    public string description;  // 주식 설명
}
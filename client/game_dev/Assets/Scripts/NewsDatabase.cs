using System.Collections.Generic;
using UnityEngine;

// 뉴스가 주식에 주는 영향을 정의하는 클래스
[System.Serializable]
public class NewsImpact
{
    public string targetStockName; // 영향을 줄 주식 이름 (예: "삼성증권")
    public float fluctuationRate;  // 변동률 (0.2는 +20%, -0.1은 -10%)
}

[System.Serializable]
public class NewsData
{
    public int turn;               // (기존) 특정 턴에 나오게 하고 싶을 때 사용
    public int newsID;             // (추가) 뉴스 식별 번호

    [TextArea(3, 10)]

    public string newsText;        // (기존) 뉴스 내용
    public List<NewsImpact> impacts; // (추가) 이 뉴스가 발생 시 변동될 주식들
}


[CreateAssetMenu(fileName = "NewNewsDatabase", menuName = "Stock System/News Database")]
public class NewsDatabase : ScriptableObject
{
    public List<NewsData> newsList = new List<NewsData>();

}
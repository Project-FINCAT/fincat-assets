using UnityEngine;
using System.Collections.Generic;

public class NewsManager : MonoBehaviour
{

    public static NewsManager Instance { get; private set; }
    public NewsDatabase database;

    // 이번 턴에 확정된 뉴스를 기억할 변수
    private NewsData currentNewsThisTurn;

    // 중복 방지용 HashSet
    private HashSet<int> usedNewsIDs = new HashSet<int>();

    // 외부에서 오늘 뉴스의 ID를 쉽게 알 수 있도록 프로퍼티 추가
    public int CurrentNewsID => currentNewsThisTurn != null ? currentNewsThisTurn.newsID : -1;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void SelectNewsForToday()
    {
        if (database == null || database.newsList.Count == 0) return;

        // 모든 뉴스를 사용했으면 초기화 (한 사이클 완료)
        if (usedNewsIDs.Count >= database.newsList.Count)
        {
            usedNewsIDs.Clear();
            Debug.Log("[NewsManager] 뉴스 사이클 초기화");
        }

        // 아직 사용 안 한 뉴스 중에서만 선택
        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, database.newsList.Count);
        } while (usedNewsIDs.Contains(database.newsList[randomIndex].newsID));

        currentNewsThisTurn = database.newsList[randomIndex];
        usedNewsIDs.Add(currentNewsThisTurn.newsID);
        
        Debug.Log($"[NewsManager] 오늘자로 선정된 뉴스: {currentNewsThisTurn.newsText} (남은 뉴스: {database.newsList.Count - usedNewsIDs.Count}개)");
    }

    // TV에서 호출할 함수: 이미 선정된 뉴스만 계속 반환함
    public string GetTodayNewsText()
    {
        if (currentNewsThisTurn == null)
        {
            // 혹시라도 선정이 안 되어 있다면 급하게 하나 선정
            SelectNewsForToday();
        }
        return currentNewsThisTurn.newsText;
    }

    public string GetNewsTextByID(int id)
    {
        if (currentNewsThisTurn != null && currentNewsThisTurn.newsID == id)
        {
            return currentNewsThisTurn.newsText;
        }
        return "";
    }

    // StockManager에서 주가 계산할 때 쓸 데이터 반환
    public NewsData GetTodayNewsData() => currentNewsThisTurn;

}
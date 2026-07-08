using UnityEngine;
using TMPro;

public class StockTabController : MonoBehaviour
{
    public GameObject stockView;
    public GameObject bondView;

    public TMP_Text infoMessageText; // 채권 퀘스트가 클리어 되지 않았을 때 보여줄 메시지 텍스트

    // 주식 탭 버튼에 연결할 함수
    public void ShowStockTab()
    {
        stockView.SetActive(true);
        bondView.SetActive(false);
        if(infoMessageText != null) infoMessageText.text = "주식 시장입니다.";
    }

    // 채권 탭 버튼에 연결할 함수
    public void ShowBondTab()
    {
        stockView.SetActive(false);
        bondView.SetActive(true);
        if(infoMessageText != null) infoMessageText.text = "채권 시장입니다.";
    }
}
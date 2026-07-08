using UnityEngine;

public class PopupItem : MonoBehaviour
{
    public GameObject popup;
    public GameObject panel;

    private bool isOpen = false;

    // 현재 열려있는 팝업 저장
    private static PopupItem currentOpenPopup;

    public void OnClickPoPUP()
    {
        // 다른 팝업이 열려있으면 닫기
        if (currentOpenPopup != null && currentOpenPopup != this)
        {
            currentOpenPopup.ClosePopup();
        }

        isOpen = !isOpen;
        popup.SetActive(isOpen);

        if (isOpen)
        {
            currentOpenPopup = this;
            transform.SetAsLastSibling();
        }
        else
        {
            currentOpenPopup = null;
        }
    }

    public void ClosePopup()
    {
        isOpen = false;
        popup.SetActive(false);
    }
}
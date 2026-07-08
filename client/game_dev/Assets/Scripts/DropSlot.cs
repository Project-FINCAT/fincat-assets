using UnityEngine;
using UnityEngine.EventSystems;

public class DropSlot : MonoBehaviour, IDropHandler
{
    public string correctCategory;
    public bool isCorrect;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dropped = eventData.pointerDrag;

        if (dropped == null) return;

        DraggableCard card = dropped.GetComponent<DraggableCard>();
        RectTransform rect = dropped.GetComponent<RectTransform>();

        if (card.category == correctCategory)
        {
            isCorrect = true;

            dropped.transform.SetParent(transform);

            // 슬롯 중앙 정렬
            rect.anchoredPosition = Vector2.zero;

            GameManager.instance.CheckAllCorrect();
        }
        else
        {
            // 원래 자리로 정확히 복귀
            dropped.transform.SetParent(card.originalParent);

            // 저장된 위치로 복귀
            rect.anchoredPosition = card.GetOriginalPosition();

            // 흔들림 효과
            ShakeEffect shake = dropped.GetComponent<ShakeEffect>();
            if (shake != null)
            {
                StartCoroutine(shake.Shake());
            }
        }
    }
}
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableCard : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Transform originalParent;

    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;

    private Vector2 originalPosition;

    public string category;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;

        // 위치 저장
        originalPosition = rectTransform.anchoredPosition;

        transform.SetParent(transform.root);
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;

        // 드롭 실패 시
        if (transform.parent == transform.root)
        {
            transform.SetParent(originalParent);

            // 원래 위치로 정확히 복귀
            rectTransform.anchoredPosition = originalPosition;
        }
    }

    public Vector2 GetOriginalPosition()
    {
        return originalPosition;
    }
}
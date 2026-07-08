using UnityEngine;

public class AccordionItem : MonoBehaviour
{
    [SerializeField] private GameObject detailPanel;

    private bool isOpen = false;

    public void OnClickHeader()
    {
        isOpen = !isOpen;
        detailPanel.SetActive(isOpen);
    }
}
using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    public Image[] tabImages;
    public GameObject[] pages;
    public int CurrentTab { get; private set; }
    void Start()
    {
        ActivateTab(0);
    }

    public void ActivateTab(int tabNo)
    {
        CurrentTab = tabNo;
        for(int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
            tabImages[i].color = Color.grey;

        }
        pages[tabNo].SetActive(true);
        tabImages[tabNo].color = Color.white;
    }
}

using UnityEngine;
using TMPro;

public class QuestEntryUI : MonoBehaviour
{
    public TextMeshProUGUI questNameText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI questPlaceText;
    public GameObject completeIcon;

    private Quest data;
    
    public void Setup(Quest quest)
    {
        data = quest;

        questNameText.text = data.title;
        descriptionText.text = data.description;
        questPlaceText.text = "퀘스트 장소:" + data.questPlace;
        completeIcon.SetActive(data.isCompleted);
    }

    public void Refresh()
    {
        if (data == null) return;

        completeIcon.SetActive(data.isCompleted);
    }
}
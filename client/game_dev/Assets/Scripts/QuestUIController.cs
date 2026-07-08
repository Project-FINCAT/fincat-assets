using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class QuestUIController : MonoBehaviour
{
    public Transform contentParent;     // 퀘스트 항목이 생성될 Content 부모 오브젝트
    public GameObject questEntryPrefab; // 앞에서 만든 퀘스트 한 줄 UI 프리팹
    public List<QuestData> allQuests;   // 게임의 모든 QuestData 등록

    public void RefreshQuestList()
    {
        // 1. 기존 리스트 UI 삭제
        foreach (Transform child in contentParent)
        {
            Destroy(child.gameObject);
        }

        // 2. 수락되었거나 완료된 퀘스트만 표시
        foreach (QuestData quest in allQuests)
        {
            if (quest.isAccepted || quest.isCompleted)
            {
                GameObject entry = Instantiate(questEntryPrefab, contentParent);
                TMP_Text text = entry.GetComponentInChildren<TMP_Text>();
                
                string status = quest.isCompleted ? "<color=green>[완료]</color>" : "<color=yellow>[진행중]</color>";
                text.text = $"{status} {quest.questName}\n<size=80%>{quest.description}</size>";
            }
        }
    }
}
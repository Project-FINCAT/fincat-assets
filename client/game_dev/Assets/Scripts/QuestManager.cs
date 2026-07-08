using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public List<Quest> quests = new List<Quest>(); // QuestData → Quest로 변경

    public event Action OnQuestUpdated;

    private void Awake()
    {
        Instance = this;

        foreach (var quest in quests)
        {
            quest.isCompleted = false;
        }
    }

    public void CompleteQuest(string id)
    {
        var quest = quests.Find(q => q.questID == id);

        if (quest == null)
        {
            Debug.LogWarning("Quest not found: " + id);
            return;
        }

        quest.isCompleted = true;

        OnQuestUpdated?.Invoke();
    }
}
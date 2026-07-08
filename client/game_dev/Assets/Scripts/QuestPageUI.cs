using System.Collections.Generic;
using UnityEngine;

public class QuestPageUI : MonoBehaviour
{
    public Transform content;               // ScrollView Content
    public QuestEntryUI questEntryPrefab;   // Prefab

    private List<QuestEntryUI> entries = new List<QuestEntryUI>();

    private void OnEnable()
    {
        QuestManager.Instance.OnQuestUpdated += RefreshUI;
        RefreshUI();
    }

    private void OnDisable()
    {
        QuestManager.Instance.OnQuestUpdated -= RefreshUI;
    }

    void RefreshUI()
    {
        // 기존 UI 삭제
        foreach (var e in entries)
            Destroy(e.gameObject);

        entries.Clear();

        // 새로 생성
        foreach (var quest in QuestManager.Instance.quests)
        {
            var entry = Instantiate(questEntryPrefab, content);
            entry.Setup(quest);

            entries.Add(entry);
        }
    }
}
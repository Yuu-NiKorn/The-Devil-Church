using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance { get; private set; }

    public event Action<QuestDataSO> OnQuestCompleted;

    private readonly HashSet<QuestDataSO> completedQuests = new();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void CompleteQuest(QuestDataSO quest)
    {
        if (quest == null)
            return;

        if (!completedQuests.Add(quest))
            return;

        Debug.Log("Quête validée : " + quest.questName);

        OnQuestCompleted?.Invoke(quest);
    }

    public bool IsQuestCompleted(QuestDataSO quest)
    {
        return quest != null && completedQuests.Contains(quest);
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
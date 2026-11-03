using UnityEngine;

public class QuestEventActivator : MonoBehaviour
{
    public enum EventAction
    {
        UnlockDoor,
        OpenDoor,
        ActivateObject
    }

    [Header("Required Quest")]
    [SerializeField] private QuestDataSO requiredQuest;

    [Header("Action")]
    [SerializeField] private EventAction eventAction;

    [Header("Door")]
    [SerializeField] private DoorController targetDoor;

    [Header("Object")]
    [SerializeField] private GameObject targetObject;

    private bool triggered;

    private void Start()
    {
        if (QuestManager.Instance == null)
        {
            Debug.LogWarning(
                "QuestManager introuvable.", this
            );
            return;
        }

        QuestManager.Instance.OnQuestCompleted += HandleQuestCompleted;

        if (QuestManager.Instance.IsQuestCompleted(requiredQuest))
            TriggerEvent();
    }

    private void HandleQuestCompleted(QuestDataSO completedQuest)
    {
        if (completedQuest != requiredQuest)
            return;

        TriggerEvent();
    }

    private void TriggerEvent()
    {
        if (triggered)
            return;

        triggered = true;

        switch (eventAction)
        {
            case EventAction.UnlockDoor:
                if (targetDoor != null)
                    targetDoor.Unlock();
                break;

            case EventAction.OpenDoor:
                if (targetDoor != null)
                    targetDoor.Open();
                break;

            case EventAction.ActivateObject:
                if (targetObject != null)
                    targetObject.SetActive(true);
                break;
        }

        Debug.Log(
            "Événement de quête déclenché : " + eventAction
        );
    }

    private void OnDestroy()
    {
        if (QuestManager.Instance != null)
        {
            QuestManager.Instance.OnQuestCompleted -=
                HandleQuestCompleted;
        }
    }
}
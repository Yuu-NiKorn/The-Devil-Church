
using System.Collections;
using UnityEngine;

public class NPCQuestController : MonoBehaviour
{
    private enum QuestState
    {
        NotStarted,
        Active,
        Completed
    }

    [Header("Quest")]
    [SerializeField] private QuestDataSO quest;

    [Header("References")]
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private PlayerInventory inventory;

    [Header("Completion")]
    [SerializeField] private GameObject npcVisual;
    [SerializeField] private float disappearanceDelay = 1f;

    private QuestState state = QuestState.NotStarted;
    private bool interactionInProgress;

    public bool CanInteract =>
        !interactionInProgress &&
        state != QuestState.Completed;

    public void Interact()
    {
        if (!CanInteract ||
            quest == null ||
            dialogueManager == null ||
            inventory == null ||
            dialogueManager.IsDialogueActive)
            return;

        DialogueDataSO dialogue = null;

        switch (state)
        {
            case QuestState.NotStarted:
                dialogue = quest.firstDialogue;
                break;

            case QuestState.Active:
                dialogue = HasRequiredItem()
                    ? quest.completionDialogue
                    : quest.reminderDialogue;
                break;
        }

        if (dialogue == null)
        {
            Debug.LogWarning(
                "Dialogue manquant pour la quête : " +
                quest.questName
            );
            return;
        }

        interactionInProgress = true;

        dialogueManager.OnDialogueFinished += HandleDialogueFinished;
        dialogueManager.StartDialogue(dialogue);
    }

    private void HandleDialogueFinished()
    {
        dialogueManager.OnDialogueFinished -= HandleDialogueFinished;

        interactionInProgress = false;

        if (state == QuestState.NotStarted)
        {
            state = QuestState.Active;

            Debug.Log("Quête commencée : " + quest.questName);
            return;
        }

        if (state == QuestState.Active && HasRequiredItem())
        {
            CompleteQuest();
        }
    }

    private bool HasRequiredItem()
    {
        return quest.requiredItem != null &&
               inventory.GetQuantity(quest.requiredItem) >=
               quest.requiredQuantity;
    }

    private void CompleteQuest()
    {
        if (!inventory.RemoveItem(
            quest.requiredItem,
            quest.requiredQuantity))
            return;

        state = QuestState.Completed;

        Debug.Log("Quête terminée : " + quest.questName);

        StartCoroutine(Disappear());
    }

    private IEnumerator Disappear()
    {
        yield return new WaitForSeconds(disappearanceDelay);

        if (npcVisual != null)
            npcVisual.SetActive(false);

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        if (dialogueManager != null)
            dialogueManager.OnDialogueFinished -= HandleDialogueFinished;
    }
}

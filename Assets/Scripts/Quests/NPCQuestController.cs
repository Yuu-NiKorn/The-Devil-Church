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

    private enum DialogueType
    {
        None,
        First,
        Reminder,
        Completion
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
    private DialogueType currentDialogueType = DialogueType.None;
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

        if (state == QuestState.NotStarted)
        {
            currentDialogueType = DialogueType.First;
            dialogue = quest.firstDialogue;
        }
        else if (state == QuestState.Active)
        {
            if (HasRequiredItem())
            {
                currentDialogueType = DialogueType.Completion;
                dialogue = quest.completionDialogue;
            }
            else
            {
                currentDialogueType = DialogueType.Reminder;
                dialogue = quest.reminderDialogue;
            }
        }

        if (dialogue == null ||
            dialogue.lines == null ||
            dialogue.lines.Count == 0)
        {
            currentDialogueType = DialogueType.None;
            Debug.LogWarning(
                "Dialogue manquant pour : " + quest.questName
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

        DialogueType finishedType = currentDialogueType;
        currentDialogueType = DialogueType.None;

        switch (finishedType)
        {
            case DialogueType.First:
                state = QuestState.Active;
                Debug.Log("Quête commencée : " + quest.questName);
                break;

            case DialogueType.Completion:
                CompleteQuest();
                break;
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
        if (!HasRequiredItem())
            return;

        if (!inventory.RemoveItem(
            quest.requiredItem,
            quest.requiredQuantity))
            return;

        state = QuestState.Completed;

        if (QuestManager.Instance != null)
            QuestManager.Instance.CompleteQuest(quest);
        else
            Debug.LogWarning(
                "QuestManager absent de la scène."
            );

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

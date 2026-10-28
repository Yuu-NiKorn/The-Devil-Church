using UnityEngine;

[CreateAssetMenu(
    fileName = "NewQuest",
    menuName = "The Devil Church/Quests/Quest"
)]
public class QuestDataSO : ScriptableObject
{
    [Header("Quest Information")]
    public string questID;
    public string questName;

    [TextArea(3, 8)]
    public string description;

    [Header("Required Item")]
    public ItemDataSO requiredItem;

    [Min(1)]
    public int requiredQuantity = 1;

    [Header("Dialogues")]
    public DialogueDataSO firstDialogue;
    public DialogueDataSO reminderDialogue;
    public DialogueDataSO completionDialogue;
}
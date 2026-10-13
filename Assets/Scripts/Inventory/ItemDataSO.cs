using UnityEngine;

public enum ItemCategory
{
    Consumable,
    Candle,
    QuestItem,
    Note
}

[CreateAssetMenu(
    fileName = "NewItem",
    menuName = "The Devil Church/Inventory/Item"
)]
public class ItemDataSO : ScriptableObject
{
    [Header("Identity")]
    public string itemID;
    public string itemName;

    [TextArea(3, 8)]
    public string description;

    public Sprite icon;

    [Header("Category")]
    public ItemCategory category;

    [Header("Inventory")]
    [Min(1)]
    public int maxStack = 1;

    [Header("Consumable Effect")]
    [Min(0)]
    public float sanityRestoreAmount = 0f;

    [Header("Note Data")]
    public NoteDataSO noteData;
}
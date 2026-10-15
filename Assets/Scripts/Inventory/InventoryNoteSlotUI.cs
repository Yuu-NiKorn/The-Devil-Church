using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryNoteSlotUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text noteTitle;
    [SerializeField] private GameObject selectionHighlight;

    private ItemDataSO item;

    public ItemDataSO Item => item;

    public void Setup(
        ItemDataSO itemData,
        Action<ItemDataSO> onSelected)
    {
        item = itemData;

        if (noteTitle != null)
        {
            noteTitle.text =
                item.noteData != null
                    ? item.noteData.noteTitle
                    : item.itemName;
        }

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(
                () => onSelected?.Invoke(item)
            );
        }

        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        if (selectionHighlight != null)
            selectionHighlight.SetActive(selected);
    }
}
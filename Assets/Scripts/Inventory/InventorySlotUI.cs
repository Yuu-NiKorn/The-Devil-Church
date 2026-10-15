using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text quantityText;
    [SerializeField] private GameObject selectionHighlight;

    private ItemDataSO item;
    
    public ItemDataSO Item => item;

    public void Setup(
        ItemDataSO itemData,
        int quantity,
        Action<ItemDataSO> onSelected)
    {
        item = itemData;

        icon.sprite = item.icon;
        icon.enabled = item.icon != null;

        quantityText.text = quantity + "/" + item.maxStack;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(
            () => onSelected?.Invoke(item)
        );

        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        if (selectionHighlight != null)
            selectionHighlight.SetActive(selected);
    }
}
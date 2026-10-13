using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryEntry
{
    public ItemDataSO item;
    public int quantity;

    public InventoryEntry(ItemDataSO item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }
}

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private List<InventoryEntry> items = new();

    public IReadOnlyList<InventoryEntry> Items => items;

    public event Action OnInventoryChanged;

    public bool AddItem(ItemDataSO item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        InventoryEntry entry = items.Find(
            x => x.item == item
        );

        int maxStack = Mathf.Max(1, item.maxStack);

        if (entry != null)
        {
            if (entry.quantity + amount > maxStack)
                return false;

            entry.quantity += amount;
        }
        else
        {
            if (amount > maxStack)
                return false;

            items.Add(new InventoryEntry(item, amount));
        }

        OnInventoryChanged?.Invoke();
        Debug.Log("Objet récupéré : " + item.itemName);
        return true;
    }

    public bool RemoveItem(ItemDataSO item, int amount = 1)
    {
        if (item == null || amount <= 0)
            return false;

        InventoryEntry entry = items.Find(
            x => x.item == item
        );

        if (entry == null || entry.quantity < amount)
            return false;

        entry.quantity -= amount;

        if (entry.quantity <= 0)
            items.Remove(entry);

        OnInventoryChanged?.Invoke();
        return true;
    }

    public int GetQuantity(ItemDataSO item)
    {
        InventoryEntry entry = items.Find(
            x => x.item == item
        );

        return entry != null ? entry.quantity : 0;
    }
}
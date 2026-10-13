using UnityEngine;

public class InventoryItemActions : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerSanity playerSanity;

    public bool UseItem(ItemDataSO item)
    {
        if (item == null || inventory == null || playerSanity == null)
            return false;

        if (item.category != ItemCategory.Consumable)
            return false;

        if (inventory.GetQuantity(item) <= 0)
            return false;

        if (playerSanity.IsDead)
            return false;

        if (playerSanity.CurrentSanity >= playerSanity.MaxSanity)
        {
            Debug.Log("La santé mentale est déjà au maximum.");
            return false;
        }

        if (item.sanityRestoreAmount <= 0f)
            return false;

        playerSanity.RestoreSanityAmount(item.sanityRestoreAmount);

        inventory.RemoveItem(item, 1);

        Debug.Log("Objet utilisé : " + item.itemName);
        return true;
    }
}
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private ItemDataSO itemData;
    [SerializeField, Min(1)] private int quantity = 1;

    [Header("Interaction")]
    [SerializeField] private Transform interactionPoint;

    public ItemDataSO ItemData => itemData;

    public Vector3 InteractionPosition =>
        interactionPoint != null
            ? interactionPoint.position
            : transform.position;

    public bool TryPickup(PlayerInventory inventory)
    {
        if (inventory == null || itemData == null)
            return false;

        if (!inventory.AddItem(itemData, quantity))
            return false;

        Destroy(gameObject);
        return true;
    }
}
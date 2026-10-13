using UnityEngine;

public class InventoryCandleReplacement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerCandle playerCandle;
    [SerializeField] private ItemDataSO candleItem;

    private void Awake()
    {
        if (inventory == null)
            inventory = GetComponent<PlayerInventory>();
    }

    private void Start()
    {
        if (playerCandle != null)
            playerCandle.OnCandleBurnedOut += HandleCandleBurnedOut;

        if (inventory != null)
            inventory.OnInventoryChanged += HandleInventoryChanged;
    }

    private void OnDestroy()
    {
        if (playerCandle != null)
            playerCandle.OnCandleBurnedOut -= HandleCandleBurnedOut;

        if (inventory != null)
            inventory.OnInventoryChanged -= HandleInventoryChanged;
    }

    private void HandleCandleBurnedOut()
    {
        TryReplaceCandle();
    }

    private void HandleInventoryChanged()
    {
        // Si le joueur ramasse une bougie alors que
        // celle dans sa main est épuisée.
        if (playerCandle != null && playerCandle.IsBurnedOut)
            TryReplaceCandle();
    }

    private void TryReplaceCandle()
    {
        if (inventory == null ||
            playerCandle == null ||
            candleItem == null)
            return;

        if (candleItem.category != ItemCategory.Candle)
            return;

        if (!playerCandle.IsBurnedOut)
            return;

        if (inventory.GetQuantity(candleItem) <= 0)
        {
            Debug.Log("Plus de bougies de réserve !");
            return;
        }

        if (!inventory.RemoveItem(candleItem, 1))
            return;

        playerCandle.ReplaceCandle();

        Debug.Log("Bougie remplacée automatiquement !");
    }
}
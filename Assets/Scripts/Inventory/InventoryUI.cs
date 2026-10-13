using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    private enum InventoryPage
    {
        Consumables,
        QuestItems,
        Notes
    }

    [Header("References")]
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerInteractor playerInteractor;

    [Header("Book")]
    [SerializeField] private CanvasGroup bookCanvasGroup;
    [SerializeField] private Transform itemGrid;
    [SerializeField] private Transform notesList;
    [SerializeField] private InventorySlotUI slotPrefab;

    [Header("Right Page")]
    [SerializeField] private TMP_Text itemTitle;
    [SerializeField] private TMP_Text itemDescription;
    [SerializeField] private Button useButton;

    [Header("Navigation")]
    [SerializeField] private Button previousPageButton;
    [SerializeField] private Button nextPageButton;

    [Header("Input")]
    [SerializeField] private KeyCode inventoryKey = KeyCode.Tab;
    
    [Header("Item Actions")]
    [SerializeField] private InventoryItemActions itemActions;

    private InventoryPage currentPage;
    private ItemDataSO selectedItem;
    private bool isOpen;

    private readonly List<InventorySlotUI> slots = new();

    private void Start()
    {
        CloseInventory();

        if (inventory != null)
            inventory.OnInventoryChanged += RefreshInventory;

        previousPageButton.onClick.AddListener(PreviousPage);
        nextPageButton.onClick.AddListener(NextPage);
        useButton.onClick.AddListener(UseSelectedItem);
    }

    private void OnDestroy()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= RefreshInventory;
    }

    private void Update()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            if (isOpen)
                CloseInventory();
            else
                OpenInventory();
        }
    }

    private void OpenInventory()
    {
        isOpen = true;
        currentPage = InventoryPage.Consumables;

        bookCanvasGroup.alpha = 1f;
        bookCanvasGroup.interactable = true;
        bookCanvasGroup.blocksRaycasts = true;

        SetPlayerControls(false);
        RefreshInventory();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void CloseInventory()
    {
        isOpen = false;

        bookCanvasGroup.alpha = 0f;
        bookCanvasGroup.interactable = false;
        bookCanvasGroup.blocksRaycasts = false;

        SetPlayerControls(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void SetPlayerControls(bool enabled)
    {
        if (playerMovement != null)
            playerMovement.enabled = enabled;

        if (playerLook != null)
            playerLook.enabled = enabled;

        if (playerInteractor != null)
            playerInteractor.enabled = enabled;
    }

    private void RefreshInventory()
    {
        if (!isOpen || inventory == null)
            return;

        foreach (InventorySlotUI slot in slots)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }

        slots.Clear();
        selectedItem = null;

        foreach (InventoryEntry entry in inventory.Items)
        {
            if (!BelongsToCurrentPage(entry.item))
                continue;

            InventorySlotUI slot = Instantiate(
                slotPrefab,
                currentPage == InventoryPage.Notes
                    ? notesList
                    : itemGrid
            );

            slot.Setup(
                entry.item,
                entry.quantity,
                SelectItem
            );

            slots.Add(slot);
        }

        itemGrid.gameObject.SetActive(
            currentPage != InventoryPage.Notes
        );

        notesList.gameObject.SetActive(
            currentPage == InventoryPage.Notes
        );

        previousPageButton.interactable =
            currentPage != InventoryPage.Consumables;

        nextPageButton.interactable =
            currentPage != InventoryPage.Notes;

        ClearSelection();
    }

    private bool BelongsToCurrentPage(ItemDataSO item)
    {
        if (item == null)
            return false;

        switch (currentPage)
        {
            case InventoryPage.Consumables:
                return item.category == ItemCategory.Consumable ||
                       item.category == ItemCategory.Candle;

            case InventoryPage.QuestItems:
                return item.category == ItemCategory.QuestItem;

            case InventoryPage.Notes:
                return item.category == ItemCategory.Note;
        }

        return false;
    }

    private void SelectItem(ItemDataSO item)
    {
        selectedItem = item;

        foreach (InventorySlotUI slot in slots)
            slot.SetSelected(false);

        itemTitle.text = item.itemName;

        if (item.category == ItemCategory.Note &&
            item.noteData != null)
        {
            itemDescription.text = item.noteData.noteText;
        }
        else
        {
            itemDescription.text = item.description;
        }

        useButton.gameObject.SetActive(
            item.category == ItemCategory.Consumable
        );
    }

    private void ClearSelection()
    {
        itemTitle.text = "";
        itemDescription.text = "";
        useButton.gameObject.SetActive(false);
    }

    private void PreviousPage()
    {
        if (currentPage == InventoryPage.Consumables)
            return;

        currentPage--;
        RefreshInventory();
    }

    private void NextPage()
    {
        if (currentPage == InventoryPage.Notes)
            return;

        currentPage++;
        RefreshInventory();
    }
    
    private void UseSelectedItem()
    {
        if (selectedItem == null || itemActions == null)
            return;

        if (itemActions.UseItem(selectedItem))
        {
            RefreshInventory();
        }
    }
}

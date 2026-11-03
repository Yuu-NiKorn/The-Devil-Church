using UnityEngine;
using TMPro;

public class PickupPromptUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text keyText;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private Camera playerCamera;

    [Header("Position")]
    [SerializeField] private Vector3 worldOffset =
        new Vector3(0f, 0.45f, 0f);

    [Header("Interaction Text")]
    [SerializeField] private string pickupText = "[E] Récupérer";
    [SerializeField] private string talkText = "[E] Parler";
    [SerializeField] private string openText = "[E] Ouvrir";
    [SerializeField] private string closeText = "[E] Fermer";
    [SerializeField] private string lockedText = "[E] Verrouillée";

    private ItemPickup targetPickup;
    private NPCInteraction targetNPC;
    private DoorController targetDoor;

    private void Awake()
    {
        if (canvasGroup == null)
            canvasGroup = GetComponentInChildren<CanvasGroup>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        Hide();
    }

    private void LateUpdate()
    {
        if (playerCamera == null)
            return;

        Vector3 position;

        if (targetPickup != null)
            position = targetPickup.InteractionPosition;
        else if (targetNPC != null)
            position = targetNPC.InteractionPosition;
        else if (targetDoor != null)
            position = targetDoor.InteractionPosition;
        else
            return;

        transform.position = position + worldOffset;
        transform.rotation = playerCamera.transform.rotation;
    }

    public void Show(ItemPickup pickup)
    {
        if (pickup == null || pickup.ItemData == null)
            return;

        targetPickup = pickup;
        targetNPC = null;
        targetDoor = null;

        SetText(pickupText, pickup.ItemData.itemName);
    }

    public void Show(NPCInteraction npc)
    {
        if (npc == null)
            return;

        targetPickup = null;
        targetNPC = npc;
        targetDoor = null;

        SetText(talkText, "");
    }

    public void Show(DoorController door, PlayerInventory inventory)
    {
        if (door == null)
            return;

        targetPickup = null;
        targetNPC = null;
        targetDoor = door;

        string text;

        if (door.IsLocked && !door.CanOpen(inventory))
            text = lockedText;
        else
            text = door.IsOpen ? closeText : openText;

        SetText(text, "");
    }

    private void SetText(string action, string itemName)
    {
        if (keyText != null)
            keyText.text = action;

        if (itemNameText != null)
            itemNameText.text = itemName;

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        targetPickup = null;
        targetNPC = null;
        targetDoor = null;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }
}
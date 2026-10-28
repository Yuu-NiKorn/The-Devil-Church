
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

    private ItemPickup targetPickup;
    private NPCInteraction targetNPC;

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

        if (targetPickup != null)
        {
            transform.position =
                targetPickup.InteractionPosition + worldOffset;
        }
        else if (targetNPC != null)
        {
            transform.position =
                targetNPC.InteractionPosition + worldOffset;
        }
        else
        {
            return;
        }

        transform.rotation =
            playerCamera.transform.rotation;
    }

    public void Show(ItemPickup pickup)
    {
        if (pickup == null || pickup.ItemData == null)
            return;

        targetPickup = pickup;
        targetNPC = null;

        if (keyText != null)
            keyText.text = pickupText;

        if (itemNameText != null)
            itemNameText.text = pickup.ItemData.itemName;

        ShowCanvas();
    }

    public void Show(NPCInteraction npc)
    {
        if (npc == null)
            return;

        targetNPC = npc;
        targetPickup = null;

        if (keyText != null)
            keyText.text = talkText;

        if (itemNameText != null)
            itemNameText.text = "";

        ShowCanvas();
    }

    private void ShowCanvas()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        targetPickup = null;
        targetNPC = null;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }
}

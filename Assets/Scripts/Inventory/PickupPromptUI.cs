using UnityEngine;
using TMPro;

public class PickupPromptUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private Camera playerCamera;

    [Header("Position")]
    [SerializeField] private Vector3 worldOffset =
        new Vector3(0f, 0.45f, 0f);

    private ItemPickup target;

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
        if (target == null || playerCamera == null)
            return;

        transform.position =
            target.InteractionPosition + worldOffset;

        transform.rotation =
            playerCamera.transform.rotation;
    }

    public void Show(ItemPickup pickup)
    {
        if (pickup == null)
            return;

        if (target != pickup)
        {
            target = pickup;

            if (itemNameText != null)
                itemNameText.text = pickup.ItemData.itemName;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        target = null;

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }
}
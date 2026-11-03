using System.Collections;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private Transform doorPivot;
    [SerializeField] private float openAngle = 90f;
    [SerializeField, Min(0.01f)]
    private float animationDuration = 1f;
    [SerializeField] private bool startsOpen;

    [Header("Lock")]
    [SerializeField] private bool startsLocked;
    [SerializeField] private ItemDataSO requiredItem;
    [SerializeField] private bool consumeRequiredItem;

    [Header("Interaction")]
    [SerializeField] private Transform interactionPoint;

    private Quaternion closedRotation;
    private Quaternion openedRotation;
    private Coroutine animationCoroutine;

    private bool isOpen;
    private bool isLocked;

    public bool IsOpen => isOpen;
    public bool IsLocked => isLocked;

    public Vector3 InteractionPosition =>
        interactionPoint != null
            ? interactionPoint.position
            : transform.position;

    private void Awake()
    {
        if (doorPivot == null)
            doorPivot = transform;

        closedRotation = doorPivot.localRotation;
        openedRotation = closedRotation *
            Quaternion.Euler(0f, 0f, -openAngle);

        isOpen = startsOpen;
        isLocked = startsLocked;

        doorPivot.localRotation =
            isOpen ? openedRotation : closedRotation;
    }

    public void Interact(PlayerInventory inventory)
    {
        if (isLocked)
        {
            if (requiredItem == null ||
                inventory == null ||
                inventory.GetQuantity(requiredItem) < 1)
            {
                Debug.Log("Cette porte est verrouillée.");
                return;
            }

            if (consumeRequiredItem &&
                !inventory.RemoveItem(requiredItem, 1))
                return;

            isLocked = false;
        }

        if (isOpen)
            Close();
        else
            Open();
    }

    public void Open()
    {
        isLocked = false;
        AnimateDoor(true);
    }

    public void Close()
    {
        AnimateDoor(false);
    }

    public void Unlock()
    {
        isLocked = false;
    }

    public void Lock()
    {
        if (!isOpen)
            isLocked = true;
    }

    public bool CanOpen(PlayerInventory inventory)
    {
        if (!isLocked)
            return true;

        return requiredItem != null &&
               inventory != null &&
               inventory.GetQuantity(requiredItem) > 0;
    }

    private void AnimateDoor(bool open)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        isOpen = open;

        animationCoroutine = StartCoroutine(
            RotateDoor(open ? openedRotation : closedRotation)
        );
    }

    private IEnumerator RotateDoor(Quaternion target)
    {
        Quaternion start = doorPivot.localRotation;
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / animationDuration
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            doorPivot.localRotation =
                Quaternion.Slerp(start, target, t);

            yield return null;
        }

        doorPivot.localRotation = target;
        animationCoroutine = null;
    }
}
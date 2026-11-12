using UnityEngine;

[RequireComponent(typeof(PlayerInventory))]
public class PlayerInteractor : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private KeyCode interactionKey = KeyCode.E;
    [SerializeField] private float interactionRadius = 2.5f;
    [SerializeField] private LayerMask interactableLayers = ~0;

    [Header("References")]
    [SerializeField] private PlayerInventory inventory;
    [SerializeField] private PickupPromptUI pickupPrompt;

    private readonly Collider[] results = new Collider[64];

    private ItemPickup currentPickup;
    private NPCInteraction currentNPC;
    private DoorController currentDoor;

    private void Awake()
    {
        if (inventory == null)
            inventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        FindClosestInteractable();
        UpdateInteractionUI();

        if (!Input.GetKeyDown(interactionKey))
            return;

        if (currentNPC != null)
        {
            NPCInteraction npc = currentNPC;
            ClearTargets();
            HidePrompt();
            npc.Interact();
        }
        else if (currentDoor != null)
        {
            currentDoor.Interact(inventory);
            HidePrompt();
        }
        else if (currentPickup != null)
        {
            if (currentPickup.TryPickup(inventory))
            {
                ClearTargets();
                HidePrompt();
            }
        }
    }

    private void UpdateInteractionUI()
    {
        if (pickupPrompt == null)
            return;

        if (currentNPC != null)
            pickupPrompt.Show(currentNPC);
        else if (currentDoor != null && currentDoor.CanInteract)
            pickupPrompt.Show(currentDoor, inventory);
        else if (currentPickup != null)
            pickupPrompt.Show(currentPickup);
        else
            pickupPrompt.Hide();
    }

    private void FindClosestInteractable()
    {
        ClearTargets();

        float closestDistance = interactionRadius;

        int count = Physics.OverlapSphereNonAlloc(
            transform.position,
            interactionRadius,
            results,
            interactableLayers,
            QueryTriggerInteraction.Collide
        );

        for (int i = 0; i < count; i++)
        {
            Collider hit = results[i];

            if (hit == null)
                continue;

            NPCInteraction npc =
                hit.GetComponentInParent<NPCInteraction>();

            if (npc != null)
            {
                if (!npc.CanInteract)
                    continue;

                float distance = Vector3.Distance(
                    transform.position,
                    npc.InteractionPosition
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    ClearTargets();
                    currentNPC = npc;
                }

                continue;
            }

            DoorController door =
                hit.GetComponentInParent<DoorController>();

            if (door != null)
            {
                if (!door.CanInteract)
                    continue;

                float distance = Vector3.Distance(
                    transform.position,
                    door.InteractionPosition
                );

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    ClearTargets();
                    currentDoor = door;
                }

                continue;
            }

            ItemPickup pickup =
                hit.GetComponentInParent<ItemPickup>();

            if (pickup == null || pickup.ItemData == null)
                continue;

            float pickupDistance = Vector3.Distance(
                transform.position,
                pickup.InteractionPosition
            );

            if (pickupDistance < closestDistance)
            {
                closestDistance = pickupDistance;
                ClearTargets();
                currentPickup = pickup;
            }
        }
    }

    private void ClearTargets()
    {
        currentPickup = null;
        currentNPC = null;
        currentDoor = null;
    }

    private void HidePrompt()
    {
        if (pickupPrompt != null)
            pickupPrompt.Hide();
    }

    private void OnDisable()
    {
        ClearTargets();
        HidePrompt();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(
            transform.position,
            interactionRadius
        );
    }
}
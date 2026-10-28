
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

    private readonly Collider[] results = new Collider[32];

    private ItemPickup currentPickup;
    private NPCInteraction currentNPC;

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

            currentNPC = null;
            currentPickup = null;

            if (pickupPrompt != null)
                pickupPrompt.Hide();

            npc.Interact();
        }
        else if (currentPickup != null)
        {
            if (currentPickup.TryPickup(inventory))
            {
                currentPickup = null;

                if (pickupPrompt != null)
                    pickupPrompt.Hide();
            }
        }
    }

    private void UpdateInteractionUI()
    {
        if (pickupPrompt == null)
            return;

        if (currentNPC != null)
        {
            pickupPrompt.Show(currentNPC);
        }
        else if (currentPickup != null)
        {
            pickupPrompt.Show(currentPickup);
        }
        else
        {
            pickupPrompt.Hide();
        }
    }

    private void FindClosestInteractable()
    {
        currentPickup = null;
        currentNPC = null;

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

            // Recherche d'un PNJ
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
                    currentNPC = npc;
                    currentPickup = null;
                }

                continue;
            }

            // Recherche d'un objet
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
                currentPickup = pickup;
                currentNPC = null;
            }
        }
    }

    private void OnDisable()
    {
        currentPickup = null;
        currentNPC = null;

        if (pickupPrompt != null)
            pickupPrompt.Hide();
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

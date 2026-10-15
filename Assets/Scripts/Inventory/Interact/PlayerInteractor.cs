
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

    private void Awake()
    {
        if (inventory == null)
            inventory = GetComponent<PlayerInventory>();
    }

    private void Update()
    {
        FindClosestPickup();

        if (pickupPrompt != null)
        {
            if (currentPickup != null)
                pickupPrompt.Show(currentPickup);
            else
                pickupPrompt.Hide();
        }

        if (currentPickup != null &&
            Input.GetKeyDown(interactionKey))
        {
            if (currentPickup.TryPickup(inventory))
            {
                currentPickup = null;

                if (pickupPrompt != null)
                    pickupPrompt.Hide();
            }
        }
    }

    private void FindClosestPickup()
    {
        currentPickup = null;
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
            ItemPickup pickup =
                results[i].GetComponentInParent<ItemPickup>();

            if (pickup == null || pickup.ItemData == null)
                continue;

            float distance = Vector3.Distance(
                transform.position,
                pickup.InteractionPosition
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentPickup = pickup;
            }
        }
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

using UnityEngine;

public class PlayerLowClearance : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerCrouch crouch;

    [Header("Collision")]
    [SerializeField] private LayerMask obstacleMask;

    [Header("Heights")]
    [SerializeField] private float standingHeight = 1.8f;
    [SerializeField] private float normalCrouchHeight = 1f;

    [Tooltip("Hauteur minimale physique du joueur.")]
    [SerializeField] private float minimumBodyHeight = 0.7f;

    [Header("Automatic Clearance")]
    [Tooltip("Distance maximale à laquelle on cherche un plafond.")]
    [SerializeField] private float maxClearanceHeight = 1.2f;

    [Tooltip("Petite marge sous les meubles.")]
    [SerializeField] private float bodyClearance = 0.05f;

    [Header("Camera")]
    [SerializeField] private float cameraClearance = 0.12f;
    [SerializeField] private float minimumCameraHeight = 0.45f;

    [Header("Detection")]
    [Tooltip("Distance d'anticipation devant le joueur.")]
    [SerializeField] private float forwardCheckDistance = 0.25f;

    [Tooltip("Nombre d'étapes utilisées pour rechercher la hauteur disponible.")]
    [Range(4, 30)]
    [SerializeField] private int heightCheckSteps = 15;

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;

    private CharacterController controller;

    private bool lowClearanceDetected;
    private float detectedHeight;

    private void Awake()
    {
        if (crouch != null)
        {
            controller = crouch.Controller;
        }
    }

    private void Update()
    {
        if (crouch == null || controller == null)
            return;

        HandleClearance();
    }

    private void HandleClearance()
    {


        bool canStand = CanFitHeight(
            standingHeight,
            transform.position
        );
        

        if (crouch.IsCrouching || lowClearanceDetected)
        {
            crouch.SetForcedCrouch(!canStand);
        }
        

        if (!crouch.IsCrouching)
        {
            lowClearanceDetected = false;

            crouch.ClearExternalHeight();

            return;
        }
        Vector3 frontPosition =
            transform.position +
            transform.forward * forwardCheckDistance;

        float currentAvailable =
            FindMaximumAvailableHeight(
                transform.position
            );

        float frontAvailable =
            FindMaximumAvailableHeight(
                frontPosition
            );
        

        float availableHeight =
            Mathf.Min(
                currentAvailable,
                frontAvailable
            );

        detectedHeight = availableHeight;
        if (availableHeight >= normalCrouchHeight)
        {
            lowClearanceDetected = false;

            crouch.SetLowClearanceControllerHeight(
                normalCrouchHeight
            );

            crouch.ClearExternalHeight();

            return;
        }

        lowClearanceDetected = true;

        float bodyHeight =
            availableHeight - bodyClearance;

        float physicalMinimum =
            Mathf.Max(
                minimumBodyHeight,
                controller.radius * 2f
            );

        bodyHeight = Mathf.Clamp(
            bodyHeight,
            physicalMinimum,
            normalCrouchHeight
        );

        crouch.SetLowClearanceControllerHeight(
            bodyHeight
        );
        

        float cameraHeight =
            availableHeight - cameraClearance;

        cameraHeight = Mathf.Clamp(
            cameraHeight,
            minimumCameraHeight,
            crouch.CrouchingViewHeight
        );

        crouch.SetExternalHeight(
            cameraHeight
        );
    }

    private float FindMaximumAvailableHeight(
        Vector3 position)
    {
        float bestHeight = minimumBodyHeight;

        float step =
            (maxClearanceHeight - minimumBodyHeight)
            / heightCheckSteps;

        for (int i = 0; i <= heightCheckSteps; i++)
        {
            float height =
                minimumBodyHeight +
                step * i;

            if (CanFitHeight(height, position))
            {
                bestHeight = height;
            }
            else
            {
                break;
            }
        }

        return bestHeight;
    }

    private bool CanFitHeight(
        float height,
        Vector3 position)
    {
        float radius =
            controller.radius * 0.95f;
        

        height = Mathf.Max(
            height,
            radius * 2f
        );

        Vector3 bottom =
            position +
            Vector3.up * radius;

        Vector3 top =
            position +
            Vector3.up * (height - radius);

        return !Physics.CheckCapsule(
            bottom,
            top,
            radius,
            obstacleMask,
            QueryTriggerInteraction.Ignore
        );
    }

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if (!showDebug)
            return;

        CharacterController cc =
            GetComponent<CharacterController>();

        if (cc == null)
            return;

        float radius =
            cc.radius * 0.95f;

        float height =
            Application.isPlaying
                ? detectedHeight
                : normalCrouchHeight;

        height = Mathf.Max(
            height,
            radius * 2f
        );

        Vector3 position =
            transform.position +
            transform.forward * forwardCheckDistance;

        Vector3 bottom =
            position +
            Vector3.up * radius;

        Vector3 top =
            position +
            Vector3.up * (height - radius);

        Gizmos.color =
            lowClearanceDetected
                ? Color.yellow
                : Color.green;

        Gizmos.DrawWireSphere(
            bottom,
            radius
        );

        Gizmos.DrawWireSphere(
            top,
            radius
        );

        Gizmos.DrawLine(
            bottom + transform.right * radius,
            top + transform.right * radius
        );

        Gizmos.DrawLine(
            bottom - transform.right * radius,
            top - transform.right * radius
        );

        Gizmos.DrawLine(
            bottom + transform.forward * radius,
            top + transform.forward * radius
        );

        Gizmos.DrawLine(
            bottom - transform.forward * radius,
            top - transform.forward * radius
        );
    }

#endif
}
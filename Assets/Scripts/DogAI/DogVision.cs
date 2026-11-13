using UnityEngine;

public class DogVision : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform eyePoint;
    [SerializeField] private Transform player;

    [Header("Vision Settings")]
    [SerializeField, Min(0.1f)] private float viewDistance = 12f;

    [Range(0f, 360f)]
    [SerializeField] private float viewAngle = 110f;

    [SerializeField] private LayerMask obstacleLayers;

    [Header("Crouch Detection")]
    [SerializeField] private bool useCrouchDetection = true;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Range(0.1f, 1f)]
    [SerializeField] private float crouchVisibilityMultiplier = 0.5f;

    [Header("Candle Detection")]
    [SerializeField] private bool useCandleDetection = true;
    [SerializeField] private Light candleLight;

    [Range(1f, 3f)]
    [SerializeField] private float candleVisibilityMultiplier = 1.5f;

    [Header("Detection Time")]
    [SerializeField, Min(0.01f)] private float detectionTime = 1.5f;
    [SerializeField, Min(0.01f)] private float detectionLossSpeed = 1.5f;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;

    private float detectionProgress;

    public bool CanSeePlayer { get; private set; }
    public float DetectionProgress => detectionProgress;

    private void Update()
    {
        bool playerVisible = CheckPlayerVisibility();

        if (playerVisible)
        {
            detectionProgress += Time.deltaTime / detectionTime;
        }
        else
        {
            detectionProgress -=
                Time.deltaTime * detectionLossSpeed;
        }

        detectionProgress = Mathf.Clamp01(detectionProgress);

        CanSeePlayer = detectionProgress >= 1f;
    }

    private bool CheckPlayerVisibility()
    {
        if (eyePoint == null || player == null)
            return false;

        float effectiveDistance = viewDistance;

        // Accroupissement.
        if (useCrouchDetection && IsPlayerCrouching())
        {
            effectiveDistance *= crouchVisibilityMultiplier;
        }

        // Bougie allumée.
        if (useCandleDetection &&
            candleLight != null &&
            candleLight.enabled &&
            candleLight.gameObject.activeInHierarchy)
        {
            effectiveDistance *= candleVisibilityMultiplier;
        }

        Vector3 targetPosition =
            player.position + Vector3.up * 1f;

        Vector3 direction =
            targetPosition - eyePoint.position;

        float distance = direction.magnitude;

        if (distance > effectiveDistance)
            return false;

        float angle = Vector3.Angle(
            transform.forward,
            direction.normalized
        );

        if (angle > viewAngle * 0.5f)
            return false;

        if (Physics.Linecast(
            eyePoint.position,
            targetPosition,
            obstacleLayers,
            QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        return true;
    }

    private bool IsPlayerCrouching()
    {
        // Version temporaire : utilise la touche d'accroupissement.
        // On pourra ensuite lire directement l'état de PlayerCrouch.
        return Input.GetKey(crouchKey);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos || eyePoint == null)
            return;

        Gizmos.color = Color.yellow;

        Vector3 leftDirection =
            Quaternion.Euler(0f, -viewAngle * 0.5f, 0f)
            * transform.forward;

        Vector3 rightDirection =
            Quaternion.Euler(0f, viewAngle * 0.5f, 0f)
            * transform.forward;

        Gizmos.DrawRay(
            eyePoint.position,
            leftDirection * viewDistance
        );

        Gizmos.DrawRay(
            eyePoint.position,
            rightDirection * viewDistance
        );

        if (Application.isPlaying && CanSeePlayer && player != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawLine(
                eyePoint.position,
                player.position + Vector3.up
            );
        }
    }
}


using UnityEngine;

public class DogVision : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform eyePoint;
    [SerializeField] private Transform player;

    [Header("Vision Settings")]
    [SerializeField] private float viewDistance = 12f;

    [Range(0f, 360f)]
    [SerializeField] private float viewAngle = 110f;

    [Header("Obstacles")]
    [SerializeField] private LayerMask obstacleLayers;

    public bool CanSeePlayer { get; private set; }

    private void Update()
    {
        CanSeePlayer = CheckPlayerVisibility();
    }

    private bool CheckPlayerVisibility()
    {
        if (eyePoint == null || player == null)
            return false;

        Vector3 targetPosition = player.position + Vector3.up * 1f;
        Vector3 direction = targetPosition - eyePoint.position;

        if (direction.magnitude > viewDistance)
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

    private void OnDrawGizmosSelected()
    {
        if (eyePoint == null)
            return;

        Gizmos.color = Color.yellow;

        Vector3 leftDirection =
            Quaternion.Euler(0, -viewAngle / 2f, 0)
            * transform.forward;

        Vector3 rightDirection =
            Quaternion.Euler(0, viewAngle / 2f, 0)
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

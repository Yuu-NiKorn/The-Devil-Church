
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DogPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField, Min(0.1f)] private float patrolRadius = 12f;
    [SerializeField, Min(0.1f)] private float patrolSpeed = 2f;

    [Header("Waiting")]
    [SerializeField, Min(0f)] private float minWaitTime = 1f;
    [SerializeField, Min(0f)] private float maxWaitTime = 4f;

    [Header("Navigation")]
    [SerializeField, Min(1)] private int maxAttempts = 20;
    [SerializeField, Min(0.1f)] private float sampleDistance = 2f;

    [Header("Stuck Detection")]
    [SerializeField, Min(0.1f)] private float stuckTime = 2.5f;
    [SerializeField, Min(0.01f)] private float minimumSpeed = 0.1f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    private NavMeshAgent agent;
    private Vector3 startingPosition;

    private float waitTimer;
    private float currentWaitTime;
    private float stuckTimer;

    private bool isWaiting;

    public float PatrolSpeed => patrolSpeed;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        startingPosition = transform.position;
    }

    private void OnEnable()
    {
        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        RestartPatrol();
    }

    private void Update()
    {
        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        if (isWaiting)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= currentWaitTime)
            {
                isWaiting = false;
                ChooseNewDestination();
            }

            return;
        }

        if (agent.pathPending)
            return;

        // Chemin inaccessible.
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid ||
            agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            Log("Chemin invalide ou partiel.");
            ChooseNewDestination();
            return;
        }

        // Aucun chemin actif.
        if (!agent.hasPath)
        {
            Log("Aucun chemin : nouvelle destination.");
            ChooseNewDestination();
            return;
        }

        // Destination atteinte.
        if (agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            StartWaiting();
            return;
        }

        // Détection de blocage.
        if (agent.velocity.magnitude < minimumSpeed)
        {
            stuckTimer += Time.deltaTime;

            if (stuckTimer >= stuckTime)
            {
                Log("Chien bloqué : nouvelle destination.");
                stuckTimer = 0f;
                ChooseNewDestination();
            }
        }
        else
        {
            stuckTimer = 0f;
        }
    }

    public void RestartPatrol()
    {
        waitTimer = 0f;
        stuckTimer = 0f;
        isWaiting = false;

        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
            return;

        agent.isStopped = false;
        agent.speed = patrolSpeed;
        agent.ResetPath();

        ChooseNewDestination();
    }

    private void StartWaiting()
    {
        isWaiting = true;
        waitTimer = 0f;
        stuckTimer = 0f;

        currentWaitTime = Random.Range(
            Mathf.Min(minWaitTime, maxWaitTime),
            Mathf.Max(minWaitTime, maxWaitTime)
        );

        if (agent.isOnNavMesh)
            agent.ResetPath();

        Log("Pause de patrouille.");
    }

    private void ChooseNewDestination()
    {
        if (!agent.isOnNavMesh)
            return;

        stuckTimer = 0f;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPoint =
                Random.insideUnitCircle * patrolRadius;

            Vector3 targetPosition = startingPosition +
                new Vector3(randomPoint.x, 0f, randomPoint.y);

            if (!NavMesh.SamplePosition(
                targetPosition,
                out NavMeshHit hit,
                sampleDistance,
                agent.areaMask))
            {
                continue;
            }

            NavMeshPath path = new NavMeshPath();

            if (agent.CalculatePath(hit.position, path) &&
                path.status == NavMeshPathStatus.PathComplete)
            {
                agent.isStopped = false;

                if (agent.SetDestination(hit.position))
                {
                    Log("Nouvelle destination : " + hit.position);
                    return;
                }
            }
        }

        Log("Aucune destination trouvée. Nouvelle tentative.");
        StartWaiting();
    }

    private void Log(string message)
    {
        if (showDebugLogs)
            Debug.Log("[DogPatrol] " + message, this);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(
            Application.isPlaying ? startingPosition : transform.position,
            patrolRadius
        );
    }
    
    
    public void SetNewPatrolCenter(Vector3 newCenter)
    {
        startingPosition = newCenter;
        RestartPatrol();
    }
}
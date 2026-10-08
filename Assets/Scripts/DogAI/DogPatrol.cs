using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class DogPatrol : MonoBehaviour
{
    [Header("Patrol Settings")]
    [SerializeField] private float patrolRadius = 12f;
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float waitTime = 2f;

    [Header("Navigation")]
    [SerializeField] private int maxAttempts = 20;

    private NavMeshAgent agent;
    private Vector3 startingPosition;
    private float waitTimer;

    public float PatrolSpeed => patrolSpeed;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        startingPosition = transform.position;
    }

    private void Start()
    {
        if (enabled && agent.isOnNavMesh)
            ChooseNewDestination();
    }

    private void Update()
    {
        if (!agent.isOnNavMesh || agent.pathPending)
            return;

        if (agent.remainingDistance > agent.stoppingDistance)
            return;

        waitTimer += Time.deltaTime;

        if (waitTimer >= waitTime)
        {
            waitTimer = 0f;
            ChooseNewDestination();
        }
    }

    private void OnEnable()
    {
        waitTimer = 0f;

        if (agent != null && agent.isOnNavMesh)
        {
            agent.speed = patrolSpeed;
            ChooseNewDestination();
        }
    }

    private void ChooseNewDestination()
    {
        if (!agent.isOnNavMesh)
            return;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPoint =
                Random.insideUnitCircle * patrolRadius;

            Vector3 targetPosition = startingPosition +
                new Vector3(randomPoint.x, 0f, randomPoint.y);

            if (NavMesh.SamplePosition(
                targetPosition,
                out NavMeshHit hit,
                2f,
                agent.areaMask))
            {
                NavMeshPath path = new NavMeshPath();

                if (agent.CalculatePath(hit.position, path) &&
                    path.status == NavMeshPathStatus.PathComplete)
                {
                    agent.SetDestination(hit.position);
                    return;
                }
            }
        }
    }
}


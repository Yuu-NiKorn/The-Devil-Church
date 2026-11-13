using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(DogVision))]
[RequireComponent(typeof(DogPatrol))]
[RequireComponent(typeof(DogHearing))]
public class DogBrain : MonoBehaviour
{
    public enum DogState
    {
        Patrol,
        Investigate,
        Chase,
        Search
    }

    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Chase Settings")]
    [SerializeField, Min(0.1f)] private float chaseSpeed = 5f;
    [SerializeField, Min(0f)] private float loseSightDelay = 2f;

    [Header("Investigation Settings")]
    [SerializeField, Min(0.1f)] private float investigateSpeed = 3f;
    [SerializeField, Min(0f)] private float investigateDuration = 4f;

    [Header("Search Settings")]
    [SerializeField, Min(0.1f)] private float searchSpeed = 2.5f;
    [SerializeField, Min(0.1f)] private float searchDuration = 10f;
    [SerializeField, Min(0.1f)] private float searchRadius = 6f;
    [SerializeField, Min(1)] private int searchMaxAttempts = 15;
    [SerializeField, Min(0f)] private float searchPause = 1f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;
    [SerializeField] private bool showSearchGizmos = true;
    
    
    [Header("Stuck Detection")]
    [SerializeField, Min(0.1f)] private float stuckTime = 2.5f;
    [SerializeField, Min(0.01f)] private float minimumMovement = 0.1f;

    private Vector3 previousPosition;
    private float stuckTimer;

    private NavMeshAgent agent;
    private DogVision vision;
    private DogPatrol patrol;
    private DogHearing hearing;

    private DogState currentState;
    private Vector3 lastKnownPosition;

    private float lostSightTimer;
    private float investigateTimer;
    private float searchTimer;
    private float searchPauseTimer;

    private bool forcedChase;
    private bool isSearchPaused;
    private bool hasReachedSearchCenter;

    public DogState CurrentState => currentState;
    public bool IsForcedChase => forcedChase;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<DogVision>();
        patrol = GetComponent<DogPatrol>();
        hearing = GetComponent<DogHearing>();
    }
    private void Start()
    {
        previousPosition = transform.position;
        ChangeState(DogState.Patrol);
    }
    
    
    private bool IsDogStuck()
    {
        if (!agent.isOnNavMesh || agent.isStopped)
            return false;

        // Ne pas compter les pauses normales.
        if (!agent.hasPath || agent.pathPending)
        {
            previousPosition = transform.position;
            stuckTimer = 0f;
            return false;
        }

        float movement = Vector3.Distance(
            transform.position,
            previousPosition
        );

        previousPosition = transform.position;

        if (movement < minimumMovement * Time.deltaTime)
        {
            stuckTimer += Time.deltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        if (stuckTimer >= stuckTime)
        {
            stuckTimer = 0f;
            return true;
        }

        return false;
    }


    private void Update()
    {
        if (!agent.isOnNavMesh || player == null)
            return;

        // La poursuite forcée reste prioritaire.
        if (forcedChase)
        {
            if (currentState != DogState.Chase)
                ChangeState(DogState.Chase);

            agent.SetDestination(player.position);
            return;
        }

        // Le chien repère visuellement le joueur.
        if (vision.CanSeePlayer &&
            currentState != DogState.Chase)
        {
            ChangeState(DogState.Chase);
        }

        // Le chien entend un bruit.
        if (hearing.HasHeardNoise)
        {
            if (currentState != DogState.Chase)
            {
                lastKnownPosition = hearing.LastHeardPosition;

                if (currentState != DogState.Investigate)
                {
                    ChangeState(DogState.Investigate);
                }
                else
                {
                    agent.SetDestination(lastKnownPosition);
                }
            }

            hearing.ClearNoise();
        }

        switch (currentState)
        {
            case DogState.Patrol:
                break;

            case DogState.Investigate:
                UpdateInvestigate();
                break;

            case DogState.Chase:
                UpdateChase();
                break;

            case DogState.Search:
                UpdateSearch();
                break;
        }
    }

    public void StartForcedChase()
    {
        forcedChase = true;

        if (player == null || !agent.isOnNavMesh)
        {
            Debug.LogWarning(
                "Impossible de lancer la poursuite forcée.",
                this
            );
            return;
        }

        ChangeState(DogState.Chase);
    }

    public void StopForcedChase()
    {
        forcedChase = false;

        if (agent.isOnNavMesh)
            ChangeState(DogState.Patrol);
    }

    private void UpdateInvestigate()
    {
        investigateTimer += Time.deltaTime;

        if (HasReachedDestination())
        {
            ChangeState(DogState.Search);
            return;
        }

        if (investigateTimer >= investigateDuration)
            ChangeState(DogState.Search);
    }

    private void UpdateChase()
    {
        if (vision.CanSeePlayer)
        {
            lastKnownPosition = player.position;
            lostSightTimer = 0f;
            agent.SetDestination(player.position);
        }
        else
        {
            lostSightTimer += Time.deltaTime;
            agent.SetDestination(lastKnownPosition);

            if (lostSightTimer >= loseSightDelay)
                ChangeState(DogState.Search);
        }
    }

    
    private void UpdateSearch()
    {
        searchTimer += Time.deltaTime;

        if (searchTimer >= searchDuration)
        {
            ChangeState(DogState.Patrol);
            return;
        }

        if (!hasReachedSearchCenter)
        {
            if (HasReachedDestination() || IsDogStuck())
            {
                hasReachedSearchCenter = true;
                StartSearchPause();
            }

            return;
        }

        if (isSearchPaused)
        {
            searchPauseTimer += Time.deltaTime;

            if (searchPauseTimer >= searchPause)
            {
                isSearchPaused = false;
                ChooseSearchDestination();
            }

            return;
        }

        if (HasReachedDestination() || IsDogStuck())
        {
            StartSearchPause();
        }
    }

    private void StartSearchPause()
    {
        isSearchPaused = true;
        searchPauseTimer = 0f;
        agent.ResetPath();
    }

    private void ChooseSearchDestination()
    {
        for (int i = 0; i < searchMaxAttempts; i++)
        {
            Vector2 randomPoint =
                Random.insideUnitCircle * searchRadius;

            Vector3 targetPosition = lastKnownPosition +
                new Vector3(randomPoint.x, 0f, randomPoint.y);

            if (!NavMesh.SamplePosition(
                targetPosition,
                out NavMeshHit hit,
                2f,
                agent.areaMask))
            {
                continue;
            }

            NavMeshPath path = new NavMeshPath();

            if (agent.CalculatePath(hit.position, path) &&
                path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetDestination(hit.position);
                return;
            }
        }

        // Aucun point accessible trouvé.
        StartSearchPause();
    }

    
    private bool HasReachedDestination()
    {
        if (!agent.isOnNavMesh || agent.pathPending)
            return false;

        // Un chemin invalide ou partiel ne doit pas bloquer le chien.
        if (agent.pathStatus == NavMeshPathStatus.PathInvalid ||
            agent.pathStatus == NavMeshPathStatus.PathPartial)
        {
            return true;
        }

        if (!agent.hasPath)
            return true;

        return agent.remainingDistance <=
               agent.stoppingDistance + 0.2f;
    }


    private void ChangeState(DogState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            
            
            
            case DogState.Patrol:
                agent.isStopped = false;
                agent.speed = patrol.PatrolSpeed;

                if (!patrol.enabled)
                {
                    patrol.enabled = true;
                }

                // Le chien reprend sa patrouille autour de sa position actuelle.
                patrol.SetNewPatrolCenter(transform.position);

                break;




            case DogState.Investigate:
                patrol.enabled = false;
                agent.isStopped = false;
                agent.speed = investigateSpeed;
                investigateTimer = 0f;
                agent.SetDestination(lastKnownPosition);
                break;

            case DogState.Chase:
                patrol.enabled = false;
                agent.isStopped = false;
                agent.speed = chaseSpeed;
                lostSightTimer = 0f;
                lastKnownPosition = player.position;
                agent.SetDestination(lastKnownPosition);
                break;

            case DogState.Search:
                patrol.enabled = false;
                agent.isStopped = false;
                agent.speed = searchSpeed;

                searchTimer = 0f;
                searchPauseTimer = 0f;
                isSearchPaused = false;
                hasReachedSearchCenter = false;

                agent.SetDestination(lastKnownPosition);
                break;
        }

        if (showDebugLogs)
            Debug.Log("Dog State : " + currentState, this);
    }

    private void OnDrawGizmosSelected()
    {
        if (!showSearchGizmos || !Application.isPlaying)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(lastKnownPosition, searchRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(lastKnownPosition, 0.25f);
    }
}
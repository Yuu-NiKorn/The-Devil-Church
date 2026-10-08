using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(DogVision))]
[RequireComponent(typeof(DogPatrol))]
public class DogBrain : MonoBehaviour
{
    public enum DogState
    {
        Patrol,
        Chase,
        Search
    }

    [Header("References")]
    [SerializeField] private Transform player;

    [Header("Chase Settings")]
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float loseSightDelay = 2f;

    [Header("Search Settings")]
    [SerializeField] private float searchDuration = 6f;
    [SerializeField] private float searchSpeed = 2.5f;

    private NavMeshAgent agent;
    private DogVision vision;
    private DogPatrol patrol;

    private DogState currentState;
    private Vector3 lastKnownPosition;

    private float lostSightTimer;
    private float searchTimer;

    public DogState CurrentState => currentState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<DogVision>();
        patrol = GetComponent<DogPatrol>();
    }

    private void Start()
    {
        ChangeState(DogState.Patrol);
    }

    private void Update()
    {
        if (!agent.isOnNavMesh || player == null)
            return;

        switch (currentState)
        {
            case DogState.Patrol:
                UpdatePatrol();
                break;

            case DogState.Chase:
                UpdateChase();
                break;

            case DogState.Search:
                UpdateSearch();
                break;
        }
    }

    private void UpdatePatrol()
    {
        if (vision.CanSeePlayer)
            ChangeState(DogState.Chase);
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
        if (vision.CanSeePlayer)
        {
            ChangeState(DogState.Chase);
            return;
        }

        searchTimer += Time.deltaTime;

        if (searchTimer >= searchDuration)
            ChangeState(DogState.Patrol);
    }

    private void ChangeState(DogState newState)
    {
        if (currentState == newState &&
            newState != DogState.Patrol)
            return;

        currentState = newState;

        switch (currentState)
        {
            case DogState.Patrol:
                agent.isStopped = false;
                patrol.enabled = true;
                agent.speed = patrol.PatrolSpeed;
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
                agent.SetDestination(lastKnownPosition);
                break;
        }

        Debug.Log("Dog State : " + currentState);
    }
}

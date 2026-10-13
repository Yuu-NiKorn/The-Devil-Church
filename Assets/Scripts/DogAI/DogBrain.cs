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
    [SerializeField] private float chaseSpeed = 5f;
    [SerializeField] private float loseSightDelay = 2f;

    [Header("Investigation Settings")]
    [SerializeField] private float investigateSpeed = 3f;
    [SerializeField] private float investigateDuration = 4f;

    [Header("Search Settings")]
    [SerializeField] private float searchDuration = 6f;
    [SerializeField] private float searchSpeed = 2.5f;

    private NavMeshAgent agent;
    private DogVision vision;
    private DogPatrol patrol;
    private DogHearing hearing;

    private DogState currentState;
    private Vector3 lastKnownPosition;

    private float lostSightTimer;
    private float investigateTimer;
    private float searchTimer;

    public DogState CurrentState => currentState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponent<DogVision>();
        patrol = GetComponent<DogPatrol>();
        hearing = GetComponent<DogHearing>();
    }

    private void Start()
    {
        ChangeState(DogState.Patrol);
    }

    private void Update()
    {
        if (!agent.isOnNavMesh || player == null)
            return;

        // La vision est prioritaire sur l'ouïe.
        if (vision.CanSeePlayer && currentState != DogState.Chase)
        {
            ChangeState(DogState.Chase);
        }

        // Un bruit peut interrompre la patrouille ou la recherche.
        if (hearing.HasHeardNoise)
        {
            if (currentState != DogState.Chase)
            {
                lastKnownPosition = hearing.LastHeardPosition;
                ChangeState(DogState.Investigate);
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

    private void UpdateInvestigate()
    {
        investigateTimer += Time.deltaTime;

        if (investigateTimer >= investigateDuration)
        {
            ChangeState(DogState.Patrol);
        }
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
            ChangeState(DogState.Patrol);
    }

    private void ChangeState(DogState newState)
    {
        currentState = newState;

        switch (currentState)
        {
            case DogState.Patrol:
                agent.isStopped = false;
                patrol.enabled = true;
                agent.speed = patrol.PatrolSpeed;
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
                agent.SetDestination(lastKnownPosition);
                break;
        }

        Debug.Log("Dog State : " + currentState);
    }
}
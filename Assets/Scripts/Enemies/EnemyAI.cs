using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, ILoopResettable
{
    enum EnemyState { Patrol, Chase, MoveToLastSeen, Search }
    EnemyState currentState = EnemyState.Patrol;

    Vector3 startPosition;
    Quaternion startRotation;
    Vector3 homePosition;
    bool hasHome;

    int patrolResumeIndex = -1;

    bool hasDetectedPlayer;

    [Header("Speeds")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4.5f;

    [Header("Search")]
    public float searchDuration = 5f;
    public float searchRadius = 3f;

    [Header("Visuals")]
    public Renderer eyeRenderer;
    public Color patrolColor = Color.blue;
    public Color chaseColor = Color.red;

    EnemyVision vision;
    EnemyPatrol patrol;
    NavMeshAgent agent;
    Transform player;

    Vector3 lastSeenPosition;
    float searchTimer;
    private bool isPaused;

    public bool IsChasing =>
        currentState == EnemyState.Chase ||
        currentState == EnemyState.MoveToLastSeen ||
        currentState == EnemyState.Search;

    private bool inCombat;

    public bool IsUnaware()
    {
        return !hasDetectedPlayer;
    }

    public void ForceChase(Vector3 playerPos)
    {
        currentState = EnemyState.Chase;

        if (patrol != null)
        {
            patrolResumeIndex = patrol.GetCurrentIndex();
            patrol.StopPatrol();
        }

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        lastSeenPosition = playerPos;
        agent.SetDestination(playerPos);
    }

    public void DisableAI()
    {
        agent.isStopped = true;
        enabled = false;
    }


    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        vision = GetComponentInChildren<EnemyVision>();
        patrol = GetComponent<EnemyPatrol>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        startPosition = transform.position;
        startRotation = transform.rotation;

        if (patrol != null)
            patrol.ResumePatrol();
        agent.isStopped = false;
    }

    void Update()
    {
        if (isPaused)
        return;

        // Handling the states of enemy behaviour
        switch (currentState)
        {
            case EnemyState.Patrol:
                HandlePatrol();
                break;

            case EnemyState.Chase:
                HandleChase();
                break;

            case EnemyState.MoveToLastSeen:
                HandleMoveToLastSeen();
                break;

            case EnemyState.Search:
                HandleSearch();
                break;
        }
    }

    // For shift enemies who only move between two positions
    public void SetHome(Vector3 pos)
    {
        homePosition = pos;
        hasHome = true;
    }

    public void ReturnHome()
    {
        if (!hasHome) return;

        NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        agent.SetDestination(homePosition);
    }



    // States

    void HandlePatrol()
    {
        SetEyeColor(patrolColor);

        if (vision.CanSeePlayer)
            EnterChase();
    }

    void HandleChase()
    {
        SetEyeColor(chaseColor);

        if (vision.CanSeePlayer)
        {
            lastSeenPosition = vision.LastSeenPosition;
            agent.SetDestination(player.position);
        }
        else
        {
            agent.SetDestination(lastSeenPosition);
            currentState = EnemyState.MoveToLastSeen;
        }
    }

    void HandleMoveToLastSeen()
    {
        if (vision.CanSeePlayer)
        {
            EnterChase();
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            EnterSearch();
        }
    }

    void HandleSearch()
    {
        if (vision.CanSeePlayer)
        {
            EnterChase();
            return;
        }

        searchTimer -= Time.deltaTime;

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            Vector3 randomPoint = lastSeenPosition + Random.insideUnitSphere * searchRadius;
            randomPoint.y = transform.position.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, searchRadius, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }

        if (searchTimer <= 0f)
        {
            EnterPatrol();
        }
    }

    // Transitions

    void EnterPatrol()
    {
        if (inCombat) return;

        hasDetectedPlayer = false;
        currentState = EnemyState.Patrol;
        agent.isStopped = false;
        agent.speed = patrolSpeed;

        if (patrol != null && patrol.patrolEnabled)
        {
            if (patrolResumeIndex >= 0)
                patrol.SetPatrolIndex(patrolResumeIndex);

            patrol.ResumePatrol();
        }
        else if (hasHome)
        {
            agent.SetDestination(homePosition);
        }

        SetEyeColor(patrolColor);
    }


    void EnterChase()
    {
        hasDetectedPlayer = true;
        currentState = EnemyState.Chase;

        if (patrol != null)
        {
            patrolResumeIndex = patrol.GetCurrentIndex();
            patrol.StopPatrol();
        }

        agent.isStopped = false;
        agent.speed = chaseSpeed;
        lastSeenPosition = vision.LastSeenPosition;
        agent.SetDestination(player.position);
    }

    void EnterSearch()
    {
        hasDetectedPlayer = false;
        currentState = EnemyState.Search;
        searchTimer = searchDuration;
        agent.isStopped = false;
        agent.speed = patrolSpeed;
    }

    void SetEyeColor(Color color)
    {
        if (eyeRenderer != null)
            eyeRenderer.material.color = color;
    }

    public void PauseChase(bool pause)
    {
        isPaused = pause;
        inCombat = pause;
    }

    public void EnterRest(Transform restSpot)
    {
        PauseChase(true);
        agent.isStopped = true;

        transform.position = restSpot.position;
        transform.rotation = restSpot.rotation;

        if (patrol != null)
            patrol.enabled = false;
    }

    public void ExitRest()
    {
        if (patrol != null)
            patrol.enabled = true;

        agent.isStopped = false;
        PauseChase(false);
    }

    public void ForceMoveTo(Vector3 target)
    {
        currentState = EnemyState.Patrol;
        isPaused = true;

        if (patrol != null)
            patrol.StopPatrol();

        agent.isStopped = false;
        agent.ResetPath();
        agent.SetDestination(target);
    }

    public void ResumeFromForcedMove()
    {
        isPaused = false;
    }

    public void ResetState()
    {
        enabled = true;

        StopAllCoroutines();

        if (agent == null)
            agent = GetComponent<NavMeshAgent>();

        if (patrol == null)
            patrol = GetComponent<EnemyPatrol>();

        agent.enabled = false;

        transform.position = startPosition;
        transform.rotation = startRotation;

        agent.enabled = true;
        agent.Warp(startPosition);

        hasDetectedPlayer = false;
        inCombat = false;
        isPaused = false;
        currentState = EnemyState.Patrol;
        searchTimer = 0f;
        patrolResumeIndex = -1;

        agent.ResetPath();
        agent.isStopped = false;
        agent.speed = patrolSpeed;

        if (patrol != null)
        {
            patrol.enabled = true;
            patrol.ResumePatrol();
        }

        SetEyeColor(patrolColor);
    }

    public void OverrideStartPosition(Vector3 pos)
    {
        startPosition = pos;
        startRotation = transform.rotation;
    }

}

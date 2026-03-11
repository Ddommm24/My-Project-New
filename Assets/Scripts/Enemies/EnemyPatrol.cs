using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public struct EnemyPatrolState
{
    public int currentWaypoint;
    public float waypointProgress;
}

public class EnemyPatrol : MonoBehaviour, ILoopResettable
{
    public Transform waypointParent;
    public float patrolSpeed = 2f;
    public float waitTimeAtPoint = 1f;
    public bool patrolEnabled = true;

    public int GetCurrentWaypointIndex()
    {
        return currentIndex;
    }

    public int GetWaypointCount()
    {
        return patrolPoints != null ? patrolPoints.Length : 0;
    }


    private NavMeshAgent agent;
    private Transform[] patrolPoints;
    private int currentIndex;
    private float waitTimer;
    private bool isPatrolling = true;

    void Awake()
    {
        // Not all enemies patrol
        if (!patrolEnabled)
            return;

        if (waypointParent == null)
        {
            Debug.LogWarning($"{name} has patrolEnabled but no waypointParent assigned.");
            patrolEnabled = false;
            return;
        }

        agent = GetComponent<NavMeshAgent>();

        // Get all of the waypoints and put into an array
        patrolPoints = new Transform[waypointParent.childCount];
        for (int i = 0; i < waypointParent.childCount; i++)
            patrolPoints[i] = waypointParent.GetChild(i);
    }

    void Update()
    {
        if (!patrolEnabled) return;
        if (!isPatrolling) return;
        if (agent.pathPending) return;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime;

            if (waitTimer >= waitTimeAtPoint)
            {
                currentIndex = (currentIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentIndex].position);
                waitTimer = 0f;
            }
        }
    }

    // Handling the patrol
    void GoToNextPoint()
    {
        if (!patrolEnabled) return;
        if (agent == null) return;
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        agent.speed = patrolSpeed;
        agent.SetDestination(patrolPoints[currentIndex].position);
    }


    public void StopPatrol()
    {
        isPatrolling = false;
    }

    // After stopped searching
    public void ResumePatrol()
    {
        if (!patrolEnabled) return;
        if (agent == null || patrolPoints == null || patrolPoints.Length == 0)
            return;

        isPatrolling = true;
        GoToNextPoint();
    }


    public EnemyPatrolState GetState()
    {
        return new EnemyPatrolState
        {
            currentWaypoint = currentIndex,
            waypointProgress = 0f 
        };
    }

    public void SetState(EnemyPatrolState state)
    {
        currentIndex = state.currentWaypoint;

        if (agent != null && agent.enabled)
        {
            agent.ResetPath();
            agent.SetDestination(patrolPoints[currentIndex].position);
        }

        ResumePatrol();
    }

    // Where they are in the route
    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    public void SetPatrolIndex(int index)
    {
        currentIndex = index;
    }

    public void ResetState()
    {
        if (!patrolEnabled || patrolPoints == null || patrolPoints.Length == 0)
            return;

        isPatrolling = true;
        waitTimer = 0f;

        if (agent != null && agent.enabled)
        {
            agent.ResetPath();

            int nextIndex = (currentIndex + 1) % patrolPoints.Length;
            Debug.Log($"{name} resetting patrol from {currentIndex} to {nextIndex}");
            agent.SetDestination(patrolPoints[nextIndex].position);
        }
    }

}

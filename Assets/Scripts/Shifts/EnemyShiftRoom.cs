using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyShiftRoom : MonoBehaviour
{
    public EnemyAI outsideEnemy;
    public EnemyAI insideEnemy;

    public Transform outsidePoint;
    public Transform insidePoint;

    public float waitBeforeSecondEnemyMoves = 1.5f;
    public float arriveDistance = 1f;
    public float shiftArriveDistance = 0.4f;

    bool isSwapping;

    public void DoShiftSwap()
    {
        if (isSwapping)
            return;

        if (outsideEnemy == null || insideEnemy == null)
            return;

        if (outsidePoint == null || insidePoint == null)
            return;

        if (!CanShift(outsideEnemy) || !CanShift(insideEnemy))
        {
            Debug.Log("Shift swap skipped because one enemy cannot move.");
            return;
        }

        StartCoroutine(DoShiftSwapRoutine());
    }

    IEnumerator DoShiftSwapRoutine()
    {
        isSwapping = true;

        MoveEnemyTo(outsideEnemy, insidePoint.position);
        yield return new WaitUntil(() => HasReached(outsideEnemy));

        outsideEnemy.PauseChase(false);

        yield return new WaitForSeconds(waitBeforeSecondEnemyMoves);

        MoveEnemyTo(insideEnemy, outsidePoint.position);
        yield return new WaitUntil(() => HasReached(insideEnemy));

        insideEnemy.PauseChase(false);

        EnemyAI temp = outsideEnemy;
        outsideEnemy = insideEnemy;
        insideEnemy = temp;

        Debug.Log("Shift swap complete");

        isSwapping = false;
    }

    bool CanShift(EnemyAI enemy)
    {
        // Debugs for checking if the enemies can do the shift change
        if (enemy == null)
        {
            Debug.Log("CanShift failed: enemy is null");
            return false;
        }

        if (!enemy.enabled)
        {
            Debug.Log($"{enemy.name} CanShift failed: EnemyAI disabled");
            return false;
        }

        EnemyTakedown takedown = enemy.GetComponentInChildren<EnemyTakedown>(true);
        if (takedown != null && takedown.IsDisabled)
        {
            Debug.Log($"{enemy.name} CanShift failed: enemy disabled");
            return false;
        }

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.Log($"{enemy.name} CanShift failed: no NavMeshAgent");
            return false;
        }

        if (!agent.enabled)
        {
            Debug.Log($"{enemy.name} CanShift failed: agent disabled");
            return false;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.Log($"{enemy.name} CanShift failed: not on NavMesh");
            return false;
        }

        return true;
    }

    // Move the shift enemies between the two points
    void MoveEnemyTo(EnemyAI enemy, Vector3 target)
    {
        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        EnemyPatrol patrol = enemy.GetComponent<EnemyPatrol>();

        enemy.SetHome(target);

        if (patrol != null)
            patrol.StopPatrol();

        enemy.PauseChase(true);

        agent.isStopped = false;
        agent.ResetPath();

        float oldStoppingDistance = agent.stoppingDistance;
        agent.stoppingDistance = 0.1f;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, 2f, NavMesh.AllAreas))
        {
            bool success = agent.SetDestination(hit.position);
        }
        else
        {
            Debug.LogError($"{enemy.name} target not on NavMesh: {target}");
        }
    }

    bool HasReached(EnemyAI enemy)
    {
        if (enemy == null)
            return true;

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        if (agent == null || !agent.enabled)
            return true;

        if (agent.pathPending)
            return false;

        return !agent.hasPath || agent.remainingDistance <= shiftArriveDistance;
    }
}
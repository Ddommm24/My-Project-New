using UnityEngine;
using System.Collections.Generic;

public class EnemyPatrolStartManager : MonoBehaviour, ILoopResettable
{
    public EnemyPatrol[] enemies;
    public int minWaypointSpacing = 3;

    List<int> usedIndices = new();

    void Start()
    {
        AssignRandomStartPoints();
    }

    public void AssignRandomStartPoints()
    {
        usedIndices.Clear();

        if (enemies.Length == 0) return;

        int waypointCount = enemies[0].GetWaypointCount();
        if (waypointCount == 0) return;

        foreach (var enemy in enemies)
        {
            if (!enemy.patrolEnabled)
                continue;

            if (enemy.waypointParent == null)
                continue;

            int index = GetSpacedRandomIndex(waypointCount);
            usedIndices.Add(index);

            enemy.SetPatrolIndex(index);
            WarpEnemyToWaypoint(enemy, index);
            enemy.ResetState();
        }
    }

    void WarpEnemyToWaypoint(EnemyPatrol enemy, int index)
    {
        UnityEngine.AI.NavMeshAgent agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
        EnemyAI ai = enemy.GetComponent<EnemyAI>();

        Transform wp = enemy.waypointParent.GetChild(index);

        if (agent != null && agent.enabled)
        {
            agent.Warp(wp.position);
        }
        else
        {
            enemy.transform.position = wp.position;
        }

        if (ai != null)
        {
            ai.SetHome(wp.position);
            ai.OverrideStartPosition(wp.position);
        }
    }


    int GetSpacedRandomIndex(int max)
    {
        const int MAX_ATTEMPTS = 100;

        for (int i = 0; i < MAX_ATTEMPTS; i++)
        {
            int candidate = Random.Range(0, max);
            bool tooClose = false;

            foreach (int used in usedIndices)
            {
                int directDistance = Mathf.Abs(candidate - used);
                int loopDistance = max - directDistance;

                int actualDistance = Mathf.Min(directDistance, loopDistance);

                if (actualDistance < minWaypointSpacing)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
                return candidate;
        }

        return Random.Range(0, max);
    }

    public void ResetState()
    {
        AssignRandomStartPoints();
    }
}

using UnityEngine;
using System.Collections.Generic;

public class EnemyPatrolStartManager : MonoBehaviour, ILoopResettable
{
    public EnemyPatrol[] enemies;
    public int minWaypointSpacing = 6;

    List<int> usedIndices = new();

    void Start()
    {
        AssignRandomStartPoints();
    }

    // Randomly assigns them a starting waypoint, while trying to space the enemies out
    public void AssignRandomStartPoints()
    {
        Dictionary<Transform, List<int>> availableByRoute = new();

        foreach (var enemy in enemies)
        {
            if (enemy == null || !enemy.patrolEnabled || enemy.waypointParent == null)
                continue;

            Transform route = enemy.waypointParent;

            if (!availableByRoute.ContainsKey(route))
            {
                int count = enemy.GetWaypointCount();

                List<int> list = new List<int>();
                for (int i = 0; i < count; i++)
                    list.Add(i);

                availableByRoute[route] = list;
            }

            List<int> available = availableByRoute[route];

            if (available.Count == 0)
                continue;

            int pickIndex = Random.Range(0, available.Count);
            int waypointIndex = available[pickIndex];

            enemy.SetPatrolIndex(waypointIndex);
            WarpEnemyToWaypoint(enemy, waypointIndex);

            RemoveNearbyIndices(available, waypointIndex, enemy.GetWaypointCount());
        }
    }

    void RemoveNearbyIndices(List<int> list, int chosen, int max)
    {
        for (int i = list.Count - 1; i >= 0; i--)
        {
            int idx = list[i];

            int direct = Mathf.Abs(idx - chosen);
            int loop = max - direct;
            int dist = Mathf.Min(direct, loop);

            if (dist < minWaypointSpacing)
                list.RemoveAt(i);
        }
    }

    // Move them to their randomly generated start point
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

    // Trying to space them out, so there are not 2 enemies on top of each other
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

    // New random start points assigned every loop
    public void ResetState()
    {
        AssignRandomStartPoints();
    }
}

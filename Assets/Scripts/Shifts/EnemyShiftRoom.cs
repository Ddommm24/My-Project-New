using UnityEngine;
using UnityEngine.AI;

public class EnemyShiftRoom : MonoBehaviour
{
    public EnemyAI outsideEnemy;
    public EnemyAI insideEnemy;

    public Transform outsidePoint;
    public Transform insidePoint;

    public void DoShiftSwap()
    {
        AssignEnemy(outsideEnemy, insidePoint.position);
        AssignEnemy(insideEnemy, outsidePoint.position);

        EnemyAI temp = outsideEnemy;
        outsideEnemy = insideEnemy;
        insideEnemy = temp;
    }

    void AssignEnemy(EnemyAI enemy, Vector3 target)
    {
        enemy.SetHome(target);

        NavMeshAgent agent = enemy.GetComponent<NavMeshAgent>();
        agent.ResetPath();
        agent.SetDestination(target);
    }
}

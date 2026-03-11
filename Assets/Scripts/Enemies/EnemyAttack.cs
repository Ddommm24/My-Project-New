using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyAttack : MonoBehaviour, ILoopResettable
{
    public float attackRange = 1.5f;
    public float attackCooldown = 1f;
    public float damage = 20f;

    public bool disabled;

    private float lastAttackTime;
    private NavMeshAgent agent;
    private Transform player;
    private PlayerStats playerStats;
    private EnemyAI enemyAI;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.stoppingDistance = attackRange;
        enemyAI = GetComponent<EnemyAI>();

        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerStats = player.GetComponent<PlayerStats>();
    }

    void Update()
    {
        // When disabled do nothing
        if (disabled)
            return;

        // This script only activates when enemy is chasing
        if (!enemyAI.IsChasing)
            return;

        if (Time.time < lastAttackTime + attackCooldown)
            return;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );


        if (!agent.pathPending && distance <= agent.stoppingDistance + attackRange)
        {
            Attack();
        }
    }


    void Attack()
    {
        Debug.Log("Enemy attacked");

        lastAttackTime = Time.time;

        playerStats.TakeDamage(damage);

        StartCoroutine(HitSlowdown());
    }

    // After attacking, enemy is slowed to allow player to get away
    IEnumerator HitSlowdown()
    {
        float originalSpeed = agent.speed;
        agent.speed = originalSpeed * 0.1f;

        float timer = 3f;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;

            if (enemyAI.IsChasing)
                agent.SetDestination(player.position);

            yield return null;
        }

        agent.speed = originalSpeed;
    }

    public void ResetState()
    {
        disabled = false;
        enabled = true;
        lastAttackTime = 0f;
    }


}

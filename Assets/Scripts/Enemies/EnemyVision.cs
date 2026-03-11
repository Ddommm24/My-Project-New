using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public LayerMask obstacleMask;

    public bool CanSeePlayer { get; private set; }
    public Vector3 LastSeenPosition { get; private set; }

    private Transform player;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
            player = p.transform;
        }

    void Update()
    {
        if (player == null)
        {
            CanSeePlayer = false;
            return;
        }
        
        CanSeePlayer = false;

        Vector3 direction = (player.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, player.position);

        // Out of range
        if (distance > viewDistance) return;

        float angle = Vector3.Angle(transform.forward, direction);
        if (angle > viewAngle / 2f) return;

        if (Physics.Raycast(transform.position + Vector3.up, direction, distance, obstacleMask))
            return;

        CanSeePlayer = true;
        // Move towards where they last saw player
        if (CanSeePlayer)
        {
            LastSeenPosition = player.position;
        }

    }

    // For testing
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }

}

// EnemyAI.cs
// Handles the enemy's movement states (Patrol & Chase).

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ExplodingEnemyAi : MonoBehaviour
{
    [Header("References")]
    private Transform player;
    private NavMeshAgent agent;

    [Header("AI Behavior")]
    public Transform[] patrolPoints;
    public float detectionRadius = 5f;

    [Header("Movement Speeds")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3f;

    private int currentPointIndex = 0;

    void Start()
    {
        // Find the player GameObject by its tag
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = true;

        // Start patrolling if patrol points are set
        if (patrolPoints.Length > 0)
        {
            agent.speed = patrolSpeed;
            agent.SetDestination(patrolPoints[0].position);
        }
    }

    void Update()
    {
        // If there's no player, do nothing.
        if (player == null) return;

        // Check the distance to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            // Player is detected: CHASE
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
        else
        {
            // Player is out of range: PATROL
            Patrol();
        }
    }

    void Patrol()
    {
        // Do nothing if there are no patrol points
        if (patrolPoints.Length == 0) return;

        agent.speed = patrolSpeed;

        // If the agent is close to its destination, move to the next patrol point
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPointIndex].position);
        }
    }

    // Draw the detection radius in the editor for easy visualization
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
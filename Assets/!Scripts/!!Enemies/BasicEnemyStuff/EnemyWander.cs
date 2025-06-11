
using UnityEngine;
using UnityEngine.AI;

public class EnemyWander : MonoBehaviour
{
    [Header("Enemy Wander")]
    public NavMeshAgent agent;
    public float walkingRange;

    public Transform centrePoint; // centre of the area the agent wants to move around in

    private bool isPatrolling;  // Track if the enemy is currently patrolling

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        isPatrolling = true;  // Initially start patrolling
    }

    void Update()
    {
        // If patrolling, keep wandering
        if (isPatrolling == true)
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        if (agent.remainingDistance <= agent.stoppingDistance) // Done with path, time to get a new point
        {
            Vector3 point;
            if (RandomPoint(centrePoint.position, walkingRange, out point)) // Find a new random patrol point
            {
                Debug.DrawRay(point, Vector3.up, Color.blue, 1.0f); // For visualization in the editor
                agent.SetDestination(point);  // Set destination to the new patrol point
            }
        }
    }

    bool RandomPoint(Vector3 center, float range, out Vector3 result)
    {
        // Generate a random point inside the given range and check if it's on the navmesh
        Vector3 randomPoint = center + Random.insideUnitSphere * range; // Random point within a sphere
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) // Ensure it's on the NavMesh
        {
            result = hit.position;
            return true;
        }

        result = Vector3.zero;
        return false;
    }

    private void OnDrawGizmosSelected()
    {
        if (centrePoint != null)
        {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Gizmos.DrawWireSphere(centrePoint.position, walkingRange);
        }
    }

    // If needed these can be called in other scripts
    public void StopPatrolling()
    {
        isPatrolling = false;
    }

    public void StartPatrolling()
    {
        isPatrolling = true;
    }
}
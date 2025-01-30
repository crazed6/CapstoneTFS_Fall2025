using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWaypoints : MonoBehaviour
{

    public List<Transform> wayPoint;
    public bool isWayPointPatrol = true;

    NavMeshAgent navMeshAgent;

    public int currentWaypointIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        isWayPointPatrol = true;

    }

    // Update is called once per frame
    void Update()
    {
        if(isWayPointPatrol == true)
        {
            Patrol();
        }
       
    }

    private void Patrol()
    {



        if (wayPoint.Count == 0)
        {

            return;
        }


        float distanceToWaypoint = Vector3.Distance(wayPoint[currentWaypointIndex].position, transform.position);

        // Check if the agent is close enough to the current waypoint
        if (distanceToWaypoint <= 2)
        {

            currentWaypointIndex = (currentWaypointIndex + 1) % wayPoint.Count;
        }

        // Set the destination to the current waypoint
        navMeshAgent.SetDestination(wayPoint[currentWaypointIndex].position);
    }

    public void StopWayPointPatrolling()
    {
        isWayPointPatrol = false;
    }

    public void StartWayPointPatrolling()
    {
        isWayPointPatrol = true;
    }
}
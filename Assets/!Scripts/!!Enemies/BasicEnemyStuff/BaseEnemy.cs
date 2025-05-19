using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemy : MonoBehaviour
{
    //STATE
    protected enum AIState { Idle, Patrol, Chase, Attack }
    protected AIState currentState = AIState.Idle;


    //REFERENCES
    protected NavMeshAgent agent;
    protected Transform player;


    //COMMON ATTRIBUTES
    public float moveSpeed = 3f;
    public float attackRange = 5f;
    public float detectionRange = 10f;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();   
        agent.speed = moveSpeed;

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    protected virtual void Update()
    {
        HandleState(); 
    }

    protected virtual void HandleState()
    {
        switch (currentState)
        { 
            case AIState.Idle:
                Idle();
                break;
            case AIState.Patrol:
                Patrol();
                break;
            case AIState.Chase:
                if (player != null)
                    Chase();
                break;
            case AIState.Attack:
                Attack();
                break;
        }
    }

    protected virtual void Idle()
    {
        // Placeholder for specific idle behavior
    }

    protected virtual void Patrol()
    {
        // To be overridden by patrolling enemies
    }

    protected virtual void Chase()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            currentState = AIState.Attack;
        }
        else
        {
            agent.SetDestination(player.position);
        }
    }

    protected virtual void Attack() 
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > attackRange)
        {
            currentState = AIState.Chase;
            return;
        }

        Debug.Log($"{gameObject.name} is attacking the player!");
    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}

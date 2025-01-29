using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WorkerBehav : MonoBehaviour
{
    //STATE TRACKING
    public AIState currentState = AIState.Idle;
    public bool isWorker = false;

    //REFERENCES
    public Transform player;
    private NavMeshAgent agent;
    public LayerMask groundLayer, playerLayer;
    private PlayerHealth playerHealth;

    //WORKER-SPECIFIC TRACKING
    private bool isProvoked = false;

    //ENEMY HEALTH
    public int maxHealth = 50;
    private int currentHealth;
    public int attackDamage = 10;
    public float attackRange = 1.5f; //Adjust to desired attack range
    public Transform attackOrigin; //A transform marking the attacks starting point


    //ATTACK COOLDOWN
    public float attackCooldown = 3f; //3 seconds between attacks - can adjust after playtesting
    private bool canAttack = true; //Check if enemy can attack

    #region Enemy Initial Setup Functions
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); //Initialize navmesh
        currentHealth = maxHealth;

        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
        }

        if (player != null)
        {
            playerHealth = player.GetComponent<PlayerHealth>();
        }    
    }

    //Updates enemy state
    private void Update()
    {
        if (isWorker && !isProvoked)
        {
            return;
        }

        switch (currentState)
        {
            case AIState.Idle:
                Idle();
                break;
            case AIState.Chase:
                Chase();
                break;
            case AIState.Attack: 
                if (canAttack)
                { 
                    Attack();
                }
                break;

            default:
                break;
        }
    }

    //Enemy collider interactions
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isWorker && !isProvoked)
            {
                Provoke();
            }
            Debug.Log("Player has attacked the Worker enemy!");
        }
    }

    

    #endregion

    #region Enemy Behaviors
    private void Idle()
    {
        if (isWorker)
        {
            Debug.Log($"{gameObject.name} is idling.");
        }
    }

    private void Chase()
    {
        if (player != null)
        {
            agent.SetDestination(player.position);
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if(distanceToPlayer <= attackRange)
            {
                currentState = AIState.Attack; //Switch to attack once in range
            }
            Debug.Log($"{gameObject.name} is chasing the player!");
        }
    }

    private void Attack()
    {
        if (player == null || !canAttack) return; //Early exit if no player or can't attack

        Vector3 directionToPlayer = player.position - attackOrigin.position;
        float distanceToPlayer = directionToPlayer.magnitude;
        
        if (distanceToPlayer <= attackRange)
        {
            currentState = AIState.Attack;
            agent.isStopped = true;

            RaycastHit hit;
            if (Physics.Raycast(attackOrigin.position, directionToPlayer, out hit, attackRange, playerLayer))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    playerHealth?.TakeDamage(attackDamage);
                    Debug.Log($"{gameObject.name} is attacking the player!");
                    StartCoroutine(AttackCooldown());

                    //Add animation trigger here once created
                    //animator.SetTrigger("Attack");
                }
                else
                {
                    Debug.DrawRay(attackOrigin.position, directionToPlayer * attackRange, Color.red, 1f);
                }
            
            }
            else if (currentState == AIState.Attack) //Return to chase if out of range
            {
                agent.isStopped = false;
                currentState = AIState.Chase;
                agent.SetDestination(player.position);
            }
        }
    }
    private IEnumerator AttackCooldown()
    {
        canAttack = false; //Prevent from attacking immediately
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Current health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }

        if (isWorker && !isProvoked)
        {
            Provoke();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }

    public void Provoke()
    {
        if (isWorker && !isProvoked)
        {
            isProvoked = true;
            currentState = AIState.Chase;
            Debug.Log($"{gameObject.name} has been provoked!");
        }
    }

    private void OnMouseDown()
    {
        Provoke();
    }
    

    public enum AIState
    { 
        Idle,
        Chase,
        Attack
    }
    #endregion
    
}

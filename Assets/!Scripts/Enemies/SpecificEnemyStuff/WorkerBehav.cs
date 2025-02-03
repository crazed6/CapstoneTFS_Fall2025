using UnityEngine;
using UnityEngine.AI;

public class WorkerBehav : MonoBehaviour
{
    //STATE TRACKING
    public AIState currentState = AIState.Idle;
    public bool isWorker = false;
    public int attackDamage = 10;

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

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
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
                Attack();
                break;

            default:
                break;
        }
    }

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
            Debug.Log($"{gameObject.name} is chasing the player!");
        }
    }

    private void Attack()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) <= agent.stoppingDistance)
        {
            //Deal damage to the player
            playerHealth?.TakeDamage(attackDamage);
            Debug.Log($"{gameObject.name} is attacking the player!");
        }
    }
    //add functionality for attack cooldown
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


}

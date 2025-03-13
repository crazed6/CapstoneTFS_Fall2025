using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class WorkerBehav : MonoBehaviour
{
    //STATE TRACKING
    public AIState currentState = AIState.Idle; //Worker Idles until attacked
    private Vector3 startPosition; //Store spawn location
    public int maxHealth = 50;
    private int currentHealth;
    private HealthBar healthBarInstance;

    //SCENE REFERENCES
    public Transform player;
    private NavMeshAgent agent;
    public LayerMask groundLayer, playerLayer;
    private PlayerHealth playerHealth;
    public Animator animator;
    public GameObject healthBarPrefab;

    //WORKER-SPECIFIC BEHAVIOUR VARS
    private bool isProvoked = false;
    [SerializeField] private bool isWorker = true;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRange = 1.5f; //Adjust to desired attack range
    [SerializeField] private Transform attackOrigin; //Attack starting point (TBD)
    [SerializeField] private float attackCooldown = 3f; //3 seconds between attacks - can adjust after playtesting
    private bool canAttack = true; //Check if enemy can attack

    public float aggroRange = 10f;


    #region Enemy Initial Setup Functions
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>(); //Initialize navmesh
        currentHealth = maxHealth;
        startPosition = transform.position; //Save initial position
        
        if (player == null)
        {
            player = GameObject.FindWithTag("Player").transform;
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        Collider enemyCollider = GetComponent<Collider>();
        Collider playerCollider = null;

        if (player != null)
        {
            playerCollider = player.GetComponent<Collider>();
            playerHealth = player.GetComponent<PlayerHealth>();
        }
        
        if (enemyCollider != null && playerCollider != null)
        {
            Physics.IgnoreCollision(enemyCollider, playerCollider);
        }
        //Instantiates the health bar on runtime
        if (healthBarPrefab != null)
        {
            GameObject hb = Instantiate(healthBarPrefab, transform);
            //Position above head, adjust local positiona as needed
            hb.transform.localPosition = new Vector3(0, 1.5f, 0);
            healthBarInstance = hb.GetComponent<HealthBar>();
            hb.SetActive(false); //initially hidden until aggro occurs
        }
    }

    //Updates enemy state
    private void Update()
    {
        if (isWorker && !isProvoked)
        {
            return;
        }

        //Reset Worker if player dies
        if (playerHealth != null && playerHealth.currentHealth <= 0)
        {
            ReturnToIdle();
            return;
        }

        if (isProvoked)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);
            if (distanceToPlayer > aggroRange)
            {
               HealAndReset();
               return;
            }
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
                //Continuously check distance even during cooldown
                float distance = Vector3.Distance(attackOrigin.position, player.position);
                if (distance > attackRange)
                {
                    currentState = AIState.Chase;
                }
                else if(canAttack)
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

    private void OnDrawGizmosSelected()
    {
        //Attack Range for debugging
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        //Aggro Range for debugging
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }

    #endregion

    #region Enemy Behaviors

    void Start()
    {
        if (healthBarInstance != null)
        {
            healthBarInstance.SetHealth(maxHealth);
        }
        else
        {
            Debug.LogError("HealthBar reference is missing on " + gameObject.name);
        }
    }

    private void Idle()
    {
        if (isWorker)
        {
            Debug.Log($"{gameObject.name} is idling.");
            //Add animation trigger here once created
            //animator.SetTrigger("Idle");
        }
    }

    private void Chase()
    {
        if (player != null)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            float distanceToPlayer = Vector3.Distance(attackOrigin.position, player.position);

            if(distanceToPlayer <= attackRange)
            {
                currentState = AIState.Attack; //Switch to attack once in range 
            }
            Debug.Log($"{gameObject.name} is chasing the player!");
            //Add animation trigger here once created
            //animator.SetTrigger("Chase");
        }
    }

    private void Attack()
    {  
        
        if (player == null || !canAttack) return; //Early exit if no player or can't attack

        Vector3 directionToPlayer = player.position - attackOrigin.position;
        
        float distanceToPlayer = directionToPlayer.magnitude;
        
        if (distanceToPlayer <= attackRange)
        {
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
                    //If raycast misses (ex. hits an obstacle), resume chasing
                    currentState = AIState.Chase;
                    agent.isStopped = false;
                    Debug.DrawRay(attackOrigin.position, directionToPlayer * attackRange, Color.red, 1f);
                }
            }
            else
            {
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
        Debug.Log($"{gameObject.name} cooldown finished, can attack again");
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Current health: {currentHealth}");

        //Normalize health between 1 and 0
        float normalizedHealth = (float)currentHealth / maxHealth;

        //Update health bar if available
        if(healthBarInstance != null)
        {
            healthBarInstance.gameObject.SetActive(true); //display healthbar if hidden
            healthBarInstance.SetHealth(normalizedHealth);
        }

        if (currentHealth <= 0)
        {
            TriggerDeathSequence();
        }

        if (isWorker && !isProvoked)
        {
            Provoke();
        }

        //Enemy dies
        //Back to idle after player out of range
        //Combat state option for Enemy so it can't regenerate while aggravated
        //

        //Add animation trigger here once created
        //animator.SetTrigger("TakeDamage");
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);

        //Add animation trigger here once created
        //animator.SetTrigger("Die");
    }

    public void Provoke()
    {
        if (isWorker && !isProvoked)
        {
            isProvoked = true;
            currentState = AIState.Chase;
            Debug.Log($"{gameObject.name} has been provoked!");
            Chase(); //Manually call Chase() to ensure movement starts immediately
        }
    }

    public void TriggerDeathSequence()
    {
        Debug.Log($"{gameObject.name} death sequence triggered!");
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }
        // Delay death trigger until after animation sequence
        Die();

        //Add keypress to trigger death sequence
    }

    private void ReturnToIdle()
    {
        Debug.Log($"{gameObject.name} returning to Idle state");

        //Reset NavMesh agent to return to the spawn point
        agent.isStopped = false;
        agent.SetDestination(startPosition);

        //Reset state vars
        isProvoked = false;
        currentState = AIState.Idle;
    }

    private void HealAndReset()
    {
        Debug.Log($"{gameObject.name} has lost aggro. Healing and returning to Idle state.");
        currentHealth = maxHealth;
        isProvoked = false;
        currentState = AIState.Idle;
        agent.isStopped = false;
        agent.SetDestination(startPosition);

        //Hide the health bar when no longer engaged in combat
        if (healthBarInstance != null)
        {
            healthBarInstance.gameObject.SetActive(false);
            healthBarInstance.SetHealth(1f);
        }
    }


    #endregion

}

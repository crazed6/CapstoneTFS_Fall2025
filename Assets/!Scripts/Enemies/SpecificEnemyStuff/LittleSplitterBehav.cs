using UnityEngine;
using UnityEngine.AI;

public class LittleSplitterBehav : MonoBehaviour
{
    [Header("LittleSplitter")]
    public NavMeshAgent agent;
    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;
    public float health;

    // Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;
    public float meleeAttackRange;
    public int meleeDamage;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private EnemyWander patrolScript;  // Reference to the patrol script

    [Header("DeBug")] // DELETE WHEN OTHER STUFF ADDED !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public float damageAmount = 10f;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        patrolScript = GetComponent<EnemyWander>(); // Initialize reference to patrol script
    }

    private void Update()
    {
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // Control behavior based on player's position
        if (!playerInSightRange && !playerInAttackRange)
        {
            patrolScript.StartPatrolling();  // Use the patrol method from the patrol script
        }
        if (playerInSightRange && !playerInAttackRange)
        {
            patrolScript.StopPatrolling();
            ChasePlayer();
        }
        if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
        }

        if (Input.GetMouseButtonDown(0))  //DELETE WHEN OTHERE STUFF ADDED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    LittleSplitterBehav enemy = hit.collider.GetComponent<LittleSplitterBehav>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage((int)damageAmount);
                    }
                }
            }
        } //DELETE WHEN OTHERE STUFF ADDED !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // Make sure the enemy doesn't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            // Melee attack code here
            Collider[] hitPlayers = Physics.OverlapSphere(transform.position, meleeAttackRange, whatIsPlayer);

            //foreach (Collider playerCollider in hitPlayers)
            // {
            //     playerCollider.GetComponent<PlayerHealth>()?.TakeDamage(meleeDamage);  // Assuming the player has a PlayerHealth script to handle health.
            // }

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0) Invoke(nameof(DestroyEnemy), 0.5f);
    }

    private void DestroyEnemy()
    {
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, meleeAttackRange);  // Display melee attack range
    }
}
using UnityEngine;
using UnityEngine.AI;

public class SplitterWayPointBehav : MonoBehaviour
{
    [Header("Splitter")]
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

    // Reference to the patrol script
    private EnemyWaypoints wayPointPatrolScript;

    // Splitting 
    public GameObject smallerEnemyPrefab;
    private bool hasSplit = false;
    public Transform splitSpawnPoint;

    [Header("DeBug")] // DELETE WHEN OTHER STUFF ADDED
    public float damageAmount = 10f;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
         // Initialize reference to patrol script
        wayPointPatrolScript = GetComponent<EnemyWaypoints>();
    }

    private void Update()
    {
        // Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        // Control behavior based on player's position
        if (!playerInSightRange && !playerInAttackRange)
        {
              // Use the patrol method from the patrol script
            wayPointPatrolScript.StartWayPointPatrolling();
        }
        if (playerInSightRange && !playerInAttackRange)
        {
            wayPointPatrolScript.StopWayPointPatrolling();
            ChasePlayer();
        }
        if (playerInAttackRange && playerInSightRange)
        {
            AttackPlayer();
        }

        if (Input.GetMouseButtonDown(0)) // 0 means left mouse button  DELETE WHEN OTHERE STUFF ADDED!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        {
            // Raycast from the camera to where the mouse is pointing
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Check if the raycast hits this enemy
                if (hit.collider.CompareTag("Enemy"))
                {
                    // Call TakeDamage() on the enemy that was clicked
                    SplitterBehav enemy = hit.collider.GetComponent<SplitterBehav>();
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

        if (health <= 0 && !hasSplit)
        {
            hasSplit = true;
            Invoke(nameof(DestroyEnemy), 0.5f);
        }
    }

    private void DestroyEnemy()
    {
        Instantiate(smallerEnemyPrefab, transform.position + Vector3.left, Quaternion.identity);
        Instantiate(smallerEnemyPrefab, transform.position + Vector3.right, Quaternion.identity);

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

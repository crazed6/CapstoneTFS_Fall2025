using UnityEngine;
using UnityEngine.AI;

public class WorkerAI : MonoBehaviour
{
    // Radius in which the enemy can detect the player
    public float detectionRadius = 15f;

    // Time between each shot the enemy can take
    public float shootCooldown = 2f;

    // If the player is moving faster than this, they dodge the shot
    public float speedThresholdToMiss = 10f;

    // How far the enemy can wander when patrolling
    public float patrolRadius = 10f;

    // How long the enemy waits before picking a new patrol point
    public float patrolDelay = 2f;

    private float patrolTimer; // Tracks how long to wait before moving again

    private Transform player; // Reference to the player
    private NavMeshAgent agent; // Reference to the enemy's movement system
    private float shootTimer; // Tracks time between shots

    public GameObject projectilePrefab; // The bullet the enemy shoots
    public Transform firePoint; // Where the bullet comes out from (an empty object at the gun tip)

    public float maxPredictionTime = 1f;       // Max time to predict into the future
    [Range(0f, 1f)]
    public float predictionWeight = 0.7f;      // How much to trust predicted position (0 = don't trust, 1 = full prediction)


    void Start()
    {
        // Get the NavMeshAgent component attached to this enemy
        agent = GetComponent<NavMeshAgent>();

        // Find the player by looking for the "Player" tag
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // Measure the distance from the enemy to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            // Player is in range — stop patrolling and attack
            agent.isStopped = true;
            FacePlayer();
            ShootPlayer();
        }
        else
        {
            // Player is not in range — keep patrolling
            Patrol();
        }
    }

    void Patrol()
    {
        patrolTimer -= Time.deltaTime; // Count down

        // If the enemy has no path or reached its target
        if (!agent.pathPending && agent.remainingDistance < 0.5f && patrolTimer <= 0f)
        {
            // Pick a random direction within a sphere radius
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position;
            randomDirection.y = transform.position.y; // Stay on the same level

            NavMeshHit hit;
            // Find the nearest walkable position to the random spot
            if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
            {
                agent.destination = hit.position; // Move there
                patrolTimer = patrolDelay; // Reset wait time
            }
        }
    }

    void FacePlayer()
    {
        // Calculate direction to the player (flat on the XZ plane)
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));

        // Smoothly rotate towards the player
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void ShootProjectile(Vector3 direction)
    {
        // Create a new projectile at the firePoint, facing forward
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

        // Tell the projectile which direction to go
        proj.GetComponent<ProjectileScript>().Initialize(direction);
    }

    void ShootPlayer()
    {
        // Decrease the shoot timer each frame
        shootTimer -= Time.deltaTime;

        // If we are still in cooldown, don't shoot yet
        if (shootTimer > 0f) return;

        // Reset the shoot cooldown
        shootTimer = shootCooldown;

        // Try to get the Rigidbody from the player (to read player velocity)
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb == null)
        {
            Debug.LogWarning("Player Rigidbody not found!");
            return;
        }

        // Gather important information
        Vector3 playerPosition = player.position;
        Vector3 playerVelocity = playerRb.linearVelocity; // Player's movement velocity
        Vector3 enemyShotPosition = firePoint.position;   // Where the bullet will spawn from
        float projectileSpeed = projectilePrefab.GetComponent<ProjectileScript>().speed; // Bullet speed

        // Calculate how far the player is from the enemy
        float distanceToPlayer = Vector3.Distance(enemyShotPosition, playerPosition);

        // Estimate how much time the projectile needs to reach the player
        float predictionTime = distanceToPlayer / projectileSpeed;

        // Clamp the prediction time to prevent extreme overshooting (based on value set in Inspector)
        predictionTime = Mathf.Clamp(predictionTime, 0f, maxPredictionTime);

        // Predict where the player will be after predictionTime seconds
        Vector3 predictedPlayerPosition = playerPosition + playerVelocity * predictionTime;

        // Blend between aiming at current position and predicted position (based on predictionWeight from Inspector)
        Vector3 finalAimPosition = Vector3.Lerp(playerPosition, predictedPlayerPosition, predictionWeight);

        // Calculate final shooting direction toward the blended aim position
        Vector3 shootDirection = (finalAimPosition - enemyShotPosition).normalized;

        // Actually shoot the projectile
        ShootProjectile(shootDirection);
    }

}


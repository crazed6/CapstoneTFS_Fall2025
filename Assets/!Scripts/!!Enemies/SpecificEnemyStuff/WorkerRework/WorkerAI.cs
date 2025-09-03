using UnityEngine;
using UnityEngine.AI;

public class WorkerAI : MonoBehaviour
{
    public float detectionRadius = 15f;
    public float shootCooldown = 2f;
    public float speedThresholdToMiss = 10f;
    public float patrolRadius = 10f;
    public float patrolDelay = 2f;

    private float patrolTimer;
    private Transform player;
    private NavMeshAgent agent;
    private float shootTimer;

    public GameObject projectilePrefab;
    public Transform firePoint;

    public float maxPredictionTime = 1f;
    [Range(0f, 1f)]
    public float predictionWeight = 0.7f;

    // --- New variables ---
    private Animator animator;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        // Get the Animator from the child object
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // --- Modified Logic ---
        // We only start the attack if the cooldown is ready
        shootTimer -= Time.deltaTime;
        if (distanceToPlayer <= detectionRadius)
        {
            // Stop moving and face the player when in range
            agent.isStopped = true;
            FacePlayer();

            // If the cooldown is ready, trigger the shoot animation
            if (shootTimer <= 0f)
            {
                ShootPlayer();
                shootTimer = shootCooldown; // Reset cooldown here
            }
        }
        else
        {
            // If player is out of range, resume patrolling
            agent.isStopped = false;
            Patrol();
        }
    }

    void Patrol()
    {
        // This function remains the same
        patrolTimer -= Time.deltaTime;

        if (!agent.pathPending && agent.remainingDistance < 0.5f && patrolTimer <= 0f)
        {
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position;
            randomDirection.y = transform.position.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
            {
                agent.destination = hit.position;
                patrolTimer = patrolDelay;
            }
        }
    }

    void FacePlayer()
    {
        // This function remains the same
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    // --- Renamed and Modified Function ---
    // This is now just responsible for triggering the animation
    void ShootPlayer()
    {
        animator.SetTrigger("Shoot");
    }

    // --- New Public Function for Animation Event ---
    // This function contains the logic to actually fire the projectile.
    // The AnimationEventForwarder will call this.
    public void HandleShootTrigger()
    {
        Rigidbody playerRb = player.GetComponent<Rigidbody>();
        if (playerRb == null)
        {
            Debug.LogWarning("Player Rigidbody not found!");
            return;
        }

        Vector3 playerPosition = player.position;
        Vector3 playerVelocity = playerRb.linearVelocity;
        Vector3 enemyShotPosition = firePoint.position;
        float projectileSpeed = projectilePrefab.GetComponent<ProjectileScript>().speed;

        float distanceToPlayer = Vector3.Distance(enemyShotPosition, playerPosition);
        float predictionTime = distanceToPlayer / projectileSpeed;
        predictionTime = Mathf.Clamp(predictionTime, 0f, maxPredictionTime);

        Vector3 predictedPlayerPosition = playerPosition + playerVelocity * predictionTime;
        Vector3 finalAimPosition = Vector3.Lerp(playerPosition, predictedPlayerPosition, predictionWeight);
        Vector3 shootDirection = (finalAimPosition - enemyShotPosition).normalized;

        ShootProjectile(shootDirection);
    }

    void ShootProjectile(Vector3 direction)
    {
        // This function remains mostly the same
        Quaternion rotation = Quaternion.LookRotation(direction);
        // This rotation adjustment might need to be tweaked depending on your projectile model
        // rotation *= Quaternion.Euler(90f, 0f, 90f); 

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, rotation);
        ProjectileScript projScript = proj.GetComponent<ProjectileScript>();
        projScript.Initialize(direction);

        projScript.ownerAudio = GetComponent<WorkerAudio>();

        WorkerAudio workerAudio = GetComponent<WorkerAudio>();
        if (workerAudio != null)
        {
            workerAudio.PlayShoot();
        }
    }
}
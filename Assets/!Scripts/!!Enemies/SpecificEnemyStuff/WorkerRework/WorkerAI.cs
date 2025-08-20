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

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            agent.isStopped = true;
            FacePlayer();
            ShootPlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
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
        Vector3 direction = (player.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    void ShootProjectile(Vector3 direction)
    {
        // Create rotation so projectile faces its shoot direction
        Quaternion rotation = Quaternion.LookRotation(direction);

        // Add 90° tilt on the X axis
        rotation *= Quaternion.Euler(90f, 0f, 90f);

        // Spawn projectile at firePoint with correct facing + tilt
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, rotation);

        // Tell the projectile which direction to go
        proj.GetComponent<ProjectileScript>().Initialize(direction);
    }

    void ShootPlayer()
    {
        shootTimer -= Time.deltaTime;
        if (shootTimer > 0f) return;

        shootTimer = shootCooldown;

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
}

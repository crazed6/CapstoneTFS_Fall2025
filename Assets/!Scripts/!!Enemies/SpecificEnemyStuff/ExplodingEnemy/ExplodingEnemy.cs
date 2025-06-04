using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingEnemy : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3f;
    public float detectionRadius = 5f;

    public float innerRadius = 1f;
    public float middleRadius = 2f;
    public float outerRadius = 3f;
    public float innerDamage = 50f;
    public float middleDamage = 30f;
    public float outerDamage = 15f;
    public ParticleSystem explosionEffect;

    [Header("Explosion Timer Settings")]
    public float outerRadiusTimerStart = 3f;
    public float middleRadiusTimeReduction = 0.5f;

    [Header("Knockback Settings")]
    public float knockbackForce = 10f;

    [Header("Avoidance Settings")]
    public float avoidanceRadius = 2f;
    public LayerMask obstacleMask;
    public float avoidanceStrength = 5f;

    private Transform player;
    private Rigidbody rb;
    private int currentPointIndex = 0;
    private bool hasExploded = false;

    private bool timerStarted = false;
    private float explosionTimer = 0f;
    private bool middleRadiusReduced = false;

    private float patrolYLevel;
    private bool isLingering = false;
    private Linger lingerScript;

    // Last known position
    private Vector3 lastKnownPlayerPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        if (patrolPoints.Length > 0)
            patrolYLevel = patrolPoints[0].position.y;

        lingerScript = GetComponent<Linger>();
        if (lingerScript != null)
            lingerScript.enabled = false;
    }

    void Update()
    {
        if (player == null || hasExploded || isLingering) return;

        if (transform.position.y < (patrolYLevel - 5f) || transform.position.y > (patrolYLevel + 5f))
        {
            EnterLingerMode();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        HandleRadiusExplosion(distanceToPlayer);

        if (distanceToPlayer <= detectionRadius)
        {
            lastKnownPlayerPosition = player.position; // Lock in

            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            Vector3 avoidanceDir = GetAvoidanceDirection(dirToPlayer);
            Vector3 finalDir = (dirToPlayer + avoidanceDir * avoidanceStrength).normalized;

            rb.linearVelocity = finalDir * chaseSpeed;
            FaceDirection(finalDir);
        }
        else
        {
            Patrol();
        }

        if (timerStarted && explosionTimer > 0f)
        {
            explosionTimer -= Time.deltaTime;
            if (explosionTimer <= 0f)
            {
                Explode();
            }
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPointIndex];
        Vector3 direction = (targetPoint.position - transform.position).normalized;

        rb.linearVelocity = direction * patrolSpeed;
        FaceDirection(direction);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.3f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    void FaceDirection(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
        }
    }

    void HandleRadiusExplosion(float distance)
    {
        if (distance <= innerRadius)
        {
            Explode();
            return;
        }

        if (distance <= middleRadius)
        {
            if (!timerStarted)
            {
                explosionTimer = outerRadiusTimerStart;
                timerStarted = true;
            }

            if (!middleRadiusReduced)
            {
                explosionTimer -= middleRadiusTimeReduction;
                middleRadiusReduced = true;
            }
        }
        else if (distance <= outerRadius)
        {
            if (!timerStarted)
            {
                explosionTimer = outerRadiusTimerStart;
                timerStarted = true;
            }
        }
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Vector3 origin = transform.position;
        HashSet<Collider> alreadyDamaged = new HashSet<Collider>();

        ApplyExplosionDamage(origin, innerRadius, innerDamage, alreadyDamaged);
        ApplyExplosionDamage(origin, middleRadius, middleDamage, alreadyDamaged);
        ApplyExplosionDamage(origin, outerRadius, outerDamage, alreadyDamaged);

        if (explosionEffect != null)
            Instantiate(explosionEffect, origin, Quaternion.identity);

        Destroy(gameObject);
    }

    void ApplyExplosionDamage(Vector3 position, float radius, float damage, HashSet<Collider> alreadyDamaged)
    {
        Collider[] hits = Physics.OverlapSphere(position, radius);
        foreach (Collider hit in hits)
        {
            if (alreadyDamaged.Contains(hit)) continue;

            Vector3 knockbackDir = (hit.transform.position - position).normalized;
            float distance = Vector3.Distance(position, hit.transform.position);
            float distanceFactor = 1f - Mathf.Clamp01(distance / radius);

            Rigidbody targetRb = hit.attachedRigidbody;
            if (targetRb != null)
            {
                float finalForce = knockbackForce * distanceFactor;
                targetRb.AddForce(knockbackDir * finalForce, ForceMode.Impulse);
            }

            if (hit.CompareTag("Player"))
            {
                // Player damage logic
            }

            alreadyDamaged.Add(hit);
        }
    }

    Vector3 GetAvoidanceDirection(Vector3 moveDir)
    {
        Ray ray = new Ray(transform.position, moveDir);
        if (Physics.SphereCast(ray, avoidanceRadius, out RaycastHit hit, avoidanceRadius * 2f, obstacleMask))
        {
            Vector3 awayFromObstacle = Vector3.Reflect(moveDir, hit.normal);
            return awayFromObstacle.normalized;
        }
        return Vector3.zero;
    }

    void EnterLingerMode()
    {
        isLingering = true;
        rb.linearVelocity = Vector3.zero;

        if (lingerScript != null)
        {
            lingerScript.targetPosition = lastKnownPlayerPosition;
            lingerScript.enabled = true;
            rb.isKinematic = false; // Enable physics before launch
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, innerRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, middleRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, outerRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, avoidanceRadius);
    }
}

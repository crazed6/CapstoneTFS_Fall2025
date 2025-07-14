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

    [Header("Lift Settings")]
    public float liftRadius = 3f;       // Radius around enemy that lifts player into air before explosion
    public float liftForce = 5f;       // Upward velocity applied to lift player
    public float KnockbackForce = 10f; // Force applied to knockback player during explosion

    private Transform player;
    private Rigidbody rb;
    private int currentPointIndex = 0;
    private bool hasExploded = false;

    private bool timerStarted = false;
    private float explosionTimer = 0f;
    private bool middleRadiusReduced = false;

    private Vector3 lastKnownPlayerPosition;

    public DamageProfile explosionDamageProfile;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null || hasExploded) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        HandleRadiusExplosion(distanceToPlayer);

        if (distanceToPlayer <= detectionRadius)
        {
            lastKnownPlayerPosition = player.position;

            // Make enemy chase player including vertical movement
            Vector3 dirToPlayer = player.position - transform.position;
            dirToPlayer = dirToPlayer.normalized;

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

        // Lift players and javilins inside liftRadius into the air before explosion
        Collider[] liftHits = Physics.OverlapSphere(origin, liftRadius);
        foreach (Collider hit in liftHits)
        {
            if (hit.CompareTag("Player") || hit.CompareTag("Javilin"))
            {
                Rigidbody rb = hit.attachedRigidbody;
                if (rb != null)
                {
                    // Only lift if object is close to ground (no significant vertical velocity)
                    if (Mathf.Abs(rb.linearVelocity.y) < 0.1f)
                    {
                        // Apply upward velocity to lift
                        rb.linearVelocity = new Vector3(rb.linearVelocity.x, liftForce, rb.linearVelocity.z);
                    }
                }
            }
        }

        HashSet<Collider> alreadyDamaged = new HashSet<Collider>();

        // Inner radius debug
        Collider[] innerHits = Physics.OverlapSphere(origin, innerRadius);
        foreach (Collider hit in innerHits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Player caught in INNER explosion radius");
            }
        }

        // Middle radius debug
        Collider[] middleHits = Physics.OverlapSphere(origin, middleRadius);
        foreach (Collider hit in middleHits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Player caught in MIDDLE explosion radius");
            }
        }

        // Outer radius debug
        Collider[] outerHits = Physics.OverlapSphere(origin, outerRadius);
        foreach (Collider hit in outerHits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Player caught in OUTER explosion radius");
            }
        }

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

                targetRb.WakeUp();

                // Knockback direction (horizontal) scaled by force
                Vector3 knockbackVector = knockbackDir * finalForce;

                // Add upward lift so it works even when grounded
                knockbackVector.y += liftForce;

                // Apply the combined knockback + lift as a physics force
                targetRb.AddForce(knockbackVector, ForceMode.VelocityChange);

                Debug.DrawRay(hit.transform.position, knockbackVector.normalized * 10f, Color.red, 1f);
                Debug.Log("Knockback vector applied: " + knockbackVector);
            }

            if (hit.CompareTag("Player"))
            {
                DamageReceive damageReceiver = hit.GetComponent<DamageReceive>();
                Health health = hit.GetComponent<Health>();
                if (damageReceiver != null && health != null)
                {
                    DamageData damageData = new DamageData(gameObject, explosionDamageProfile);
                    health.TakeDamage(damageData);
                    alreadyDamaged.Add(hit);
                    
                 //Josh script, ensure to attach Explode Damage Profile in inspector
                Health playerHealth = hit.GetComponent<Health>();
                if (playerHealth != null && GeneralExplosion != null)
                {
                    DamageData damageData = new DamageData(gameObject, GeneralExplosion);
                    playerHealth.PlayerTakeDamage(damageData);
                }
                //Josh script end
                
                }
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

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, liftRadius);
    }
}

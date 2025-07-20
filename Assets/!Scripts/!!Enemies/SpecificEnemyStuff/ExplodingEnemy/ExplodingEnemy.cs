using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingEnemy : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3f;
    public float detectionRadius = 5f;

    [Header("Explosion Settings")]
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
    public float knockbackForceX = 10f;
    public float knockbackForceY = 10f;

    [Header("Avoidance Settings")]
    public float avoidanceRadius = 2f;
    public LayerMask obstacleMask;
    public float avoidanceStrength = 5f;

    [Header("Lift Settings")]
    public float liftRadius = 3f;
    public float liftForce = 5f;

    private Transform player;
    private Rigidbody rb;
    private int currentPointIndex = 0;
    private bool hasExploded = false;

    private bool timerStarted = false;
    private float explosionTimer = 0f;
    private bool middleRadiusReduced = false;

    private Vector3 lastKnownPlayerPosition;

    //public DamageProfile GeneralExplosion; // Josh's damage profile reference
    public DamageProfile InnerExplosionDamage; // Josh's damage profile reference
    public DamageProfile MiddleExplosionDamage; // Josh's damage profile reference
    public DamageProfile OuterExplosionDamage; // Josh's damage profile reference

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

            // Chase player with avoidance logic
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

        // Countdown explosion timer
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
            Explode(); // Immediate explosion if player is in inner zone
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
                explosionTimer -= middleRadiusTimeReduction; // Faster if player is closer
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

        // Lift players or javilins inside liftRadius
        Collider[] liftHits = Physics.OverlapSphere(origin, liftRadius);
        foreach (Collider hit in liftHits)
        {
            if (hit.CompareTag("Player") || hit.CompareTag("Javilin"))
            {
                Rigidbody rb = hit.attachedRigidbody;
                if (rb != null && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
                {
                    rb.linearVelocity = new Vector3(rb.linearVelocity.x, liftForce, rb.linearVelocity.z);
                }
            }
        }

        // Apply explosion damage and knockback only once per target
        HashSet<Collider> alreadyDamaged = new HashSet<Collider>();
        ApplySingleExplosionDamage(origin, alreadyDamaged);

        // Spawn VFX
        if (explosionEffect != null)
            Instantiate(explosionEffect, origin, Quaternion.identity);

        Destroy(gameObject);
    }

    /// <summary>
    /// Applies knockback and determines correct damage tier based on distance.
    /// Ensures only one hit per target.
    /// </summary>
    void ApplySingleExplosionDamage(Vector3 position, HashSet<Collider> alreadyDamaged)
    {
        Collider[] hits = Physics.OverlapSphere(position, outerRadius);
        foreach (Collider hit in hits)
        {
            if (alreadyDamaged.Contains(hit)) continue;

            float distance = Vector3.Distance(position, hit.transform.position);
            float damageToApply = 0f;
            DamageProfile selectedProfile = null;

            // Decide damage based on tiered radii
            if (distance <= innerRadius)
            {
                damageToApply = innerDamage;
                selectedProfile = InnerExplosionDamage; // Use Josh's damage profile for inner explosion
                Debug.Log("Player caught in INNER explosion radius");


            }
            else if (distance <= middleRadius && distance >= innerRadius)
            {
                damageToApply = middleDamage;
                selectedProfile = MiddleExplosionDamage; // Use Josh's damage profile for middle explosion
                Debug.Log("Player caught in MIDDLE explosion radius");
            }
            else if (distance <= outerRadius && distance >= middleRadius)
            {
                damageToApply = outerDamage;
                selectedProfile = OuterExplosionDamage; // Use Josh's damage profile for outer explosion
                Debug.Log("Player caught in OUTER explosion radius");
            }
            else
            {
                continue; // Shouldn't happen, but for safety
            }

            // Knockback force direction and magnitude
            Vector3 knockbackDir = (hit.transform.position - position).normalized;
            float distanceFactor = 1f - Mathf.Clamp01(distance / outerRadius);

            Rigidbody targetRb = hit.attachedRigidbody;
            if (targetRb != null)
            {
                targetRb.WakeUp();
                targetRb.linearVelocity = Vector3.zero;

                Vector3 adjustedKnockback = new Vector3(
                    knockbackDir.x * knockbackForceX * distanceFactor,
                    knockbackForceY * distanceFactor,
                    knockbackDir.z * knockbackForceX * distanceFactor
                );

                targetRb.AddForce(adjustedKnockback, ForceMode.Impulse);
                Debug.DrawRay(targetRb.position, adjustedKnockback.normalized * 10f, Color.red, 5f);
                Debug.Log("Adjusted knockback applied: " + adjustedKnockback);
            }

            if (hit.CompareTag("Player"))
            {
                // Apply damage through Josh's custom damage system
                Health playerHealth = hit.GetComponent<Health>();
                if (playerHealth != null && selectedProfile!= null)
                {
                    DamageData damageData = new DamageData(gameObject, selectedProfile);
                    playerHealth.PlayerTakeDamage(damageData);
                }
            }

            alreadyDamaged.Add(hit);
        }
    }

    /// <summary>
    /// Used for future vertical lift delay if needed (currently unused)
    /// </summary>
    private IEnumerator ApplyStaggeredKnockback(Rigidbody targetRb, Vector3 knockbackDir, float finalForce)
    {
        Vector3 liftVector = Vector3.up * liftForce;
        targetRb.AddForce(liftVector, ForceMode.VelocityChange);
        Debug.Log("Vertical lift applied: " + liftVector);

        yield return new WaitForSeconds(0.2f);
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

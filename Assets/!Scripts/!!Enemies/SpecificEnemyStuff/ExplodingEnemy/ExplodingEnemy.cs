
using System.Collections.Generic;
using UnityEngine;

public class ExplodingEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public bool isStationary = false;
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3f;
    public float detectionRadius = 5f;

    [Header("Y-Level Management")]
    public GameObject yLevelReference;
    public float yLevelBuffer = 5f;

    [Header("Explosion Damage")]
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

    private float referenceYLevel;
    private bool isLingering = false;
    private Linger lingerScript;

    private Vector3 lastKnownPlayerPosition = Vector3.zero;
    private bool hasDetectedPlayer = false;

    //Declared Variable
    //Josh testing
    public DamageProfile InnerRadiusExplosion; // Reference to the damage profile for explosion damage
    public DamageProfile MiddleRadiusExplosion;
    public DamageProfile OuterRadiusExplosion;
    public DamageProfile GeneralExplosion; // General explosion damage profile
    //Josh testing end

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        SetReferenceYLevel();

        lingerScript = GetComponent<Linger>();
        if (lingerScript != null)
            lingerScript.enabled = false;
    }

    void SetReferenceYLevel()
    {
        if (yLevelReference != null)
        {
            referenceYLevel = yLevelReference.transform.position.y;
        }
        else if (!isStationary && patrolPoints.Length > 0)
        {
            referenceYLevel = patrolPoints[0].position.y;
        }
        else
        {
            referenceYLevel = transform.position.y;
        }
    }

    void Update()
    {
        if (player == null || hasExploded || isLingering) return;

        if (transform.position.y < (referenceYLevel - yLevelBuffer) ||
            transform.position.y > (referenceYLevel + yLevelBuffer))
        {
            EnterLingerMode();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        HandleRadiusExplosion(distanceToPlayer);

        if (distanceToPlayer <= detectionRadius)
        {
            lastKnownPlayerPosition = player.position;
            hasDetectedPlayer = true;

            Vector3 dirToPlayer = (player.position - transform.position).normalized;
            Vector3 avoidanceDir = GetAvoidanceDirection(dirToPlayer);
            Vector3 finalDir = (dirToPlayer + avoidanceDir * avoidanceStrength).normalized;

            rb.linearVelocity = finalDir * chaseSpeed;
            FaceDirection(finalDir);
        }
        else
        {
            if (isStationary)
            {
                if (hasDetectedPlayer)
                {
                    EnterLingerMode();
                }
                else
                {
                    rb.linearVelocity = Vector3.zero;
                }
            }
            else
            {
                if (hasDetectedPlayer)
                {
                    EnterLingerMode();
                }
                else
                {
                    Patrol();
                }
            }
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
            if (hit.transform == transform) continue;

            Vector3 knockbackDir = (hit.transform.position - position).normalized;
            if (knockbackDir == Vector3.zero)
                knockbackDir = Vector3.up;

            float distance = Vector3.Distance(position, hit.transform.position);
            float distanceFactor = 1f - Mathf.Clamp01(distance / radius);

            // Apply full knockback in direction from explosion center to object
            float finalForce = knockbackForce * distanceFactor;

            Rigidbody targetRb = hit.GetComponent<Rigidbody>();
            if (targetRb == null)
                targetRb = hit.attachedRigidbody;

            if (targetRb != null && !targetRb.isKinematic)
            {
                targetRb.AddForce(knockbackDir * finalForce, ForceMode.Impulse);
                Debug.Log($"Applied knockback force of {finalForce:F2} to {hit.name}");
            }

            if (hit.CompareTag("Player"))
            {
                Debug.Log($"Player hit by explosion for {damage} damage");

                //Josh script, ensure to attach Explode Damage Profile in inspector
                Health playerHealth = hit.GetComponent<Health>();
                if (playerHealth != null && GeneralExplosion != null)
                {
                    DamageData damageData = new DamageData(gameObject, GeneralExplosion);
                    playerHealth.PlayerTakeDamage(damageData);
                }
                //Josh script end
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
            rb.isKinematic = false;
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

        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Vector3 pos = transform.position;
            Vector3 upperBound = new Vector3(pos.x, referenceYLevel + yLevelBuffer, pos.z);
            Vector3 lowerBound = new Vector3(pos.x, referenceYLevel - yLevelBuffer, pos.z);

            Gizmos.DrawWireCube(upperBound, new Vector3(0.5f, 0.1f, 0.5f));
            Gizmos.DrawWireCube(lowerBound, new Vector3(0.5f, 0.1f, 0.5f));
            Gizmos.DrawLine(upperBound, lowerBound);
        }
    }
}

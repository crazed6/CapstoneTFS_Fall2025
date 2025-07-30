//Asad with Help of Joshua and Ritwik
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    public LayerMask damageableLayer; // Layer mask to filter damageable objects

    [Header("Explosion Timer Settings")]
    public float outerRadiusTimerStart = 3f;
    public float middleRadiusTimeReduction = 2f;

    [Header("Knockback Settings")]
    public float knockbackForceX = 10f;
    public float knockbackForceY = 10f;

    private Transform player;
    private NavMeshAgent agent;
    private int currentPointIndex = 0;
    private bool hasExploded = false;

    private bool timerStarted = false;
    private float explosionTimer = 0f;


    private Vector3 lastKnownPlayerPosition;

    public DamageProfile InnerExplosionDamage;
    public DamageProfile MiddleExplosionDamage;
    public DamageProfile OuterExplosionDamage;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;

        agent = GetComponent<NavMeshAgent>();
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent missing on ExplodingEnemy.");
            enabled = false;
            return;
        }

        agent.speed = patrolSpeed;
        agent.autoBraking = true;

        if (patrolPoints.Length > 0)
        {
            agent.SetDestination(patrolPoints[0].position);
        }
    }

    void Update()
    {
        if (player == null || hasExploded) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        HandleRadiusExplosion(distanceToPlayer);

        if (distanceToPlayer <= detectionRadius)
        {
            lastKnownPlayerPosition = player.position;
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);
        }
        else
        {
            Patrol();
        }

        if (timerStarted)
        {
            explosionTimer -= Time.deltaTime * (distanceToPlayer < middleRadius ? middleRadiusTimeReduction : 1); // Speed up timer if inside middle radius
            if (explosionTimer <= 0f)
            {
                Explode();
            }
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        agent.speed = patrolSpeed;

        if (!agent.pathPending && agent.remainingDistance < 0.3f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPointIndex].position);
        }
    }

    void HandleRadiusExplosion(float distance)
    {
        // 💥 Immediate explosion if inside inner radius
        if (distance <= innerRadius)
        {
            explosionTimer = 0f;
 
        }

        // 🔒 Start explosion timer permanently when player enters outer radius
        if (!timerStarted && distance <= outerRadius)
        {
            explosionTimer = outerRadiusTimerStart;
            timerStarted = true;
        }

    }

    //Javilin exploding and it works YAY
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Javilin") && !hasExploded)
        {
            Explode();
        }
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Vector3 origin = transform.position;

        HashSet<Collider> alreadyDamaged = new HashSet<Collider>();
        ApplySingleExplosionDamage(origin, alreadyDamaged);

        if (explosionEffect != null)
            Instantiate(explosionEffect, origin, Quaternion.identity);

        Destroy(gameObject);
    }

    void ApplySingleExplosionDamage(Vector3 position, HashSet<Collider> alreadyDamaged) // Applies single damage and knockback loop
    {
        //Declared variables before for each so it dosnt die after the for each loop
        float distance = 0f; 
        float damageToApply = 0f;
        Vector3 hitPosition = Vector3.zero;
        Rigidbody targetRb = null;
        Collider hitCollider = null;
        DamageProfile selectedProfile = null;

        Collider[] hits = Physics.OverlapSphere(position, outerRadius, damageableLayer); 
        /* checks the over lap sphere compare to the outer radius, and check how man collider are in the overlap, 
            then checks the damage and knockback variable for one collider relating to it */
        foreach (Collider hit in hits)
        {
            if (alreadyDamaged.Contains(hit)) continue;

            distance = Vector3.Distance(position, hit.transform.position);
            damageToApply = 0f;
            

            if (distance <= innerRadius)
            {
                damageToApply = innerDamage;
                selectedProfile = InnerExplosionDamage;
            }
            else if (distance <= middleRadius)
            {
                damageToApply = middleDamage;
                selectedProfile = MiddleExplosionDamage;
            }
            else if (distance <= outerRadius)
            {
                damageToApply = outerDamage;
                selectedProfile = OuterExplosionDamage;
            }

            else continue;

            hitPosition = hit.transform.position;
            targetRb = hit.attachedRigidbody;
            hitCollider = hit; // Store the hit enemy collider for later use
        }

        //Vector3 knockbackDir = (hitPosition - position).normalized;
        float distanceFactor = 1f - Mathf.Clamp01(distance / outerRadius);
        Debug.Log($"distance factor is {distanceFactor}");

        //if (targetRb != null)
        //{
        //    targetRb.WakeUp();
        //    targetRb.linearVelocity = Vector3.zero;

        //    Vector3 adjustedKnockback = new Vector3(
        //        knockbackDir.x * knockbackForceX * distanceFactor,
        //        knockbackForceY * distanceFactor,
        //        knockbackDir.z * knockbackForceX * distanceFactor
        //    );

        //    targetRb.AddForce(adjustedKnockback, ForceMode.Impulse);
        //}
        // Knockback
       
        KnockbackReceiver kb = hitCollider.GetComponent<KnockbackReceiver>();
        if  (kb != null)
        {
            KnockbackData kbData = new KnockbackData(
                source: transform.position,
                force: knockbackForceX, // Adjust force based on distance
                duration: 1f,
                upwardForce: knockbackForceY,
                overrideVel: true
            ); 

            kb.ApplyKnockback(kbData);
        }


        if (hitCollider.CompareTag("Player"))
        {
            Health playerHealth = hitCollider.GetComponent<Health>();
            if (playerHealth != null && selectedProfile != null)
            {
                DamageData damageData = new DamageData(gameObject, selectedProfile);
                playerHealth.PlayerTakeDamage(damageData);
            }
        }

        alreadyDamaged.Add(hitCollider);
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
    }
}

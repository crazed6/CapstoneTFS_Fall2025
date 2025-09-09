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
    public GameObject explosionPrefab;
    public GameObject warningEffectPrefab; // NEW – warning particle while countdown runs
    public LayerMask damageableLayer; // Layer mask to filter damageable objects

    [Header("Explosion Timer Settings")]
    public float outerRadiusTimerStart = 3f;
    public float middleRadiusTimeReduction = 2f;

    [Header("Knockback Settings")]
    public float knockbackForceX = 10f;
    public float knockbackForceY = 10f;

    [Header("Bobbing Settings")]
    public float bobAmplitude = 0.1f; // How high the bobbing goes
    public float bobFrequency = 4f;   // How fast it bobs

    private float bobTimer = 0f;
    private float baseYPos;

    private Transform player;
    private NavMeshAgent agent;
    private int currentPointIndex = 0;
    private bool hasExploded = false;

    public bool TimerStarted { get { return timerStarted; } }
    private bool timerStarted = false;
    public float explosionTimer = 0f;

    private Vector3 lastKnownPlayerPosition;

    // NEW – Active warning effect instance
    private GameObject activeWarningEffect;

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

        // Store original Y position for bobbing
        baseYPos = transform.position.y;
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
            // Start warning effect if not already active
            if (warningEffectPrefab != null && activeWarningEffect == null)
            {
                activeWarningEffect = Instantiate(warningEffectPrefab, transform.position, Quaternion.identity, transform);
            }

            explosionTimer -= Time.deltaTime * (distanceToPlayer < middleRadius ? middleRadiusTimeReduction : 1);
            if (explosionTimer <= 0f)
            {
                Explode();
            }
        }

        // 🌀 Bobbing effect only if moving
        if (agent.velocity.magnitude > 0.1f)
        {
            bobTimer += Time.deltaTime * bobFrequency;
            Vector3 pos = transform.position;

            pos.y = baseYPos + Mathf.Sin(bobTimer) * bobAmplitude;
            pos.x += Mathf.Sin(bobTimer) * (bobAmplitude * 0.2f);

            transform.position = pos;
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

        // Stop/destroy warning effect
        if (activeWarningEffect != null)
        {
            Destroy(activeWarningEffect);
            activeWarningEffect = null;
        }

        GetComponent<ExploderAudio>()?.PlayExplosion();

        Vector3 origin = transform.position;

        HashSet<Collider> alreadyDamaged = new HashSet<Collider>();
        ApplySingleExplosionDamage(origin, alreadyDamaged);

        if (explosionPrefab != null)
        {
            GameObject explosionInstance = Instantiate(explosionPrefab, origin, Quaternion.identity);

            float maxDuration = 0f;
            foreach (var ps in explosionInstance.GetComponentsInChildren<ParticleSystem>())
            {
                maxDuration = Mathf.Max(maxDuration, ps.main.duration + ps.main.startLifetime.constantMax);
            }
            Destroy(explosionInstance, maxDuration);
        }

        Destroy(gameObject);
    }

    void ApplySingleExplosionDamage(Vector3 position, HashSet<Collider> alreadyDamaged)
    {
        float distance = 0f;
        float damageToApply = 0f;
        Vector3 hitPosition = Vector3.zero;
        Rigidbody targetRb = null;
        Collider hitCollider = null;
        DamageProfile selectedProfile = null;

        Collider[] hits = Physics.OverlapSphere(position, outerRadius, damageableLayer);
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
            hitCollider = hit;
        }

        float distanceFactor = 1f - Mathf.Clamp01(distance / outerRadius);
        Debug.Log($"distance factor is {distanceFactor}");

        if (hitCollider != null)
        {
            KnockbackReceiver kb = hitCollider.GetComponent<KnockbackReceiver>();
            if (kb != null)
            {
                KnockbackData kbData = new KnockbackData(
                    source: transform.position,
                    force: knockbackForceX,
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

    public float GetPlayerDistance()
    {
        if (player == null) return Mathf.Infinity;
        return Vector3.Distance(transform.position, player.position);
    }
}

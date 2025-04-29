using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingEnemy : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3f;
    public float detectionRadius = 5f;

    // Explosion radii and damage
    public float innerRadius = 1f;
    public float middleRadius = 2f;
    public float outerRadius = 3f;

    public float innerDamage = 50f;
    public float middleDamage = 30f;
    public float outerDamage = 15f;

    public ParticleSystem explosionEffect;

    [Header("Explosion Timer Settings")]
    public float outerRadiusTimerStart = 1f; // How long the timer is when entering outer radius
    public float middleRadiusTimeReduction = 0.5f; // How much time is reduced when in middle radius

    private Transform player;
    private int currentPointIndex = 0;
    private bool isDetonating = false;
    private bool hasExploded = false;
    private bool isChasing = false;

    private Rigidbody rb;

    // Timer-related variables
    private bool timerStarted = false;
    private float explosionTimer = 0f;

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

        if (distanceToPlayer <= detectionRadius && !isDetonating)
        {
            isChasing = true;
            ChasePlayer();
        }
        else if (!isDetonating)
        {
            isChasing = false;
            Patrol();
        }

        if (timerStarted)
        {
            explosionTimer -= Time.deltaTime;
            if (explosionTimer <= 0f)
            {
                Explode();
            }
        }
    }

    void HandleRadiusExplosion(float distance)
    {
        if (distance <= innerRadius)
        {
            Explode();
        }
        else if (distance <= middleRadius)
        {
            if (!timerStarted)
            {
                explosionTimer = outerRadiusTimerStart;
                timerStarted = true;
            }
            explosionTimer -= middleRadiusTimeReduction;
            explosionTimer = Mathf.Max(0f, explosionTimer); // Clamp to prevent negative timer
        }
        else if (distance <= outerRadius)
        {
            if (!timerStarted)
            {
                explosionTimer = outerRadiusTimerStart;
                timerStarted = true;
            }
        }
        // IMPORTANT: No else to reset the timer anymore!
        // Timer continues ticking even if the player leaves
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPointIndex];
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        rb.linearVelocity = direction * patrolSpeed;

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.3f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    void ChasePlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (hasExploded || isDetonating) return;

        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Javilin"))
        {
            StartCoroutine(DetonationCountdown());
        }
    }

    IEnumerator DetonationCountdown()
    {
        isDetonating = true;
        rb.linearVelocity = Vector3.zero;
        yield return new WaitForSeconds(0.2f);
        Explode();
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
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);

        foreach (Collider hit in hitColliders)
        {
            if (alreadyDamaged.Contains(hit)) continue;

            if (hit.CompareTag("Player"))
            {
                NotPlayerHealth health = hit.GetComponent<NotPlayerHealth>() ?? hit.GetComponentInParent<NotPlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    alreadyDamaged.Add(hit);
                }
                else
                {
                    Debug.LogWarning("No NotPlayerHealth found on Player object or parent.");
                }
            }
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
}

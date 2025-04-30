using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingEnemy : MonoBehaviour
{
    // Patrol waypoints
    public Transform[] patrolPoints;

    // Movement speeds
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3f;

    // Detection range for chasing the player
    public float detectionRadius = 5f;

    // Explosion radii
    public float innerRadius = 1f;
    public float middleRadius = 2f;
    public float outerRadius = 3f;

    // Damage values
    public float innerDamage = 50f;
    public float middleDamage = 30f;
    public float outerDamage = 15f;

    // Particle effect for explosion
    public ParticleSystem explosionEffect;

    [Header("Explosion Timer Settings")]
    public float outerRadiusTimerStart = 3f;        // Time before explosion when player enters outer radius
    public float middleRadiusTimeReduction = 0.5f;  // Amount of time reduced if player enters middle radius

    
    private Transform player;
    private Rigidbody rb;
    private int currentPointIndex = 0;
    private bool hasExploded = false;
    private bool timerStarted = false;
    private float explosionTimer = 0f;
    private bool middleRadiusReduced = false; // Ensure reduction only happens once

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Find player in scene
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        if (player == null || hasExploded) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Check for explosion triggers based on player distance
        HandleRadiusExplosion(distanceToPlayer);

        // Chase or patrol
        if (distanceToPlayer <= detectionRadius)
        {
            ChasePlayer();
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

    void HandleRadiusExplosion(float distance)
    {
        // Player in inner radius? Explode immediately
        if (distance <= innerRadius)
        {
            Explode();
            return;
        }

        // Player in middle radius? Start timer (if not already) and reduce once
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
        // Player only in outer radius
        else if (distance <= outerRadius)
        {
            if (!timerStarted)
            {
                explosionTimer = outerRadiusTimerStart;
                timerStarted = true;
            }
        }
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform targetPoint = patrolPoints[currentPointIndex];
        Vector3 direction = (targetPoint.position - transform.position).normalized;

        rb.linearVelocity = direction * patrolSpeed; // ? FIXED

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.3f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    void ChasePlayer()
    {
        if (player == null) return;

        Vector3 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed; // ? FIXED
    }

    void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Vector3 origin = transform.position;
        HashSet<Collider> alreadyDamaged = new HashSet<Collider>();

        // Apply damage in each radius
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

            if (hit.CompareTag("Player"))
            {
                NotPlayerHealth health = hit.GetComponent<NotPlayerHealth>() ?? hit.GetComponentInParent<NotPlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    alreadyDamaged.Add(hit);
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

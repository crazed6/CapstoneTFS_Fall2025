using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingEnemy : MonoBehaviour
{
    // Patrol points the enemy will follow when not chasing the player
    public Transform[] patrolPoints;

    // Speed of patrolling and chasing
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3f;

    // The detection radius where the enemy will start chasing the player
    public float detectionRadius = 5f;

    // Explosion radii (the area in which the explosion deals damage)
    public float innerRadius = 1f;
    public float middleRadius = 2f;
    public float outerRadius = 3f;

    // Damage values based on distance from the explosion center
    public float innerDamage = 50f;
    public float middleDamage = 30f;
    public float outerDamage = 15f;

    // Particle system for explosion effect
    public ParticleSystem explosionEffect;

    // Explosion timer settings
    [Header("Explosion Timer Settings")]
    public float outerRadiusTimerStart = 1f; // How long the timer lasts when the player enters outer radius
    public float middleRadiusTimeReduction = 0.5f; // How much time is reduced when entering middle radius

    // Private variables for tracking state
    private Transform player; // Reference to the player object
    private int currentPointIndex = 0; // Current patrol point
    private bool isDetonating = false; // Whether the enemy is in the detonation countdown
    private bool hasExploded = false; // Whether the enemy has exploded
    private bool isChasing = false; // Whether the enemy is chasing the player

    private Rigidbody rb; // Rigidbody component for movement

    // Timer-related variables for managing explosion countdown
    private bool timerStarted = false;
    private float explosionTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to the enemy

        // Find the player object by its tag and assign it to the player variable
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    void Update()
    {
        // If the player is null or the enemy has already exploded, do nothing
        if (player == null || hasExploded) return;

        // Calculate the distance from the enemy to the player
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Handle explosion behavior based on the player's distance from the enemy
        HandleRadiusExplosion(distanceToPlayer);

        // If the player is within detection radius and not detonating, start chasing the player
        if (distanceToPlayer <= detectionRadius && !isDetonating)
        {
            isChasing = true;
            ChasePlayer();
        }
        // If not chasing and not detonating, patrol the waypoints
        else if (!isDetonating)
        {
            isChasing = false;
            Patrol();
        }

        // If the timer has started, decrease the timer and explode if it reaches 0
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
        // Check if the player is within the inner explosion radius
        if (distance <= innerRadius)
        {
            Explode(); // Explode immediately if within inner radius
        }
        // If within middle radius, start the timer and reduce time as the player moves closer
        else if (distance <= middleRadius)
        {
            if (!timerStarted)
            {
                explosionTimer = outerRadiusTimerStart;
                timerStarted = true;
            }
            explosionTimer -= middleRadiusTimeReduction; // Reduce timer when inside middle radius
            explosionTimer = Mathf.Max(0f, explosionTimer); // Clamp timer to prevent negative values
        }
        // If within outer radius, start the timer
        else if (distance <= outerRadius)
        {
            if (!timerStarted)
            {
                explosionTimer = outerRadiusTimerStart;
                timerStarted = true;
            }
        }
        // No action needed if outside all explosion radii
    }

    void Patrol()
    {
        // If no patrol points are defined, do nothing
        if (patrolPoints.Length == 0) return;

        // Move towards the next patrol point
        Transform targetPoint = patrolPoints[currentPointIndex];
        Vector3 direction = (targetPoint.position - transform.position).normalized;
        rb.linearVelocity = direction * patrolSpeed;

        // If close to the current patrol point, move to the next one
        if (Vector3.Distance(transform.position, targetPoint.position) < 0.3f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    void ChasePlayer()
    {
        // If the player reference is null, return
        if (player == null) return;

        // Move towards the player
        Vector3 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * chaseSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        // If already detonating or exploded, do nothing
        if (hasExploded || isDetonating) return;

        // Start detonation countdown if the enemy collides with the player or a javelin
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Javilin"))
        {
            StartCoroutine(DetonationCountdown());
        }
    }

    // Coroutine for handling detonation countdown
    IEnumerator DetonationCountdown()
    {
        isDetonating = true;
        rb.linearVelocity = Vector3.zero; // Stop movement
        yield return new WaitForSeconds(0.2f); // Wait for a short time before exploding
        Explode(); // Trigger explosion
    }

    void Explode()
    {
        // Prevent re-exploding if already exploded
        if (hasExploded) return;

        hasExploded = true;
        Vector3 origin = transform.position;
        HashSet<Collider> alreadyDamaged = new HashSet<Collider>(); // To prevent multiple damage to the same collider

        // Apply damage to the player or objects within the explosion radii
        ApplyExplosionDamage(origin, innerRadius, innerDamage, alreadyDamaged);
        ApplyExplosionDamage(origin, middleRadius, middleDamage, alreadyDamaged);
        ApplyExplosionDamage(origin, outerRadius, outerDamage, alreadyDamaged);

        // Instantiate explosion particle effect
        if (explosionEffect != null)
            Instantiate(explosionEffect, origin, Quaternion.identity);

        // Destroy the enemy game object after explosion
        Destroy(gameObject);
    }

    void ApplyExplosionDamage(Vector3 position, float radius, float damage, HashSet<Collider> alreadyDamaged)
    {
        // Get all colliders within the explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(position, radius);

        foreach (Collider hit in hitColliders)
        {
            // Skip already damaged objects
            if (alreadyDamaged.Contains(hit)) continue;

            // If the object is the player, apply damage
            if (hit.CompareTag("Player"))
            {
                NotPlayerHealth health = hit.GetComponent<NotPlayerHealth>() ?? hit.GetComponentInParent<NotPlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(damage);
                    alreadyDamaged.Add(hit); // Mark this collider as damaged
                }
                else
                {
                    Debug.LogWarning("No NotPlayerHealth found on Player object or parent.");
                }
            }
        }
    }

    // Gizmos for visualizing the enemy's detection and explosion radii in the editor
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

using System.Collections;
using UnityEngine;

public class ExplodingEnemy : MonoBehaviour
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3f;
    public float detectionRadius = 5f;
    public float explosionRadius = 3f;
    public float explosionDamage = 25f;
    public ParticleSystem explosionEffect;

    private Transform player;
    private int currentPointIndex = 0;
    private bool isDetonating = false;
    private bool hasExploded = false;
    private bool isChasing = false;

    private Rigidbody rb;

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
        yield return new WaitForSeconds(0.3f);
        Explode();
    }

    void Explode()
    {
        if (hasExploded) return;

        hasExploded = true;

        // Detect objects within explosion radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                // Try to find NotPlayerHealth on the object or its parent
                NotPlayerHealth playerHealth = hit.GetComponent<NotPlayerHealth>();
                if (playerHealth == null)
                {
                    playerHealth = hit.GetComponentInParent<NotPlayerHealth>();
                }

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(explosionDamage);
                }
                else
                {
                    Debug.LogWarning("No NotPlayerHealth found on Player object or parent.");
                }
            }
        }

        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

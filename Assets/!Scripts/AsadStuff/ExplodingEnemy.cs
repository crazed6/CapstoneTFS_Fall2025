using UnityEngine;
using System.Collections;

public class ExplodingEnemy : MonoBehaviour
{
    public GameObject explosionPrefab; // Particle system explosion
    public AudioClip explosionSound; // Explosion sound
    public float damage = 100f; // Set damage value
    public float explosionRadius = 5f; // Explosion area
    private AudioSource audioSource;

    public NotPlayerHealth playerHealth; // Reference to player health

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.volume = 1.0f;  // Set audio volume

        // Auto-find the player if not assigned in the inspector
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<NotPlayerHealth>();
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left mouse button
        {
            CheckForExplosion();
        }
    }

    void CheckForExplosion()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        Debug.DrawRay(mousePosition, Vector2.zero, Color.red, 1f); // Debug raycast

        if (hit.collider != null && hit.collider.gameObject == gameObject)
        {
            Explode();
        }
    }

    void Explode()
    {
        // Instantiate explosion particle effect
        if (explosionPrefab != null)
        {
            GameObject explosionEffect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosionEffect, 2f); // Destroy the effect after 2 seconds
        }

        // Play explosion sound
        if (explosionSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(explosionSound);
        }

        // Apply damage to the player if within radius
        if (playerHealth != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, playerHealth.transform.position);
            if (distanceToPlayer <= explosionRadius)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Damaging player for " + damage + " damage.");

                if (playerHealth.GetCurrentHealth() <= 0)
                {
                    playerHealth.gameObject.SetActive(false); // Deactivate only if dead
                }
            }
        }

        // Destroy enemy after explosion effect
        StartCoroutine(DestroyAfterDelay());
    }

    IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
}

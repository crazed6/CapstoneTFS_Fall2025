using UnityEngine;
using System.Collections;

public class HeavyEnemyRock : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 3f;
    public float explosionForce = 10f;
    public float explosionUpwardModifier = 0.5f; // upward force -_-
    public float damage = 20f;
    public GameObject explosionEffect;

    [Header("Knockback Settings")]
    public float horizontalKnockbackMultiplier = 1f;
    public float verticalKnockbackMultiplier = 0.5f;

    private Vector3 startPoint;
    private Vector3 targetPoint;
    private Vector3 peakPoint;
    private float flightTime;
    private float elapsedTime = 0;

    public void Launch(Vector3 target, float speed, float height)
    {
        startPoint = transform.position;
        targetPoint = target;
        peakPoint = (startPoint + targetPoint) / 2 + Vector3.up * height;
        flightTime = Vector3.Distance(startPoint, targetPoint) / speed;

        StartCoroutine(FollowArc());
    }

    private IEnumerator FollowArc()
    {
        while (elapsedTime < flightTime)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / flightTime;

            Vector3 newPosition = QuadraticBezier(startPoint, peakPoint, targetPoint, progress);
            transform.position = newPosition;
            yield return null;
        }

        Explode();
    }

    private Vector3 QuadraticBezier(Vector3 start, Vector3 middle, Vector3 end, float t)
    {
        return (1 - t) * (1 - t) * start + 2 * (1 - t) * t * middle + t * t * end;
    }

    private void Explode()
    {
        // Instantiate explosion effect (optional) -_-
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(explosionEffect, 2f);
        }

        // Detect objects in explosion radius -_-
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in hitColliders)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Player hit by rock explosion!");

                //  damage (if player has a health system)
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }

                //  knockback -_-
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    ApplyKnockback(rb, hit.transform.position);
                }
            }
        }

        Destroy(gameObject); // Destroy rock after explosion -_-
    }

    private void ApplyKnockback(Rigidbody rb, Vector3 explosionCenter)
    {
        //  Calculating knockback direction -_-
        Vector3 knockbackDirection = (rb.transform.position - explosionCenter).normalized;

        //  Scale down the vertical (Y) component -_-
        knockbackDirection.y = Mathf.Clamp(knockbackDirection.y, 0.1f, 0.5f);

        //  Adding more force to the horizontal (XZ) push -_-
        Vector3 finalKnockback = new Vector3(knockbackDirection.x, knockbackDirection.y * explosionUpwardModifier, knockbackDirection.z);

        rb.AddForce(finalKnockback * explosionForce, ForceMode.Impulse);
    }


}


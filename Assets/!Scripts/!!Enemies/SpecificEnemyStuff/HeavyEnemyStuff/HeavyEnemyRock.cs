using UnityEngine;
using Cysharp.Threading.Tasks;

public class HeavyEnemyRock : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 3f;
    public float explosionForce = 10f;
    public float explosionUpwardModifier = 0.5f;
    public float damage = 20f;
    public GameObject explosionEffect;

    [Header("Knockback Settings")]
    public float horizontalKnockbackMultiplier = 1f;
    public float verticalKnockbackMultiplier = 0.5f;

    private Vector3 startPoint;
    private Vector3 targetPoint;
    private Vector3 peakPoint;
    private float flightTime;

    public void Launch(Vector3 target, float speed, float height)
    {
        startPoint = transform.position;
        targetPoint = target;
        peakPoint = (startPoint + targetPoint) / 2 + Vector3.up * height;
        flightTime = Vector3.Distance(startPoint, targetPoint) / speed;

        _ = FollowArc(); // fire and forget using UniTask
    }

    private async UniTaskVoid FollowArc()
    {
        float elapsedTime = 0f;

        while (elapsedTime < flightTime)
        {
            //  Defensive: If the object is destroyed, exit gracefully
            if (this == null || transform == null) return;

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / flightTime);

            Vector3 newPosition = QuadraticBezier(startPoint, peakPoint, targetPoint, t);
            transform.position = newPosition;

            await UniTask.Yield(); // Wait until next frame
        }

        //  Extra check before explosion logic
        if (this != null && transform != null)
            Explode();
    }

    private Vector3 QuadraticBezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        return (1 - t) * (1 - t) * a + 2 * (1 - t) * t * b + t * t * c;
    }

    private void Explode()
    {
        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Player hit by rock explosion!");

                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }

                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    ApplyKnockback(rb, hit.transform.position);
                }
            }
        }

        Destroy(gameObject);
    }

    private void ApplyKnockback(Rigidbody rb, Vector3 explosionCenter)
    {
        Vector3 direction = (rb.transform.position - explosionCenter).normalized;
        direction.y = Mathf.Clamp(direction.y, 0.1f, 0.5f);

        Vector3 finalForce = new Vector3(
            direction.x * horizontalKnockbackMultiplier,
            direction.y * explosionUpwardModifier,
            direction.z * horizontalKnockbackMultiplier
        );

        rb.AddForce(finalForce * explosionForce, ForceMode.Impulse);
    }
}

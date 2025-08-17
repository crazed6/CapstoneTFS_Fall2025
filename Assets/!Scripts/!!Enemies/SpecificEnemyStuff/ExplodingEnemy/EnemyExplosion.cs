// EnemyExplosion.cs
// This version uses your original damage and knockback logic.

using System.Collections.Generic;
using UnityEngine;

public class EnemyExplosion : MonoBehaviour
{
    [Header("References")]
    private Transform player;

    [Header("Explosion Settings")]
    public float innerRadius = 1f;
    public float middleRadius = 2f;
    public float outerRadius = 3f;
    public float innerDamage = 50f;
    public float middleDamage = 30f;
    public float outerDamage = 15f;
    public ParticleSystem explosionEffect;
    public LayerMask damageableLayer;

    [Header("Explosion Timer Settings")]
    public float outerRadiusTimerStart = 3f;
    public float middleRadiusTimeReduction = 2f;

    [Header("Knockback Settings")]
    public float knockbackForceX = 10f;
    public float knockbackForceY = 10f;

    [Header("Damage Profiles")]
    public DamageProfile InnerExplosionDamage;
    public DamageProfile MiddleExplosionDamage;
    public DamageProfile OuterExplosionDamage;

    // Private state variables
    private bool hasExploded = false;
    private bool timerStarted = false;
    private float explosionTimer = 0f;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (player == null || hasExploded) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        HandleExplosionTimer(distanceToPlayer);

        if (timerStarted)
        {
            float timerSpeed = (distanceToPlayer < middleRadius) ? middleRadiusTimeReduction : 1;
            explosionTimer -= Time.deltaTime * timerSpeed;

            if (explosionTimer <= 0f)
            {
                Explode();
            }
        }
    }

    void HandleExplosionTimer(float distance)
    {
        if (distance <= innerRadius)
        {
            Explode();
            return;
        }

        if (!timerStarted && distance <= outerRadius)
        {
            timerStarted = true;
            explosionTimer = outerRadiusTimerStart;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Javilin") && !hasExploded)
        {
            Explode();
        }
    }

    public void Explode()
    {
        if (hasExploded) return;
        hasExploded = true;

        Vector3 origin = transform.position;

        // Using your original damage function
        HashSet<Collider> alreadyDamaged = new HashSet<Collider>();
        ApplySingleExplosionDamage(origin, alreadyDamaged);

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, origin, Quaternion.identity);
        }

        Destroy(gameObject);
    }

    // YOUR ORIGINAL FUNCTION: Restored as requested
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
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, innerRadius);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, middleRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, outerRadius);
    }
}
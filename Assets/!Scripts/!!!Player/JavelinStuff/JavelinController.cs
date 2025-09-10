//Ritwik
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.VFX;

public class JavelinController : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 120f;
    public float lifetime = 5f;
    public float aoeRadius = 3f;
    public float damageAmount = 25f;
    public LayerMask damageMask;
    public DamageProfile damageProfile;

    [Header("Collider Delay")]
    public float colliderActivationDelay = 0.1f;

    [Header("VFX")]
    public VisualEffect aoeVFXPrefab;
    public float aoeVFXLifetime = 3f;

    private CancellationTokenSource cts;
    private bool hasExploded = false;
    private float timer = 0f;
    private float activationTimer = 0f;
    private Vector3 direction;
    private Vector3 hitPoint = Vector3.zero;
    private bool isAiming = false;

    public void SetDirection(Vector3 dir)
    {
        direction = dir.normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        // Enable collider & change layer AFTER aiming
        SetAimingMode(false);

        // Begin movement
        cts = new CancellationTokenSource();
        FlyForward(cts.Token).Forget();
    }

    public void SetAimingMode(bool aiming)
    {
        isAiming = aiming;

        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
            col.enabled = !aiming;

        if (!aiming)
            gameObject.layer = LayerMask.NameToLayer("Default");

        // --- NEW: toggle trails ---
        var trailController = GetComponent<JavelinVFXController>();
        if (trailController != null)
        {
            trailController.SetTrailsActive(!aiming);
        }
    }

    private async UniTaskVoid FlyForward(CancellationToken token)
    {
        Vector3 previousPos = transform.position;

        try
        {
            while (timer < lifetime && !hasExploded)
            {
                token.ThrowIfCancellationRequested();
                await UniTask.Yield(PlayerLoopTiming.Update, token);

                float deltaTime = Time.deltaTime;
                activationTimer += deltaTime;
                timer += deltaTime;

                Vector3 newPos = transform.position + direction * speed * deltaTime;

                if (activationTimer >= colliderActivationDelay)
                {
                    float distance = Vector3.Distance(previousPos, newPos);
                    if (Physics.SphereCast(previousPos, 0.1f, direction, out RaycastHit hit, distance, damageMask))
                    {
                        hitPoint = hit.point;
                        TriggerAoE();
                        return;
                    }
                }

                transform.position = newPos;
                previousPos = newPos;
            }

            if (!hasExploded)
            {
                hitPoint = transform.position;
                TriggerAoE();
            }
        }
        catch (OperationCanceledException)
        {
            // Ignore
        }
    }

    private void TriggerAoE()
    {
        hasExploded = true;

        if (cts != null && !cts.IsCancellationRequested)
            cts.Cancel();

        // AoE VFX spawn
        if (aoeVFXPrefab != null)
        {
            VisualEffect vfxInstance = Instantiate(aoeVFXPrefab, hitPoint, Quaternion.identity);

            // Optionally send events/parameters if your VFX Graph expects them
            // vfxInstance.SetFloat("ExplosionRadius", aoeRadius);
            // vfxInstance.SendEvent("OnExplode");

            Destroy(vfxInstance.gameObject, aoeVFXLifetime);
        }

        // Damage logic unchanged
        Collider[] hits = Physics.OverlapSphere(hitPoint, aoeRadius, damageMask);
        foreach (var hit in hits)
        {
            EnemyDamageComponent damageable = hit.GetComponentInParent<EnemyDamageComponent>();
            if (damageable != null)
            {
                DamageData data = new DamageData(gameObject, damageProfile)
                {
                    source = gameObject,
                    profile = damageProfile
                };

                damageable.TakeDamage2(data);
            }
        }

        Destroy(gameObject);
    }


    private void OnDestroy()
    {
        if (cts != null && !cts.IsCancellationRequested)
            cts.Cancel();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawSphere(hitPoint == Vector3.zero ? transform.position : hitPoint, aoeRadius);
    }
}

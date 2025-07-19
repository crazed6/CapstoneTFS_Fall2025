//Ritwik
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class JavelinController : MonoBehaviour
{
    [Header("Arc Settings")]
    public float speed = 90f;
    public float gravityStrength = 30f;
    public float lifetime = 5f;

    [Header("Rotation")]
    public float rotationSpeed = 720f;

    [Header("AoE Settings")]
    public float aoeRadius = 3f;
    public float damageAmount = 25f;
    public LayerMask damageMask;

    [Header("Damage Profile")]
    public DamageProfile damageProfile; // Assign your JavelinDamage in Inspector

    private Vector3 velocity;
    private bool isFlying = false;
    private bool hasExploded = false;
    private Vector3 hitPoint = Vector3.zero;

    private CancellationTokenSource cts;

    public void SetDirection(Vector3 direction)
    {
        velocity = direction.normalized * speed;
        transform.rotation = Quaternion.LookRotation(velocity);
        isFlying = true;

        cts = new CancellationTokenSource();
        MoveAlongArc(cts.Token).Forget();
    }

    private async UniTaskVoid MoveAlongArc(CancellationToken token)
    {
        float timer = 0f;

        try
        {
            while (timer < lifetime && !hasExploded)
            {
                token.ThrowIfCancellationRequested();

                await UniTask.Yield(PlayerLoopTiming.Update, token);

                velocity += Vector3.down * gravityStrength * Time.deltaTime;
                transform.position += velocity * Time.deltaTime;

                if (velocity != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(velocity);

                transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);

                timer += Time.deltaTime;
            }

            if (!hasExploded)
            {
                hitPoint = transform.position;
                TriggerAoE();
            }
        }
        catch (OperationCanceledException)
        {
            // Safe exit
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!hasExploded)
        {
            hitPoint = transform.position;
            TriggerAoE();
        }
    }

    private void TriggerAoE()
    {
        hasExploded = true;

        if (cts != null && !cts.IsCancellationRequested)
            cts.Cancel();

        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius, damageMask);

        foreach (var hit in hits)
        {
            EnemyDamageComponent damageable = hit.GetComponent<EnemyDamageComponent>();
            if (damageable != null)
            {
                DamageData data = new DamageData
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

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.4f);
        Gizmos.DrawSphere(hitPoint == Vector3.zero ? transform.position : hitPoint, aoeRadius);
    }
}

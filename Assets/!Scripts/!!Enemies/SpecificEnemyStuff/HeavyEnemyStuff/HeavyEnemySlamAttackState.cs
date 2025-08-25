using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class HeavyEnemySlamAttackState : IHeavyEnemyState
{
    private readonly HeavyEnemyAI enemy;
    private CancellationTokenSource slamTokenSource;

    // Small telegraph/impact delay so we check dash at the exact moment we apply damage.
    // Tune this to taste (0.10f–0.25f usually feels good).
    private const float HitDelaySeconds = 0.15f;

    public HeavyEnemySlamAttackState(HeavyEnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("Entered Slam Attack State");
        enemy.StopTracking();                 // cancel rock logic
        enemy.isSlamming = true;
        slamTokenSource = new CancellationTokenSource();
        SlamAttackAsync().Forget();
    }

    public void Execute()
    {
        // No per-frame logic; handled asynchronously.
    }

    public void Exit()
    {
        Debug.Log("Exiting Slam Attack State");
        enemy.isSlamming = false;
        slamTokenSource?.Cancel();
    }

    private async UniTaskVoid SlamAttackAsync()
    {
        // 1) Optional: play windup animation/VFX here

        try
        {
            // 2) Wait a small delay and THEN apply the AoE (dash flag is checked at impact time)
            await UniTask.Delay(TimeSpan.FromSeconds(HitDelaySeconds), cancellationToken: slamTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            return; // state changed / enemy destroyed
        }

        if (enemy == null || !enemy.gameObject.activeInHierarchy) return;

        // 3) Apply AOE at impact time (we re-check dash here)
        ApplySlamAOE();

        // 4) Start cooldown
        enemy.StartSlamCooldown().Forget();

        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(enemy.slamCooldown), cancellationToken: slamTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            // safely cancelled
        }

        if (enemy == null || !enemy.gameObject.activeInHierarchy) return;

        // 5) Return to next appropriate state
        if (enemy.CanSeePlayer())
            enemy.stateMachine.ChangeState(new HeavyEnemyShootingState(enemy));
        else
            enemy.stateMachine.ChangeState(new HeavyEnemyIdleState(enemy));
    }

    private void ApplySlamAOE()
    {
        // Guard enemy references
        if (enemy == null || enemy.slamOrigin == null) return;

        enemy.OnSlam?.Invoke(); // <-- Audio triggers here

        Collider[] hits = Physics.OverlapSphere(enemy.slamOrigin.position, enemy.slamRadius);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            // --- Robustly fetch your custom CharacterController no matter where the collider lives ---
            // Use global::CharacterController to avoid clashing with UnityEngine.CharacterController.
            global::CharacterController controller =
                hit.GetComponent<global::CharacterController>() ??
                hit.GetComponentInParent<global::CharacterController>() ??
                (hit.attachedRigidbody != null ? hit.attachedRigidbody.GetComponent<global::CharacterController>() : null);

            // If the player is in ANY dash mode at impact time, skip damage & knockback.
            if (enemy.LastDashedPlayer == hit.gameObject)
            {
                
                continue;
            }

            // --- Damage ---
            Health playerHealth =
                hit.GetComponent<Health>() ??
                hit.GetComponentInParent<Health>() ??
                (hit.attachedRigidbody != null ? hit.attachedRigidbody.GetComponent<Health>() : null);

            if (playerHealth != null && enemy.GroundSlam != null)
            {
                DamageData damageData = new DamageData(enemy.gameObject, enemy.GroundSlam);
                playerHealth.PlayerTakeDamage(damageData);
            }

            // --- Knockback ---
            KnockbackReceiver kb =
                hit.GetComponent<KnockbackReceiver>() ??
                hit.GetComponentInParent<KnockbackReceiver>() ??
                (hit.attachedRigidbody != null ? hit.attachedRigidbody.GetComponent<KnockbackReceiver>() : null);

            if (kb != null)
            {
                KnockbackData kbData = new KnockbackData(
                    source: enemy.slamOrigin.position,
                    force: enemy.slamKnockbackForce,
                    duration: 0.35f,
                    upwardForce: 0.6f,
                    overrideVel: true
                );
                kb.ApplyKnockback(kbData);
            }
        }
    }
}
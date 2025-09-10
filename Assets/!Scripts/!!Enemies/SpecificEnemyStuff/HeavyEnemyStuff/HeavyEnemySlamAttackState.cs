// HeavyEnemySlamAttackState.cs
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class HeavyEnemySlamAttackState : IHeavyEnemyState
{
    private readonly HeavyEnemyAI enemy;
    private CancellationTokenSource stateTransitionTokenSource;

    public HeavyEnemySlamAttackState(HeavyEnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("SLAM STATE ENTERED! If the animation isn't playing, check the Animator transition.");

        Debug.Log("Entered Slam Attack State");
        enemy.StopTracking();
        enemy.isSlamming = true;
        stateTransitionTokenSource = new CancellationTokenSource();

        // Trigger the slam animation!
        if (enemy.animator != null)
        {
            enemy.animator.SetTrigger("LobberSlam");
        }
    }

    public void Execute()
    {
        // No per-frame logic; everything is driven by the animation now.
    }

    public void Exit()
    {
        Debug.Log("Exiting Slam Attack State");
        enemy.isSlamming = false;
        stateTransitionTokenSource?.Cancel();
    }

    // THIS FUNCTION IS NOW PUBLIC and will be called by the animation event
    public void ApplySlamAOE()
    {
        // Guard enemy references
        if (enemy == null || enemy.slamOrigin == null) return;

        Debug.Log("Animation Event triggered ApplySlamAOE!");
        enemy.OnSlam?.Invoke(); // <-- Audio triggers here

        Collider[] hits = Physics.OverlapSphere(enemy.slamOrigin.position, enemy.slamRadius);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;

            // ... (The rest of your existing damage and knockback logic remains unchanged)
            global::CharacterController controller =
                hit.GetComponent<global::CharacterController>() ??
                hit.GetComponentInParent<global::CharacterController>() ??
                (hit.attachedRigidbody != null ? hit.attachedRigidbody.GetComponent<global::CharacterController>() : null);

            if (enemy.LastDashedPlayer == hit.gameObject) continue;

            Health playerHealth =
                hit.GetComponent<Health>() ??
                hit.GetComponentInParent<Health>() ??
                (hit.attachedRigidbody != null ? hit.attachedRigidbody.GetComponent<Health>() : null);

            if (playerHealth != null && enemy.GroundSlam != null)
            {
                DamageData damageData = new DamageData(enemy.gameObject, enemy.GroundSlam);
                playerHealth.PlayerTakeDamage(damageData);
            }

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

        // After applying damage, start the cooldown and prepare to transition to the next state.
        enemy.StartSlamCooldown().Forget();
        TransitionAfterSlamAsync().Forget();
    }

    private async UniTaskVoid TransitionAfterSlamAsync()
    {
        try
        {
            // Wait for a short duration after the slam to let the animation finish blending out
            await UniTask.Delay(TimeSpan.FromSeconds(1.0f), cancellationToken: stateTransitionTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            return; // State was changed by something else
        }

        if (enemy == null || !enemy.gameObject.activeInHierarchy) return;

        // Transition to the next appropriate state
        if (enemy.CanSeePlayer())
            enemy.stateMachine.ChangeState(new HeavyEnemyShootingState(enemy));
        else
            enemy.stateMachine.ChangeState(new HeavyEnemyIdleState(enemy));
    }
}
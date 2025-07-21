using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class HeavyEnemySlamAttackState : IHeavyEnemyState
{
    private HeavyEnemyAI enemy;
    private bool slamInProgress = false;
    private CancellationTokenSource slamTokenSource;

    public HeavyEnemySlamAttackState(HeavyEnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("Entered Slam Attack State");

        enemy.StopTracking(); // Cancel rock logic
        enemy.isSlamming = true;
        slamTokenSource = new CancellationTokenSource();

        SlamAttackAsync().Forget(); // Start the async slam
    }

    public void Execute()
    {
        // Nothing here – slam is async driven
    }

    public void Exit()
    {
        Debug.Log("Exiting Slam Attack State");

        enemy.isSlamming = false;
        slamTokenSource?.Cancel(); // Cancel delay safely
    }

    private async UniTaskVoid SlamAttackAsync()
    {
        slamInProgress = true;

        ApplySlamAOE();

        // Start cooldown timer on enemy
        enemy.StartSlamCooldown().Forget();

        try
        {
            await UniTask.Delay((int)(enemy.slamCooldown * 1000f), cancellationToken: slamTokenSource.Token);
        }
        catch (OperationCanceledException)
        {
            Debug.Log("Slam Attack was cancelled early.");
        }

        slamInProgress = false;

        // Double-check enemy state before continuing
        if (enemy == null || !enemy.gameObject.activeInHierarchy) return;

        if (enemy.CanSeePlayer())
            enemy.stateMachine.ChangeState(new HeavyEnemyShootingState(enemy));
        else
            enemy.stateMachine.ChangeState(new HeavyEnemyIdleState(enemy));
    }

    private void ApplySlamAOE()
    {
        Collider[] hits = Physics.OverlapSphere(enemy.slamOrigin.position, enemy.slamRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Slam hit the player!");

                Health playerHealth = hit.GetComponent<Health>();
                if (playerHealth != null && enemy.GroundSlam != null)
                {
                    DamageData damageData = new DamageData(enemy.gameObject, enemy.GroundSlam);
                    playerHealth.PlayerTakeDamage(damageData);
                }

                // Knockback
                KnockbackReceiver kb = hit.GetComponent<KnockbackReceiver>();
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
}

using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class HeavyEnemySlamAttackState : IHeavyEnemyState
{
    private HeavyEnemyAI enemy;
    private bool slamExecuted = false;
    private CancellationTokenSource slamTokenSource;


    public HeavyEnemySlamAttackState(HeavyEnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("Entered Slam Attack State");

        enemy.StopTracking(); // ?? Kill all tracking and rock logic
        enemy.isSlamming = true;
        slamTokenSource = new CancellationTokenSource();

        SlamAttackAsync().Forget(); // Start slam logic
    }

    public void Execute()
    {
        // Nothing needed, handled asynchronously
    }

    public void Exit()
    {
        Debug.Log("Exiting Slam Attack State");
        enemy.isSlamming = false;
        slamTokenSource?.Cancel();
    }

    private async UniTaskVoid SlamAttackAsync()
    {
        // Add optional delay or animation trigger if needed later
        ApplySlamAOE();

        await UniTask.Delay((int)(enemy.slamCooldown * 1000f), cancellationToken: slamTokenSource.Token);

        if (enemy != null && enemy.CanSeePlayer())
        {
            enemy.stateMachine.ChangeState(new HeavyEnemyShootingState(enemy));
        }
        else
        {
            enemy.stateMachine.ChangeState(new HeavyEnemyIdleState(enemy));
        }
    }

    private void ApplySlamAOE()
    {
        Collider[] hits = Physics.OverlapSphere(enemy.slamOrigin.position, enemy.slamRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Slam hit the player!");

                // Damage
                //PlayerHealth health = hit.GetComponent<PlayerHealth>();
                //health?.TakeDamage(enemy.slamDamage);

                //Josh script, ensure to attach RockShoot Damage Profile in inspector, on Rock script
                Health playerHealth = hit.GetComponent<Health>();
                if (playerHealth != null && enemy.GroundSlam != null)
                {
                    DamageData damageData = new DamageData(enemy.gameObject, enemy.GroundSlam);
                    playerHealth.PlayerTakeDamage(damageData);
                }
                //Josh script end


                // Knockback
                Rigidbody rb = hit.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 forceDir = (hit.transform.position - enemy.transform.position).normalized;
                    rb.AddForce(forceDir * enemy.slamKnockbackForce, ForceMode.Impulse);
                }
            }
        }
    }
}

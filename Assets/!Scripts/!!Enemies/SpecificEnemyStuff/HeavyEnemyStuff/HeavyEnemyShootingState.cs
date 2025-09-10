using UnityEngine;

public class HeavyEnemyShootingState : IHeavyEnemyState
{
    private readonly HeavyEnemyAI enemy;

    public HeavyEnemyShootingState(HeavyEnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("Entered Shooting State");
        enemy.StartTracking(); // Begin aiming and rock throw
    }

    public void Execute()
    {
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.position);

        // Check for Slam opportunity only if not on cooldown
        if (distanceToPlayer <= enemy.slamRadius && !enemy.slamOnCooldown)
        {
            enemy.stateMachine.ChangeState(new HeavyEnemySlamAttackState(enemy));
            return;
        }

        // Exit to Idle if player is out of sight
        if (distanceToPlayer > enemy.firingZoneRange || !enemy.CanSeePlayer())
        {
            Debug.Log("Player left the firing zone, stopping attack...");
            enemy.StopTracking();
            enemy.stateMachine.ChangeState(new HeavyEnemyIdleState(enemy));
            return;
        }

        // Face the player while shooting
        Vector3 direction = (enemy.player.position - enemy.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        enemy.transform.rotation = Quaternion.Slerp(
            enemy.transform.rotation,
            lookRotation,
            Time.deltaTime * enemy.rotationSpeed
        );
    }

    public void Exit()
    {
        Debug.Log("Exiting Shooting State");
    }
}

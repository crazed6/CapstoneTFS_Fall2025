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

        // 1. Tell the animator to play the shooting animation loop
        if (enemy.animator != null)
        {
            enemy.animator.SetBool("isShooting", true);
        }

        // 2. Start the aiming process
        enemy.AimAndLockTarget();
    }

    public void Execute()
    {
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.position);

        // Check for Slam opportunity
        if (distanceToPlayer <= enemy.slamRadius && !enemy.slamOnCooldown)
        {
            enemy.stateMachine.ChangeState(new HeavyEnemySlamAttackState(enemy));
            return;
        }

        // Exit to Idle if player is out of sight (and we are not currently aiming)
        if ((distanceToPlayer > enemy.firingZoneRange || !enemy.CanSeePlayer()) && !enemy.isTracking)
        {
            enemy.StopTracking();
            enemy.stateMachine.ChangeState(new HeavyEnemyIdleState(enemy));
            return;
        }

        // Face the player while in this state
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

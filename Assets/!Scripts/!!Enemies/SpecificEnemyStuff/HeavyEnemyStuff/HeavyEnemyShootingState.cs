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
        enemy.StartTracking(); // Starts async tracking sequence
    }

    public void Execute()
    {
        if (Vector3.Distance(enemy.transform.position, enemy.player.position) <= enemy.slamRadius)
        {
            enemy.stateMachine.ChangeState(new HeavyEnemySlamAttackState(enemy));
            return;
        }

        float playerDistance = Vector3.Distance(enemy.transform.position, enemy.player.position);


        if (playerDistance > enemy.firingZoneRange || !enemy.CanSeePlayer())
        {
            Debug.Log("Player left the firing zone, stopping attack...");
            enemy.StopTracking();
            enemy.stateMachine.ChangeState(new HeavyEnemyIdleState(enemy));
            return;
        }

        

        // Rotate to face player
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

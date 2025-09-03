using UnityEngine;

public class HeavyEnemyIdleState : IHeavyEnemyState
{
    private readonly HeavyEnemyAI enemy;

    public HeavyEnemyIdleState(HeavyEnemyAI enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log("Entered Idle State");
        enemy.StopTracking(); // Stops async process if running
       
        if (enemy.animator != null)
        {
            enemy.animator.SetBool("isShooting", false);
        }
    }

    public void Execute()
    {
        float playerDistance = Vector3.Distance(enemy.transform.position, enemy.player.position);

        if (playerDistance <= enemy.detectionRange && enemy.CanSeePlayer())
        {
            Debug.Log("Player detected! Switching to Shooting State.");
            enemy.stateMachine.ChangeState(new HeavyEnemyShootingState(enemy));
        }
        else
        {
            enemy.transform.rotation = Quaternion.Slerp(
                enemy.transform.rotation,
                enemy.originalRotation,
                Time.deltaTime * enemy.rotationSpeed
            );
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}

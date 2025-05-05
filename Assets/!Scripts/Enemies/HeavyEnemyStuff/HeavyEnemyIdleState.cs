using UnityEngine;

public class HeavyEnemyIdleState : IHeavyEnemyState
{
    private HeavyEnemyAI enemy;

    public HeavyEnemyIdleState(HeavyEnemyAI enemy) { this.enemy = enemy; }

    public void Enter()
    {
        Debug.Log("Entered Idle State");
        enemy.StopTracking();
    }

    public void Execute()
    {
        float playerDistance = Vector3.Distance(enemy.transform.position, enemy.player.position);

        //  Only detect player if they are inside `detectionRange` again -_-
        if (playerDistance <= enemy.detectionRange && enemy.CanSeePlayer())
        {
            Debug.Log("Player detected! Switching to Shooting State.");
            enemy.stateMachine.ChangeState(new HeavyEnemyShootingState(enemy));
        }
        else
        {
            //  Rotate back to original rotation -_-
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, enemy.originalRotation, Time.deltaTime * enemy.rotationSpeed);
        }
    }


    public void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}

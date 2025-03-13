using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class HeavyEnemyShootingState : IHeavyEnemyState
{
    private HeavyEnemyAI enemy;

    public HeavyEnemyShootingState(HeavyEnemyAI enemy) { this.enemy = enemy; }

    public void Enter()
    {
        Debug.Log("Entered Shooting State");
        enemy.StartTracking();
    }

    public void Execute()
    {
        float playerDistance = Vector3.Distance(enemy.transform.position, enemy.player.position);

        //  Player must be inside the firing zone to keep throwing rocks for you guessed it sins -_-
        if (playerDistance > enemy.firingZoneRange || !enemy.CanSeePlayer())
        {
            Debug.Log("Player left the firing zone, stopping attack...");
            enemy.StopTracking();  //  Stop trajectory and shooting immediately -_-
            enemy.stateMachine.ChangeState(new HeavyEnemyIdleState(enemy));
            return;
        }
        // Rotate enemy to face the player -_-
        Vector3 direction = (enemy.player.position - enemy.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, lookRotation, Time.deltaTime * enemy.rotationSpeed);
    }


    public void Exit()
    {
        Debug.Log("Exiting Shooting State");
    }
}

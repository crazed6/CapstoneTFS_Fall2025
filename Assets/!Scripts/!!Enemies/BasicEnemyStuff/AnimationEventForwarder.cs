// AnimationEventForwarder.cs
using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    private HeavyEnemyAI heavyEnemyAI;
    private WorkerAI workerAI; // <-- Add this line

    // This runs when the object first becomes active
    void Awake()
    {
        // Find the HeavyEnemyAI script in this object's parent
        heavyEnemyAI = GetComponentInParent<HeavyEnemyAI>();
        // Find the WorkerAI script in this object's parent
        workerAI = GetComponentInParent<WorkerAI>(); // <-- Add this line
    }

    #region Worker Forward Events

    // This is the public function our Worker's Animation Event will call
    public void TriggerWorkerShoot()
    {
        // Tell the main AI script to handle the shooting logic
        workerAI?.HandleShootTrigger();
    }

    #endregion


    #region Lobber Forward Events
    public void TriggerRockThrow()
    {
        heavyEnemyAI?.HandleRockThrowTrigger();
    }

    public void TriggerRockThrowChargeUpVFX()
    {
        heavyEnemyAI?.GetComponentInChildren<LobberVFXController>()?.PlayRockThrowChargeUpVFX();
    }

    public void StopRockThrowChargeUpVFX()
    {
        heavyEnemyAI?.GetComponentInChildren<LobberVFXController>()?.StopRockThrowChargeUpVFX();
    }

    // This is the public function our Animation Event will call
    public void TriggerSlamAOE()
    {
        // Tell the main AI script to trigger the slam damage
        heavyEnemyAI?.TriggerSlamAOE();
    }
    public void TriggerSlamVFX()
    {
        heavyEnemyAI?.GetComponentInChildren<LobberVFXController>()?.PlaySlamVFX();
    }

    #endregion
}
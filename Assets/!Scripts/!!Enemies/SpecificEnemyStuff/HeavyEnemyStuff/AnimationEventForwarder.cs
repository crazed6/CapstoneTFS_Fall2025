// AnimationEventForwarder.cs
using UnityEngine;

public class AnimationEventForwarder : MonoBehaviour
{
    private HeavyEnemyAI heavyEnemyAI;

    // This runs when the object first becomes active
    void Awake()
    {
        // Find the HeavyEnemyAI script in this object's parent
        heavyEnemyAI = GetComponentInParent<HeavyEnemyAI>();
    }

    public void TriggerRockThrow()
    {
        heavyEnemyAI?.HandleRockThrowTrigger();
    }

    public void TriggerRockThrowChargeUpVFX()
    {
        heavyEnemyAI?.GetComponentInChildren<EnemyVFXController>()?.PlayRockThrowChargeUpVFX();
    }

    public void StopRockThrowChargeUpVFX()
    {
        heavyEnemyAI?.GetComponentInChildren<EnemyVFXController>()?.StopRockThrowChargeUpVFX();
    }

    // This is the public function our Animation Event will call
    public void TriggerSlamAOE()
    {
        // Tell the main AI script to trigger the slam damage
        heavyEnemyAI?.TriggerSlamAOE();
    }
    public void TriggerSlamVFX()
    {
        heavyEnemyAI?.GetComponentInChildren<EnemyVFXController>()?.PlaySlamVFX();
    }
}
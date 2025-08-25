using UnityEngine;

public class WorkerAudio : MonoBehaviour
{
    private WorkerAI workerAI;
    private EnemyDamageComponent damageComponent;

    void Awake()
    {
        workerAI = GetComponent<WorkerAI>();
        damageComponent = GetComponent<EnemyDamageComponent>();
    }

    void OnEnable()
    {
        if (damageComponent != null)
        {
            damageComponent.OnDamaged += PlayDamagedSFX;
            damageComponent.OnDied += PlayDeathSFX;
            ProjectileScript.OnAnyProjectileHit += HandleProjectileHit;
        }
    }

    void OnDisable()
    {
        if (damageComponent != null)
        {
            damageComponent.OnDamaged -= PlayDamagedSFX;
            damageComponent.OnDied -= PlayDeathSFX;
            ProjectileScript.OnAnyProjectileHit -= HandleProjectileHit;
        }
    }

    public void PlayShootSFX()
    {
        AudioManager.instance.PlaySFX("WorkerLaser");
    }

    private void HandleProjectileHit(ProjectileScript proj, Collider target)
    {
        AudioManager.instance.PlaySFX("WorkerLaserHit");
    }

    private void PlayDamagedSFX()
    {
        AudioManager.instance.PlaySFX("WorkerInjured");
    }

    private void PlayDeathSFX()
    {
        AudioManager.instance.PlaySFX("WorkerDeath");
    }
}

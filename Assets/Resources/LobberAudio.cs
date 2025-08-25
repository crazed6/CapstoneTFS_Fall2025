using UnityEngine;

public class LobberAudio : MonoBehaviour
{
    private HeavyEnemyAI enemy;

    void Awake()
    {
        enemy = GetComponent<HeavyEnemyAI>();
    }

    void OnEnable()
    {
        if (enemy != null)
            enemy.OnSlam += PlaySlamSFX;
            enemy.OnThrowRock += PlayThrowSFX;
            enemy.OnLockOn += PlayLockOnSFX;
            enemy.OnStopTracking += StopLockOnSFX;  // stop audio when tracking sto
    }

    void OnDisable()
    {
        if (enemy != null)
            enemy.OnSlam -= PlaySlamSFX;
            enemy.OnThrowRock -= PlayThrowSFX;
            enemy.OnLockOn -= PlayLockOnSFX;
            enemy.OnStopTracking -= StopLockOnSFX;
    }

    private void PlaySlamSFX()
    {
        AudioManager.instance.PlaySFX("LobberSmash");
    }

    public void PlayThrowSFX()
    {
        AudioManager.instance.PlaySFX("LobberProj.Woosh");
    }

    public void PlayLockOnSFX()
    {
        AudioManager.instance.PlaySFX("LobberCharge");
    }

    private void StopLockOnSFX()
    {
        AudioManager.instance.StopSFX("LobberCharge");
    }
}
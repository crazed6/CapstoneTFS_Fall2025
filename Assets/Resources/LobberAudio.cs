using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class LobberAudio : MonoBehaviour
{
    private HeavyEnemyAI enemy;
    private EnemyDamageComponent damageComponent;
    private AudioSource audioSource;

    [Header("Audio Clips & Volumes")]
    public string slamClip = "LobberSmash";
    [Range(0f, 2f)] public float slamVolume = 1f;

    public string throwClip = "LobberProj.Woosh";
    [Range(0f, 2f)] public float throwVolume = 1f;

    public string lockOnClip = "LobberCharge";
    [Range(0f, 2f)] public float lockOnVolume = 1f;

    public string deathClip = "LobberDeath";    // 🔥 New death SFX key
    [Range(0f, 2f)] public float deathVolume = 1f;

    void Awake()
    {
        enemy = GetComponent<HeavyEnemyAI>();
        damageComponent = GetComponent<EnemyDamageComponent>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        if (enemy != null)
        {
            enemy.OnSlam += PlaySlamSFX;
            enemy.OnThrowRock += PlayThrowSFX;
            enemy.OnLockOn += PlayLockOnSFX;
            enemy.OnStopTracking += StopLockOnSFX;
        }

        if (damageComponent != null)
        {
            damageComponent.OnDeath += PlayDeathSFX;
        }
    }

    void OnDisable()
    {
        if (enemy != null)
        {
            enemy.OnSlam -= PlaySlamSFX;
            enemy.OnThrowRock -= PlayThrowSFX;
            enemy.OnLockOn -= PlayLockOnSFX;
            enemy.OnStopTracking -= StopLockOnSFX;
        }

        if (damageComponent != null)
        {
            damageComponent.OnDeath -= PlayDeathSFX;
        }
    }

    private void PlaySlamSFX()
    {
        PlayClip(slamClip, slamVolume);
    }

    public void PlayThrowSFX()
    {
        PlayClip(throwClip, throwVolume);
    }

    public void PlayLockOnSFX()
    {
        PlayClip(lockOnClip, lockOnVolume);
    }

    private void StopLockOnSFX()
    {
        audioSource.Stop();
    }

    private void PlayDeathSFX()
    {
        PlayClip(deathClip, deathVolume);
    }

    private void PlayClip(string clipKey, float volume)
    {
        if (AudioManager.instance == null) return;

        AudioClip clip = AudioManager.instance.GetClipByName(clipKey);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogWarning($"[LobberAudio] Missing audio clip: {clipKey}");
        }
    }
}

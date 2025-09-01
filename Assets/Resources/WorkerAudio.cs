using UnityEngine;

[System.Serializable]
public class WorkerSound
{
    public string clipKey;   // The AudioManager key (e.g., "WorkerLaser")
    [Range(0f, 2f)] public float volume = 1f; // Volume multiplier
}

public class WorkerAudio : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Worker Sounds")]
    public WorkerSound shootSound;   // Shooting projectile
    public WorkerSound deathSound;   // Dying
    public WorkerSound hitSound;     // Worker takes damage
    public WorkerSound impactSound;  // Projectile impact

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void PlayClip(WorkerSound sound)
    {
        if (AudioManager.instance == null || sound == null) return;

        AudioClip clip = AudioManager.instance.GetClipByName(sound.clipKey);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, sound.volume);
        }
        else
        {
            Debug.LogWarning($"[WorkerAudio] Missing audio clip: {sound.clipKey}");
        }
    }

    // 🔫 Call when worker shoots
    public void PlayShoot()
    {
        PlayClip(shootSound);
    }

    // 💀 Call when worker dies
    public void PlayDeath()
    {
        PlayClip(deathSound);
    }

    // 🤕 Call when worker takes damage
    public void PlayHit()
    {
        PlayClip(hitSound);
    }

    // 💥 Call when projectile impacts something
    public void PlayImpact()
    {
        PlayClip(impactSound);
    }
}

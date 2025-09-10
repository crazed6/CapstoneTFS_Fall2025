using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ExploderAudio : MonoBehaviour
{
    private ExplodingEnemy enemy;
    private AudioSource audioSource;

    [Header("Audio Clips & Volumes")]
    public string idleClip = "ExploderIdle";
    [Range(0f, 2f)] public float idleVolume = 1f;

    public string detectClip = "ExploderDetect";
    [Range(0f, 2f)] public float detectVolume = 1f;

    public string explosionClip = "ExploderExplosion";
    [Range(0f, 2f)] public float explosionVolume = 1f;

    private bool isDetecting = false;

    void Awake()
    {
        enemy = GetComponent<ExplodingEnemy>();

        // Create dedicated 3D audio source
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 0.6f; // fully 3D
        audioSource.playOnAwake = false;
        audioSource.loop = false;
    }

    void Update()
    {
        if (enemy == null) return;

        float distanceToPlayer = enemy.GetPlayerDistance();

        // Detection vs Idle logic
        if (distanceToPlayer <= enemy.detectionRadius && !isDetecting)
        {
            PlayLoop(detectClip, detectVolume);
            isDetecting = true;
        }
        else if (distanceToPlayer > enemy.detectionRadius && isDetecting)
        {
            PlayLoop(idleClip, idleVolume);
            isDetecting = false;
        }
    }

    public void PlayExplosion()
    {
        PlayOneShot(explosionClip, explosionVolume);
    }

    private void PlayLoop(string clipKey, float volume)
    {
        if (AudioManager.instance == null) return;

        AudioClip clip = AudioManager.instance.GetClipByName(clipKey);
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.volume = volume;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning($"[ExploderAudio] Missing loop clip: {clipKey}");
        }
    }

    private void PlayOneShot(string clipKey, float volume)
    {
        if (AudioManager.instance == null) return;

        AudioClip clip = AudioManager.instance.GetClipByName(clipKey);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogWarning($"[ExploderAudio] Missing oneshot clip: {clipKey}");
        }
    }
}

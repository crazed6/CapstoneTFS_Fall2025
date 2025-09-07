using UnityEngine;

[RequireComponent(typeof(AudioSource))] // Ensures an AudioSource is always attached
public class ExploderAudio : MonoBehaviour
{
    private ExplodingEnemy enemy; // Reference to the Exploder enemy logic
    private AudioSource audioSource; // Local AudioSource for playing sounds

    [Header("Audio Clips & Volumes")]
    public string idleClip = "ExploderIdle"; // Key for idle hum SFX
    [Range(0f, 2f)] public float idleVolume = 1f; // Volume control for idle hum

    public string detectClip = "ExploderDetect"; // Key for detection/beep SFX
    [Range(0f, 4f)] public float detectVolume = 1f; // Volume control for detection

    public string explosionClip = "ExploderExplosion"; // Key for explosion SFX
    [Range(0f, 5f)] public float explosionVolume = 1f; // Volume control for explosion

    [Header("Hum Range Trigger")]
    public SphereCollider humRange; // Trigger area for idle hum (assign in Inspector)

    private bool isDetecting = false; // Tracks if detect beep is currently active
    private bool isPlayerInHumRange = false; // Tracks if player is inside hum radius

    void Awake()
    {
        // Cache references
        enemy = GetComponent<ExplodingEnemy>();
        audioSource = GetComponent<AudioSource>();

        // Make sure the hum range collider acts as a trigger
        if (humRange != null)
            humRange.isTrigger = true;
    }

    void Update()
    {
        if (enemy == null) return;

        // Ask the enemy how far the player is
        float distanceToPlayer = enemy.GetPlayerDistance();

        // 🔔 Play detect beep if player enters detection radius
        if (distanceToPlayer <= enemy.detectionRadius && !isDetecting)
        {
            PlayOneShot(detectClip, detectVolume);
            isDetecting = true;
        }
        // Stop beep if player leaves detection radius
        else if (distanceToPlayer > enemy.detectionRadius && isDetecting)
        {
            AudioManager.instance.StopSFX(detectClip);
            isDetecting = false;
        }
    }

    // 🎵 Handles idle hum when the player enters the trigger area
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPlayerInHumRange)
        {
            PlayLoop(idleClip, idleVolume);
            isPlayerInHumRange = true;
        }
    }

    // 🎵 Stop idle hum when the player leaves the trigger area
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isPlayerInHumRange)
        {
            StopLoop();
            isPlayerInHumRange = false;
        }
    }

    // 💥 Play explosion SFX (called from ExplodingEnemy on explosion)
    public void PlayExplosion()
    {
        AudioClip clip = AudioManager.instance.GetClipByName(explosionClip);
        if (clip != null)
        {
            // create a temp object at exploder’s position
            GameObject temp = new GameObject("TempExplosionAudio");
            temp.transform.position = transform.position;

            // configure audio
            AudioSource tempSource = temp.AddComponent<AudioSource>();
            tempSource.clip = clip;
            tempSource.volume = explosionVolume;
            tempSource.spatialBlend = 0.6f;   
            tempSource.minDistance = 10f;   // 🔥 boost loudness nearby
            tempSource.maxDistance = 50f;   // fade out naturally
            tempSource.Play();

            // cleanup
            Destroy(temp, clip.length);
        }
    }


    // === Helper Methods ===

    // Plays a looping sound (idle hum)
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
    }

    // Stops any current looping sound
    private void StopLoop()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

    // Plays a one-shot sound (detect beep or explosion)
    private void PlayOneShot(string clipKey, float volume)
    {
        if (AudioManager.instance == null) return;

        AudioClip clip = AudioManager.instance.GetClipByName(clipKey);
        if (clip != null)
            audioSource.PlayOneShot(clip, volume);
    }
}

using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CrashedShipAudioZone : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioClip sfx1;
    [Range(0f, 1f)] public float volume1 = 1f;

    public AudioClip sfx2;
    [Range(0f, 1f)] public float volume2 = 1f;

    private AudioSource source1;
    private AudioSource source2;

    void Awake()
    {
        // Ensure collider is a trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        // Create audio sources at runtime
        source1 = gameObject.AddComponent<AudioSource>();
        source1.loop = true;
        source1.playOnAwake = false;
        source1.spatialBlend = 0.7f;   // 3D-ish sound
        source1.dopplerLevel = 0f;     // Disable doppler effect

        source2 = gameObject.AddComponent<AudioSource>();
        source2.loop = true;
        source2.playOnAwake = false;
        source2.spatialBlend = 0.7f;   // 3D-ish sound
        source2.dopplerLevel = 0f;     // Disable doppler effect
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (sfx1 != null)
            {
                source1.clip = sfx1;
                source1.volume = volume1;
                source1.Play();
            }

            if (sfx2 != null)
            {
                source2.clip = sfx2;
                source2.volume = volume2;
                source2.Play();
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            source1.Stop();
            source2.Stop();
        }
    }
}

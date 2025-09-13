using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class VoiceTrigger : MonoBehaviour
{
    [Header("Voice Settings")]
    public string voiceClipID;   // The ID from your CSV for this voice line
    [Range(0f, 2f)] public float volume = 1f; // Volume multiplier

    [Header("Optional Intro SFX")]
    public AudioClip introSfx;   // Play this before the voice line
    [Range(0f, 1f)] public float introVolume = 1f;

    [Header("PA / Distance Settings")]
    public bool isDistant = false;       // Toggle PA/distant mode
    [Range(0f, 1f)] public float spatialBlend = 0.7f;
    [Range(0f, 1f)] public float distantVolumeScale = 0.6f;

    [Header("Radio Effect (Optional)")]
    public bool useRadioEffect = false;  // Toggle radio/PA filter
    [Range(500f, 5000f)] public float cutoffFrequency = 3000f; // How muffled it sounds

    private bool hasPlayed = false;

    private void Reset()
    {
        // Ensure collider defaults to trigger
        BoxCollider col = GetComponent<BoxCollider>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasPlayed) return;
        if (!other.CompareTag("Player")) return;

        if (introSfx != null)
        {
            // Play intro crackle/beep
            AudioSource tempSource = gameObject.AddComponent<AudioSource>();
            tempSource.clip = introSfx;
            tempSource.volume = introVolume;
            tempSource.spatialBlend = spatialBlend;
            tempSource.dopplerLevel = 0f;
            tempSource.Play();

            Invoke(nameof(PlayVoiceLine), introSfx.length);
        }
        else
        {
            PlayVoiceLine();
        }

        hasPlayed = true;
    }

    private void PlayVoiceLine()
    {
        if (AudioManager.instance == null) return;

        AudioManager.instance.PlayVoice(voiceClipID);

        AudioSource source = AudioManager.instance.GetVoiceSource(voiceClipID);
        if (source != null)
        {
            float baseVolume = volume * AudioManager.instance.globalVoiceBoost;

            if (isDistant)
            {
                source.spatialBlend = spatialBlend;
                source.dopplerLevel = 0f;
                source.volume = baseVolume * distantVolumeScale;
            }
            else
            {
                source.spatialBlend = 0f;
                source.volume = baseVolume;
            }

            // ✅ Apply radio filter if toggled
            if (useRadioEffect)
            {
                AudioLowPassFilter filter = source.gameObject.GetComponent<AudioLowPassFilter>();
                if (filter == null) filter = source.gameObject.AddComponent<AudioLowPassFilter>();
                filter.cutoffFrequency = cutoffFrequency; // Lower = more muffled
            }
        }
    }
}

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    private CharacterController player;
    private AudioSource audioSource;

    [System.Serializable]
    public class PlayerSound
    {
        public string clipKey;   // The AudioManager key
        [Range(0f, 2f)] public float volume = 1f; // Inspector volume multiplier
    }

    [Header("Player Sounds")]
    public PlayerSound landSound;
    public PlayerSound slideSound;
    public PlayerSound poleVaultSound;
    public PlayerSound dashSound;

    private bool wasGroundedLastFrame = true;

    void Awake()
    {
        player = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        HandleLanding();
    }

    private void HandleLanding()
    {
        if (player == null) return;

        // Detect transition from not grounded → grounded
        if (!wasGroundedLastFrame && player.IsGrounded)
        {
            PlayOneShot(landSound);
        }

        wasGroundedLastFrame = player.IsGrounded;
    }

    public void PlaySlide()
    {
        PlayOneShot(slideSound);
    }

    public void PlayPoleVault()
    {
        PlayOneShot(poleVaultSound);
    }

    public void PlayDash()
    {
        PlayOneShot(dashSound);
    }

    private void PlayOneShot(PlayerSound sound)
    {
        if (sound == null || string.IsNullOrEmpty(sound.clipKey) || AudioManager.instance == null) return;

        AudioClip clip = AudioManager.instance.GetClipByName(sound.clipKey);
        if (clip != null)
        {
            audioSource.PlayOneShot(clip, sound.volume);
        }
        else
        {
            Debug.LogWarning($"[PlayerAudio] Missing clip: {sound.clipKey}");
        }
    }
}

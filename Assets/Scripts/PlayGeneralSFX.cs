using UnityEngine;

public class PlayGeneralSFX : MonoBehaviour
{
    public AudioClip soundClip; // Assign this in the Inspector
    private AudioSource audioSource;
    private bool canPlaySound = true; // Condition to control sound playback

    void Start()
    {
        // Add an AudioSource component if it doesn't already exist
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = soundClip;
    }

    void Update()
    {
        // Play sound only if "Space" is pressed and canPlaySound is true, (to be modified to suit purposes)
        if (Input.GetKeyDown(KeyCode.Space) && canPlaySound)
        {
            PlaySoundEffect();
        }
    }

    public void PlaySoundEffect()
    {
        if (soundClip != null && audioSource != null)
        {
            audioSource.Play();
            canPlaySound = false; // Disable further playback until reset
        }
    }

    // Example: Resets the condition to allow the sound to play again, (to be modified)
    public void ResetSoundCondition()
    {
        canPlaySound = true;
    }
}

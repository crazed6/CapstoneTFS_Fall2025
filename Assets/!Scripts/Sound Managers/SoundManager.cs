using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Mixer")]
    public AudioMixer audioMixer;

    [Header("Audio Sources")]           //Add an Audio Source Component to SoundManager in Hierarchy and set it's Output to BGM/SFX/Voice then drag that component to these in inspector
    public AudioSource bgmSource;
    public AudioSource sfxSource;
    public AudioSource voiceSource;

    [Header("BGM Audio Clips")]             //click + and drag the clip into the index 
    public AudioClip[] backgroundMusic;

    [Header("SFX Audio Clips")]
    public AudioClip[] sfxSound;

    [Header("Voice Audio Clips")]
    public AudioClip[] voiceSound;

    //Usage - add these lines in the script you want to play the clip SoundManager.Instance.PlayBGM(0); or SoundManager.Instance.PlayBGM(0); or SoundManager.Instance.PlayBGM(0);
    //change the (0) to the number of the clip you set

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(int index)
    {
        if (index >= 0 && index < backgroundMusic.Length)
        {
            bgmSource.clip = backgroundMusic[index];
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(int index)
    {
        if (index >= 0 && index < sfxSound.Length)
        {
            sfxSource.clip = sfxSound[index];
            sfxSource.Play();
        }
    }

    public void PlayVoice(int index)
    {
        if (index >= 0 && index < voiceSound.Length)
        {
            voiceSource.Stop(); // Stop any ongoing voice clip
            voiceSource.clip = voiceSound[index];
            voiceSource.Play();
        }
    }

    public void SetVolume(string parameter, float value)
    {
        audioMixer.SetFloat(parameter, Mathf.Log10(value) * 20);
    }

    public void SetMasterVolume(float volume)
    {
        SoundManager.Instance.SetVolume("MasterVolume", volume);
    }
    public void SetMusicVolume(float volume)
    {
        SoundManager.Instance.SetVolume("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        SoundManager.Instance.SetVolume("SFXVolume", volume);
    }
}

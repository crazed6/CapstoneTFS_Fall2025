using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : SingletonInScene<SoundManager>
{
    public static SoundManager instance;
    public AudioClip[] backgroundMusic;
    public AudioClip sceneStartSound;
    public AudioMixer audioMixer; // Reference to the Audio Mixer
    public AudioMixerGroup musicGroup; // Reference to the Music Mixer Group
    private AudioSource musicSource;
    //private AudioSource audioSource;

    protected override void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        
        base.Awake();
        DontDestroyOnLoad(this.gameObject);

        //audioSource = GetComponent<AudioSource>();
        //audioSource.loop = true; //Used to loop the audio

        // added to control music separately 
        musicSource = gameObject.AddComponent<AudioSource>();
        musicSource.outputAudioMixerGroup = musicGroup;
        musicSource.loop = true;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int sceneIndex = scene.buildIndex; //Gets the index of the scene, (Title being 0, Game being 1. Order can be found in File> BuildSettings)

        if (sceneIndex < backgroundMusic.Length && backgroundMusic[sceneIndex] != null) //Used to check that the Scene index corresponds wit the array for the music
        {
            musicSource.clip = backgroundMusic[sceneIndex]; //Used to ensure that the corresponding track plays
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("No music assigned for scene index: " + sceneIndex);
        }

        if (scene.name == "GameOver") //Unused but want to play a certain sound upon the scene loading. Need to find way to designate another audio clip within same component.
        {
            musicSource.PlayOneShot(sceneStartSound);
        }
    }

    public void StopAudio() 
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
    }

    public void SetMasterVolume(float volume)
    {
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

} //AudioSource destroyed on load of Game Scene


using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundManager : SingletonInScene<SoundManager>
{
    public AudioClip[] backgroundMusic;
    public AudioClip sceneStartSound;

    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();

        audioSource.loop = true; //Used to loop the audio
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int sceneIndex = scene.buildIndex; //Gets the index of the scene, (Title being 0, Game being 1. Order can be found in File> BuildSettings)

        if (sceneIndex < backgroundMusic.Length && backgroundMusic[sceneIndex] != null) //Used to check that the Scene index corresponds wit the array for the music
        {
            audioSource.clip = backgroundMusic[sceneIndex]; //Used to ensure that the corresponding track plays
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No music assigned for scene index: " + sceneIndex);
        }

        if (scene.name == "GameOver") //Unused but want to play a certain sound upon the scene loading. Need to find way to designate another audio clip within same component.
        {
            audioSource.PlayOneShot(sceneStartSound);
        }
    }
} //AudioSource destroyed on load of Game Scene


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video; // Needed for VideoPlayer

public class Intro_Manager : MonoBehaviour
{
    [Header("Next Scene")]
    [Tooltip("Name of the scene to load after intro finishes")]
    public string nextSceneName = "Kadeem_MainMenu";

    [Header("Timer Option (if not using video)")]
    [Tooltip("How many seconds to wait before switching scene (used only if no video is assigned)")]
    public float introDuration = 5f;

    [Header("Optional Video Player")]
    public VideoPlayer introVideo; // Drag your VideoPlayer component here in Inspector (if any)

    private void Start()
    {
        if (introVideo != null)
        {
            // Subscribe to event when video ends
            introVideo.loopPointReached += OnVideoFinished;
            introVideo.Play();
        }
        else
        {
            // No video assigned: fallback to timer
            StartCoroutine(LoadNextSceneAfterDelay(introDuration));
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        LoadNextScene();
    }

    private System.Collections.IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadNextScene();
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.UI;
using System.Collections;

public class Intro_Manager : MonoBehaviour
{
    [Header("Next Scene")]
    public string nextSceneName = "Kadeem_MainMenu";

    [Header("Timer Option (if not using video)")]
    public float introDuration = 5f;

    [Header("Optional Video Player")]
    public VideoPlayer introVideo;

    [Header("Skip UI")]
    public GameObject skipButton;
    public float skipButtonTimeout = 2f;

    private bool hasLoadedNext = false;
    private float skipButtonTimer = 0f;

    private void Start()
    {
        if (skipButton != null)
            skipButton.SetActive(false);

        if (introVideo != null)
        {
            introVideo.loopPointReached += OnVideoFinished;
            introVideo.Play();
        }
        else
        {
            StartCoroutine(LoadNextSceneAfterDelay(introDuration));
        }
    }

    private void Update()
    {
        if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
        {
            ShowSkipButton();
        }

        if (skipButton != null && skipButton.activeSelf)
        {
            skipButtonTimer += Time.deltaTime;
            if (skipButtonTimer >= skipButtonTimeout)
            {
                skipButton.SetActive(false);
            }
        }
    }

    private void ShowSkipButton()
    {
        if (skipButton == null || hasLoadedNext) return;

        skipButton.SetActive(true);
        skipButtonTimer = 0f;
    }

    public void OnSkipButtonClicked()
    {
        if (!hasLoadedNext)
        {
            Debug.Log("Cutscene skipped via button.");
            PauseAllAudio();
            LoadNextScene();
        }
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        LoadNextScene();
    }

    private IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadNextScene();
    }

    private void PauseAllAudio()
    {
        Debug.Log("Pausing all audio in intro scene.");
        AudioListener.pause = true; // halts all AudioSources globally
    }

    private void LoadNextScene()
    {
        if (hasLoadedNext) return;
        hasLoadedNext = true;

        // Subscribe to re-enable audio after scene load
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene(nextSceneName);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded, resuming audio.");
        AudioListener.pause = false;

        // Unsubscribe so it only runs once
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

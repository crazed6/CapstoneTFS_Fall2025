using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class OutroManager : MonoBehaviour
{
    [Header("Outro & Credits Objects")]
    public GameObject outroObject;       // Empty GameObject with outro video
    public GameObject creditsObject;     // Empty GameObject with credits video

    [Header("UI Panels")]
    public GameObject pauseMenuUI;       // UI with MainMenu + Quit buttons
    public Image blackScreen;            // UI Image covering screen (black)

    [Header("Timers")]
    public float outroDuration = 5f;        // Outro duration
    public float creditsDuration = 10f;     // How long credits play before menu shows

    [Header("Fade Durations")]
    public float blackFadeDuration = 1.5f;  // Black screen fade in/out speed

    private bool menuActivated = false;

    private void Awake()
    {
        if (blackScreen != null)
        {
            Color c = blackScreen.color;
            c.a = 1f; // fully black at start
            blackScreen.color = c;

            // 🚫 Make sure the fade overlay never blocks button clicks
            blackScreen.raycastTarget = false;
        }

        outroObject.SetActive(true);
        creditsObject.SetActive(false);
        pauseMenuUI.SetActive(false);
    }

    private void Start()
    {
        StartCoroutine(PlayOutroThenCredits());
    }

    private IEnumerator PlayOutroThenCredits()
    {
        // Fade from black into outro
        yield return StartCoroutine(FadeBlack(1f, 0f));

        // Wait outro duration
        yield return new WaitForSeconds(outroDuration);

        // Fade to black before switching
        yield return StartCoroutine(FadeBlack(0f, 1f));
        outroObject.SetActive(false);
        creditsObject.SetActive(true);

        // Fade from black into credits
        yield return StartCoroutine(FadeBlack(1f, 0f));

        // Wait credits duration
        yield return new WaitForSeconds(creditsDuration);

        // Fade to black before switching
        yield return StartCoroutine(FadeBlack(0f, 1f));
        creditsObject.SetActive(true);  // keep credits running
        pauseMenuUI.SetActive(true);

        // Fade from black into pause menu
        yield return StartCoroutine(FadeBlack(1f, 0f));

        menuActivated = true;
    }

    private IEnumerator FadeBlack(float start, float end)
    {
        float elapsed = 0f;
        Color c = blackScreen.color;

        while (elapsed < blackFadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(start, end, elapsed / blackFadeDuration);
            blackScreen.color = c;
            yield return null;
        }

        c.a = end;
        blackScreen.color = c;
    }

    // === BUTTON FUNCTIONS ===
    public void OnMainMenuButton()
    {
        StartCoroutine(LoadSceneWithBlack("Kadeem_MainMenu"));
    }

    public void OnQuitButton()
    {
        StartCoroutine(QuitWithBlack());
    }

    private IEnumerator LoadSceneWithBlack(string sceneName)
    {
        // Fade to black
        yield return StartCoroutine(FadeBlack(0f, 1f));
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator QuitWithBlack()
    {
        // Fade to black
        yield return StartCoroutine(FadeBlack(0f, 1f));
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

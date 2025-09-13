using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class OutroManager : MonoBehaviour
{
    [Header("Outro & Credits Objects")]
    public GameObject outroObject;       // Empty GameObject with outro video
    public GameObject creditsObject;     // Empty GameObject with credits video

    [Header("UI Panels")]
    public GameObject pauseMenuUI;       // UI with MainMenu + Quit buttons

    [Header("Timers")]
    public float outroDuration = 5f;        // Outro duration
    public float creditsDuration = 10f;     // How long credits play before menu shows

    [Header("Fade Durations")]
    public float fadeInDuration = 1.5f;     // Time to fade in
    public float fadeOutDuration = 1.5f;    // Time to fade out

    private CanvasGroup outroGroup;
    private CanvasGroup creditsGroup;
    private CanvasGroup pauseMenuGroup;

    private bool menuActivated = false;

    private void Awake()
    {
        outroGroup = outroObject.GetComponent<CanvasGroup>();
        if (outroGroup == null) outroGroup = outroObject.AddComponent<CanvasGroup>();

        creditsGroup = creditsObject.GetComponent<CanvasGroup>();
        if (creditsGroup == null) creditsGroup = creditsObject.AddComponent<CanvasGroup>();

        pauseMenuGroup = pauseMenuUI.GetComponent<CanvasGroup>();
        if (pauseMenuGroup == null) pauseMenuGroup = pauseMenuUI.AddComponent<CanvasGroup>();

        // Initial states
        outroObject.SetActive(true);
        outroGroup.alpha = 1f;

        creditsObject.SetActive(false);
        creditsGroup.alpha = 0f;

        pauseMenuUI.SetActive(false);
        pauseMenuGroup.alpha = 0f;
    }

    private void Start()
    {
        StartCoroutine(PlayOutroThenCredits());
    }

    private IEnumerator PlayOutroThenCredits()
    {
        // Outro timing
        yield return new WaitForSeconds(outroDuration);

        // Fade out outro
        yield return StartCoroutine(FadeCanvasGroup(outroGroup, 1f, 0f, fadeOutDuration));
        outroObject.SetActive(false);

        // Fade in credits
        creditsObject.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(creditsGroup, 0f, 1f, fadeInDuration));

        // Wait credits duration
        yield return new WaitForSeconds(creditsDuration);

        // Fade in pause menu (credits still playing or finished in background)
        pauseMenuUI.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(pauseMenuGroup, 0f, 1f, fadeInDuration));

        menuActivated = true;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float start, float end, float duration)
    {
        float elapsed = 0f;
        group.alpha = start;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, end, elapsed / duration);
            yield return null;
        }

        group.alpha = end;
    }

    // === BUTTON FUNCTIONS (no fade out) ===
    public void OnMainMenuButton()
    {
        if (!menuActivated) return;
        SceneManager.LoadScene("Kadeem_MainMenu");
    }

    public void OnQuitButton()
    {
        if (!menuActivated) return;
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

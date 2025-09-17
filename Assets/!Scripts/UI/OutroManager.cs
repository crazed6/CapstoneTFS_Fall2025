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

    private bool menuActivated = false;

    private void Awake()
    {
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
        // Play outro
        yield return new WaitForSeconds(outroDuration);

        // Switch to credits
        outroObject.SetActive(false);
        creditsObject.SetActive(true);

        // Wait credits duration
        yield return new WaitForSeconds(creditsDuration);

        // Show pause menu
        pauseMenuUI.SetActive(true);
        menuActivated = true;
    }

    // === BUTTON FUNCTIONS ===
    public void OnMainMenuButton()
    {
        SceneManager.LoadScene("Kadeem_MainMenu");
    }

    public void OnQuitButton()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}

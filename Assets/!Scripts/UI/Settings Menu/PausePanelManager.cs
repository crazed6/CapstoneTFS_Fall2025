using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PausePanelManager : MonoBehaviour
{
    public static PausePanelManager Instance { get; private set; }
    
    [SerializeField] private GameObject pauseMenuBackground;
    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsMenuUI;

    [Header("Settings Menu Root Elements")]
    [SerializeField] private GameObject buttonsVGroup;    // The group of Keybind/Volume/Display/Back buttons

    [Header("Settings Sub Panels")]
    [SerializeField] private GameObject keybindsUI_View;
    [SerializeField] private GameObject volumeUI_View;
    [SerializeField] private GameObject displayUI_View;

    [Header("External UI Panels - In Game CheckPoints")]
    [SerializeField] private GameObject inGameCheckPointsPanel;

    [Header("Checkpoint Proximity Control")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform[] checkpointTransform; // Current checkpoint to check distance from
    [SerializeField] private float checkpointActivationDistance = 2.45f; // Range to consider 'close'

    private bool isPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("PausePanelManager Awake - initialized - active in scene:" + SceneManager.GetActiveScene().name);

        // default state:
        if (buttonsVGroup != null) buttonsVGroup.SetActive(true);
        HideAllSettingsViews();
    }

    private void Update()
    {
        if (Keyboard.current.vKey.wasPressedThisFrame) // Check if the V key is pressed
        {
            // Prevent pause toggle if in settings menu or in-game checkpoints panel is active
            if (inGameCheckPointsPanel != null && inGameCheckPointsPanel.activeSelf)
            {
                Debug.Log("[Check] Checkpoint Panel is Active");

                if (playerTransform != null && checkpointTransform != null && checkpointTransform.Length > 0)
                {
                    foreach (Transform checkpoint in checkpointTransform)
                    {
                        float distanceToCheckpoint = Vector3.Distance(playerTransform.position, checkpoint.position);
                        Debug.Log($"Distance to checkpoint '{checkpoint.name}': {distanceToCheckpoint:F2}");

                        if (distanceToCheckpoint <= checkpointActivationDistance)
                        {
                            Debug.Log($"[Blocked] Checkpoint panel active & player within {checkpointActivationDistance} units – pause disabled.");
                            return; // Exit early if player is near a checkpoint
                        }
                    }
                }
                else
                {
                    Debug.LogWarning("PlayerTransform or CheckpointTransform not assigned properly.");
                }
            }
            Debug.Log("V key [Detected] pressed - toggling pause menu");
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        Time.timeScale = isPaused ? 0 : 1;
        pauseMenuBackground.SetActive(isPaused);
        pauseMenuUI.SetActive(isPaused);

        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(false); // Always hide settings when toggling pause

        // Hide sub-panels when unpausing
        if (!isPaused)
        {
            if (keybindsUI_View != null)
                keybindsUI_View.SetActive(false);
            if (volumeUI_View != null)
                volumeUI_View.SetActive(false);
            if (displayUI_View != null)
                displayUI_View.SetActive(false);
        }

        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }


    public void ShowGameSettings()
    {
        isPaused = true; // Ensure the game is paused when opening settings
        Time.timeScale = 0; // Pause the game time
        pauseMenuUI.SetActive(false); // Hide the pause menu UI
        settingsMenuUI.SetActive(true); // Show the settings menu UI

        // show buttons VGroup, hide any subviews
        if (buttonsVGroup != null) buttonsVGroup.SetActive(true);
        HideAllSettingsViews();

        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Show the cursor

        Debug.Log("Entered Settings Menu - Defaulted to keyboard panel visible.");
    }

    public void CloseGameSettings()
    {
        settingsMenuUI.SetActive(false); // Hide the settings menu UI
        pauseMenuUI.SetActive(true); // Show the pause menu UI
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor back (for FPS-style control)
        Cursor.visible = false; // Hide it again
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        pauseMenuBackground.SetActive(isPaused);
        pauseMenuUI.SetActive(isPaused);
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor back (for FPS-style control)
        Cursor.visible = false;                   // Hide it again
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
#if UNITY_EDITOR
        EditorApplication.isPlaying = false; // Stop play mode in the editor
#else
        Application.Quit(); // Quit the game in a build
#endif
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1; // Ensure time is normal before loading
        SceneManager.LoadScene(sceneName);
        Debug.Log($"Loading scene: and or relocate to where saved files are stored and user sleects which save file to click on and then it loads thatr saved file that was stored.");
    }

    public bool IsPaused() => isPaused;

    // Utility
    private void HideAllSettingsViews()
    {
        if (keybindsUI_View != null) keybindsUI_View.SetActive(false);
        if (volumeUI_View != null) volumeUI_View.SetActive(false);
        if (displayUI_View != null) displayUI_View.SetActive(false);
    }

    // Button Handlers for switching panels
    public void OnKeybindsUI_ViewBtnClicked()
    {
        HideAllSettingsViews();
        if (buttonsVGroup != null) buttonsVGroup.SetActive(false);
        if (keybindsUI_View != null) keybindsUI_View.SetActive(true);
        Debug.Log("Keybinds UI View Opened");
    }

    public void OnVolumeUI_ViewBtnClicked()
    {
        HideAllSettingsViews();
        if (buttonsVGroup != null) buttonsVGroup.SetActive(false);
        if (volumeUI_View != null) volumeUI_View.SetActive(true);
        Debug.Log("Gamepad Panel Opened");
    }

    public void OnDisplayUI_ViewBtnClicked()
    {
        HideAllSettingsViews();
        if (buttonsVGroup != null) buttonsVGroup.SetActive(false);
        if (displayUI_View != null) displayUI_View.SetActive(true);
        Debug.Log("Volume Panel Opened");
    }

    public void OnBackToPauseMenuUI()
    {
        // Hide all panels and show pause menu UI and return/ show pause menu UI
        HideAllSettingsViews();
        if (buttonsVGroup != null) buttonsVGroup.SetActive(true);
        Debug.Log("Back to Pause Menu UI");
    }

    // called by “Back” button in Buttons_VGroup to return to Pause Menu:
    public void OnReturnToPauseMenuUI()
    {
        HideAllSettingsViews();
        if (buttonsVGroup != null) buttonsVGroup.SetActive(false);
        settingsMenuUI.SetActive(false);
        pauseMenuUI.SetActive(true);
    }

    public void OnReturnToMainMenu()
    {
        // Retun to the main menu scene
        Time.timeScale = 1; // Ensure time is normal before loading
        SceneManager.LoadScene("Kadeem_MainMenu"); // Replace with your main menu scene name
        Debug.Log("Returning to Main Menu");
    }
}
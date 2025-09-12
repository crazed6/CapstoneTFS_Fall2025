using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PausePanelManager : MonoBehaviour
{
    public static PausePanelManager Instance { get; private set; }

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject pauseMenuBackground;
    [SerializeField] private GameObject settingsMenuUI;

    [Header("Settings Sub Panels")]
    [SerializeField] private GameObject keyboardPanel;
    [SerializeField] private GameObject gamepadPanel;
    [SerializeField] private GameObject volumePanel;
    [SerializeField] private GameObject volumeBtn;

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

        // Show keyboard panel by default
        if (keyboardPanel != null)
        {
            keyboardPanel.SetActive(true);
            if (gamepadPanel != null) gamepadPanel.SetActive(false);
            if (volumePanel != null) volumePanel.SetActive(false);
        }
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
        pauseMenuUI.SetActive(isPaused);
        pauseMenuBackground.SetActive(isPaused);

        if (settingsMenuUI != null)
            settingsMenuUI.SetActive(false); // Always hide settings when toggling pause

        Cursor.lockState = isPaused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    public void ShowGameSettings()
    {
        isPaused = true; // Ensure the game is paused when opening settings
        Time.timeScale = 0; // Pause the game time
        pauseMenuUI.SetActive(false); // Hide the pause menu UI
        settingsMenuUI.SetActive(true); // Show the settings menu UI

        // Show keyboard panel by default
        if (keyboardPanel != null) keyboardPanel.SetActive(true);
        if (gamepadPanel != null) gamepadPanel.SetActive(false);
        if (volumePanel != null) volumePanel.SetActive(false);
        if (volumeBtn != null) volumeBtn.SetActive(true); // Make sure volume button is visible again

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
        pauseMenuUI.SetActive(false);
        pauseMenuBackground.SetActive(false);
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
        Debug.Log($"Loading scene: and or relocate to where saved files are stored and user sleects which save file to click on and then it loads thatr saved file that was stored.");
    }

    public bool IsPaused() => isPaused;

    // Button Handlers for switching panels
    public void OnKeyboardPanelBtnClicked()
    {
        if (keyboardPanel != null) keyboardPanel.SetActive(true);
        if (gamepadPanel != null) gamepadPanel.SetActive(false);
        if (volumePanel != null) volumePanel.SetActive(false);
        if (volumeBtn != null) volumeBtn.SetActive(true);
        Debug.Log("Keyboard Panel Opened");
    }

    public void OnGamepadPanelBtnClicked()
    {
        if (keyboardPanel != null) keyboardPanel.SetActive(false);
        if (gamepadPanel != null) gamepadPanel.SetActive(true);
        if (volumePanel != null) volumePanel.SetActive(false);
        Debug.Log("Gamepad Panel Opened");
    }

    public void OnVolumePanelBtnClicked()
    {
        if (keyboardPanel != null) keyboardPanel.SetActive(false);
        if (gamepadPanel != null) gamepadPanel.SetActive(false);
        if (volumePanel != null) volumePanel.SetActive(true);
        if (volumeBtn != null) volumeBtn.SetActive(false); // Hide the volume button when volume panel is open
        Debug.Log("Volume Panel Opened");
    }

    public void OnBackToPauseMenuUI()
    {
        // Hide all panels and show pause menu UI and return/ show pause menu UI
        if (keyboardPanel != null) keyboardPanel.SetActive(false);
        if (gamepadPanel != null) gamepadPanel.SetActive(false);
        if (volumePanel != null) volumePanel.SetActive(false);
        if (volumeBtn != null) volumeBtn.SetActive(true); // Show the volume button again
        pauseMenuUI.SetActive(true);
        if (settingsMenuUI != null) settingsMenuUI.SetActive(false); // Hide settings menu UI
        Debug.Log("Back to Pause Menu UI");
    }

    public void OnReturnToMainMenu()
    {
        // Retun to the main menu scene
        Time.timeScale = 1; // Ensure time is normal before loading
        SceneManager.LoadScene("Kadeem_MainMenu"); // Replace with your main menu scene name
        Debug.Log("Returning to Main Menu");
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    private static PauseManager instance;

    // The name of your main game scene. If you always use "SampleScene," you can keep it fixed.
    // The scene is never unloaded, so it won't restart from zero.
    private string gameSceneName = "BuildTesting";

    private bool isPaused = false;

    public static PauseManager Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("PauseManager instance is null!");
            }
            return instance;
        }
    }

    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        // Check the name of the currently active scene
        string activeScene = SceneManager.GetActiveScene().name;

        // 1) If we are in MainMenu, ESC key does nothing
        if (activeScene == "MainMenu")
        {
            // ESC in the main menu is disabled. No pause menu opening.
            return;
        }

        // 2) If SettingsMenu is loaded, ESC key will close it
        if (SceneManager.GetSceneByName("SettingsMenu").isLoaded)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.UnloadSceneAsync("SettingsMenu");
            }
            return;
        }

        // 3) If PauseMenu is loaded, ESC key will resume the game
        if (SceneManager.GetSceneByName("PauseMenu").isLoaded)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ResumeGame();
            }
            return;
        }

        // 4) If we are in a game scene (anything not MainMenu/PauseMenu/SettingsMenu), ESC key will pause the game
        if (IsGameScene(activeScene))
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PauseGame();
            }
        }
    }

    // Check if the given scene name is a "game scene" (not MainMenu, PauseMenu, or SettingsMenu)
    private bool IsGameScene(string sceneName)
    {
        return sceneName != "MainMenu" && sceneName != "PauseMenu" && sceneName != "SettingsMenu";
    }

    // Called when ESC is pressed in the game scene
    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;

        // Load PauseMenu additively
        if (!SceneManager.GetSceneByName("PauseMenu").isLoaded)
        {
            SceneManager.LoadScene("PauseMenu", LoadSceneMode.Additive);
        }
    }

    // Called when ESC or "Resume" is pressed in the PauseMenu
    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f;

        // If SettingsMenu is open, close it
        if (SceneManager.GetSceneByName("SettingsMenu").isLoaded)
        {
            SceneManager.UnloadSceneAsync("SettingsMenu");
        }

        // If PauseMenu is loaded, unload it and then make the game scene active
        if (SceneManager.GetSceneByName("PauseMenu").isLoaded)
        {
            SceneManager.UnloadSceneAsync("PauseMenu").completed += (op) =>
            {
                Scene loadedGame = SceneManager.GetSceneByName(gameSceneName);
                if (loadedGame.IsValid() && loadedGame.isLoaded)
                {
                    SceneManager.SetActiveScene(loadedGame);
                }
                else
                {
                    // If it's not loaded, additively load the game scene
                    SceneManager.LoadSceneAsync(gameSceneName, LoadSceneMode.Additive);
                }
                Debug.Log("Resumed to game scene: " + gameSceneName);
            };
        }
    }

    // Called when the player presses a "Main Menu" button in the PauseMenu
    public void OnMainMenuButtonPressed()
    {
        // If PauseMenu or SettingsMenu are open, unload them
        if (SceneManager.GetSceneByName("PauseMenu").isLoaded)
        {
            SceneManager.UnloadSceneAsync("PauseMenu");
        }
        if (SceneManager.GetSceneByName("SettingsMenu").isLoaded)
        {
            SceneManager.UnloadSceneAsync("SettingsMenu");
        }

        Time.timeScale = 1f;

        // Load MainMenu additively. We do not unload the game scene,
        // so your progress is kept in memory
        if (!SceneManager.GetSceneByName("MainMenu").isLoaded)
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }
    }
}


using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [Header("UI Panels")]
    public GameObject mainMenu; // Reference to the main menu panel
    public GameObject savedGameFilesPanel; // Reference to the panel for saved game files
    public GameObject creditsPanel; // Reference to the panel for credits
    public GameObject creditsVideo; // Reference to the credits video


    public void Start()
    {
        //SoundManager.Instance.PlayBGM(0);

        // Ensure input bindings are loaded (once)
        if (InputManager.Instance != null)
        {
            InputManager.Instance.LoadRebinds(); // Load rebinds from PlayerPrefs  
        }
    }
    //public void PlayGame() //method to start the first game scene
    //{
    //    SceneManager.LoadSceneAsync("BuildTesting"); //Replace SampleScene with the name of the first game scene 
    //    SoundManager.Instance.PlayBGM(2);
    //}
    public void Settings() //method to start the first game scene
    {
        SceneManager.LoadSceneAsync("Kadeem_OptionsMenu"); //Replace SampleScene with the name of the first game scene
        creditsVideo.SetActive(false);
        SoundManager.Instance.PlayBGM(1);
        Debug.Log("Settings button clicked");
    }

    public void ChangeScene(string SceneName)
    {
        //SoundManager.Instance.StopAudio();    ---causing errors with new SoundManager.cs
        GameSession.IsNewSession = true; // Reset the game session flag when changing scenes
        GameSession.IsLoadedGame = false; // Reset the loaded game flag when changing scenes
        SceneManager.LoadSceneAsync(SceneName);
    }
    public void QuitGame() //method to close the game 
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif

    }
    // Opens the panel that shows saved game files.
    public void ShowSavedGameFilesPanel()
    {
        if(savedGameFilesPanel == null)
        {
          Debug.LogError("LoadGameMenuController is not assigned in UIManager."); 
        }
        else
        {
          savedGameFilesPanel.SetActive(true);
          mainMenu.SetActive(false);
          creditsVideo.SetActive(false);
        } 
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Kadeem_MainMenu");
        savedGameFilesPanel.SetActive(false);
        mainMenu.SetActive(true);
        creditsVideo.SetActive(false);
        //SoundManager.Instance.PlayBGM(1);
    }

    public void OpenCreditsPanel()
    {
        if(creditsPanel == null)
        {
          Debug.LogError("CreditsPanel is not assigned in UIManager."); 
        }
        else
        {
          creditsPanel.SetActive(true);
          creditsVideo.SetActive(true);
          mainMenu.SetActive(false);
        }
    }

}


using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [Header("UI Panels")]
    public GameObject savedGameFilesPanel; // Reference to the panel for saved game files


    public void Start()
    {
        //SoundManager.Instance.PlayBGM(0);

        // Ensure input bindings are loaded (once)
        if (InputManager.Instance != null)
        {
            InputManager.Instance.LoadRebinds(); // Load rebinds from PlayerPrefs  
        }
    }
    public void PlayGame() //method to start the first game scene
    {
        SceneManager.LoadSceneAsync("BuildTesting"); //Replace SampleScene with the name of the first game scene 
        SoundManager.Instance.PlayBGM(2);
    }
    public void Settings() //method to start the first game scene
    {
        SceneManager.LoadSceneAsync("Kadeem_SettingsMenu"); //Replace SampleScene with the name of the first game scene 
        SoundManager.Instance.PlayBGM(1);
        Debug.Log("Settings button clicked");
    }

    public void ChangeScene(string SceneName)
    {
        //SoundManager.Instance.StopAudio();    ---causing errors with new SoundManager.cs
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
        } 
    }

    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("Kadeem_MainMenu");
        savedGameFilesPanel.SetActive(false);
        //SoundManager.Instance.PlayBGM(1);
    }

}

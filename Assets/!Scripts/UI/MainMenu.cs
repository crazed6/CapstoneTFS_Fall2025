
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
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


    //Load Code Below

    //!!!!!!!    SAVE AND LOAD SYSTEM BEING REDONE DELETE THIS CODE OR CHANGE LATER   !!!!!!!

    //public void LoadGameButton()       
    //{
    //    string sceneName = SaveLoadSystem.GetSavedSceneName(); // Get the saved scene name from the save file

    //    if (string.IsNullOrEmpty(sceneName))
    //    {
    //        Debug.LogError("Scene name is empty or null. Cannot load saved scene.");
    //        return;
    //    }

    //    // Subscribe to sceneLoaded event before loading
    //    SceneManager.sceneLoaded += OnSceneLoaded;
    //    SceneManager.LoadScene(sceneName);
    //}

    //private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to prevent multiple triggers

    //    SaveLoadSystem.Load(); // 
    //}


}

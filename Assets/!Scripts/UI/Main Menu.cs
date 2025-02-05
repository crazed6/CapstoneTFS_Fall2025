using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Load "SampleScene" additively
        // Once it's loaded, unload the MainMenu
        SceneManager.LoadSceneAsync("SampleScene", LoadSceneMode.Additive).completed += (op) =>
        {
            if (SceneManager.GetSceneByName("MainMenu").isLoaded)
            {
                SceneManager.UnloadSceneAsync("MainMenu");
            }
        };
    }

    public void Settings()
    {
        // SettingsMenu can be loaded single or additively
        SceneManager.LoadSceneAsync("SettingsMenu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuSettingsButton : MonoBehaviour
{
    // When the user clicks the "Settings" button in the pause menu
    public void LoadingForSettingsButton()
    {
        if (!SceneManager.GetSceneByName("SettingsMenu").isLoaded)
        {
            SceneManager.LoadScene("SettingsMenu", LoadSceneMode.Additive);
        }
    }

    // When the user clicks "Resume" in the pause menu
    public void LoadingForResumeButton()
    {
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.ResumeGame();
        }
        else
        {
            Debug.LogWarning("PauseManager instance not found!");
        }
    }

    // When the user clicks "Main Menu" in the pause menu
    public void LoadingForMainMenuButton()
    {
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.OnMainMenuButtonPressed();
        }
        else
        {
            Debug.LogWarning("PauseManager instance not found!");
        }
    }
}

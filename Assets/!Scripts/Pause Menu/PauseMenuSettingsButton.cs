using UnityEngine;

public class PauseMenuSettingsButton : MonoBehaviour
{
    public void LoadingForSettingsBUtton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SettingsMenu");
    }

    public void LoadingForResumeButton()
    {
        
        if (PauseManager.Instance != null)
        {
            PauseManager.Instance.ResumeGame();
        }
        else
        {
            Debug.LogWarning("There is no!");
        }
    }
}

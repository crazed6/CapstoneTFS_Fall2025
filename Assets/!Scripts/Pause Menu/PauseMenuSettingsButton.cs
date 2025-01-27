using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenuSettingsButton : MonoBehaviour
{
    public void LoadingForSettingsBUtton()
    {
        SceneManager.LoadScene("SettingsMenu"); 
    }

    public void LoadingForResumeButton()
    {
        SceneManager.LoadScene("SampleScene"); 
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame() //method to start the first game scene
    {
        SceneManager.LoadSceneAsync("SampleScene"); //Replace SampleScene with the name of the first game scene 
    }
    public void Settings() //method to start the first game scene
    {
        SceneManager.LoadSceneAsync("SettingsMenu"); //Replace SampleScene with the name of the first game scene 
    }

    public void ChangeScene(string SceneName)
    {
        SoundManager.Instance.StopAudio();
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
}

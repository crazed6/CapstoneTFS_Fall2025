using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Start()
    {
        SoundManager.Instance.PlayBGM(0);
    }
    public void PlayGame() //method to start the first game scene
    {
        SceneManager.LoadSceneAsync("EnemyPlayground"); //Replace SampleScene with the name of the first game scene 
        SoundManager.Instance.PlayBGM(2);
    }
    public void Settings() //method to start the first game scene
    {
        SceneManager.LoadSceneAsync("SettingsMenu"); //Replace SampleScene with the name of the first game scene 
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
}

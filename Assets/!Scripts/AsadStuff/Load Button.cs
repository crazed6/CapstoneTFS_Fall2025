using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadGameButton() //Josh's method to load the game
    {
        SaveLoadSystem.Load();
        string sceneName = SaveLoadSystem.GetSaveData().PlayerSaveData.SceneName;
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is empty or null. Loading last scene.");
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("No scene name found in save data. Loading default scene.");
            SceneManager.LoadScene("BuildTesting");
        }
    }
}

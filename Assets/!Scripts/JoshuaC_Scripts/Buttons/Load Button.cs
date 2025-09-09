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
        string sceneName = SaveLoadSystem.GetSavedSceneName(); // Get the saved scene name from the save file

        if (!string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Loading last scene." +sceneName);
            SceneManager.sceneLoaded += OnSceneLoaded; // Subscribe to the scene loaded event
            SceneManager.LoadScene(sceneName);

            SaveLoadSystem.Load(); // Load the save data after the scene is loaded
        }
        else
        {
            Debug.Log("No scene name found in save data.");
        }



    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // This method can be used to handle any actions after the scene is loaded
        Debug.Log("Scene loaded: " + scene.name);
        SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to avoid multiple calls

        SaveLoadSystem.Load(); // Load the save data after the scene is loaded

        //var player = GameObject.FindWithTag("Player");
        //if (player != null)
        //{
        //    var playerSave = player.GetComponent<PlayerSave>();
        //    if (playerSave != null)
        //    {
        //        playerSave.Load(SaveLoadSystem.GetSaveData().PlayerSaveData); // Load player data after scene load
        //    }
        //    else
        //    {
        //        Debug.LogError("PlayerSave component not found on player object.");
        //    }
        //}
        //else
        //{
        //  Debug.LogError("Player object not found in the scene.");
        //}
    }
}

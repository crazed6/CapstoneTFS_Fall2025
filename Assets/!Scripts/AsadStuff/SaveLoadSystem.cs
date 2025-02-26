using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;

public class SaveLoadSystem
{
    private static SaveData _saveData = new SaveData();
    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData PlayerSaveData;
    }

    public static string SaveFileName() //Persistent Path means that the files stored persist between sessions. /save determines the name of the file, while .save determines the file extension
    {
        string SaveFile = Application.persistentDataPath + "/save" + ".save";
        return SaveFile;
    }

    public static void Save()
    {
        HandleSaveData();
        string json = JsonUtility.ToJson(_saveData, true);


        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true)); //Replaces what is in the SaveFile and so overwrites it.
        Debug.Log("Saved Data: " + json); //Logging data to verify that saving is working
    }

    private static void HandleSaveData()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("GameManager.Instance is null! Ensure GameManager exists in the scene.");
            return;
        }

        if (GameManager.Instance.PlayerSave == null)
        {
            Debug.LogError("PlayerSave is null! Make sure it's assigned in GameManager.");
            return;
        }

        GameManager.Instance.PlayerSave.Save(ref _saveData.PlayerSaveData);
        //After implementing and defining Player, then we'll be able to write various things to the file about the player.

    }

    public static void Load()
    {
        if (!File.Exists(SaveFileName()))
        {
            Debug.LogWarning("Save file does not exist! Skipping load.");
            return;
        }
        
        string SaveContent = File.ReadAllText(SaveFileName()); //Reads all text content of the save file
        Debug.Log("Loaded Data:" + SaveContent); //Logging Loaded data to verify that Load is working and info is correct
        _saveData = JsonUtility.FromJson<SaveData>(SaveContent);


        //Loading the scene saved in the data

        string SavedSceneName = _saveData.PlayerSaveData.SceneName;
        if (!string.IsNullOrEmpty(SavedSceneName))
        {
            // Subscribe to the sceneLoaded event to handle actions after the scene is loaded
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
            UnityEngine.SceneManagement.SceneManager.LoadScene(SavedSceneName); // Load the saved scene
        }

        HandleLoadData();

        GameObject player = GameObject.FindWithTag("Player"); // Assuming the player has the "Player" tag
        if (player != null)
        {
            // Get the PlayerSave component and apply the loaded data
            PlayerSave playerSave = player.GetComponent<PlayerSave>();
            if (playerSave != null)
            {
                playerSave.Load(_saveData.PlayerSaveData); // Apply saved player data
            }
            else
            {
                Debug.LogError("PlayerSave component not found on the Player object!");
            }
        }
        else
        {
            Debug.LogError("Player object not found in the scene!");
        }

    }

    public static void HandleLoadData()
    {
        if (GameManager.Instance.PlayerSave == null)
        {
            Debug.LogError("PlayerSave is null during load! Ensure PlayerSave exists in the scene.");
            return;
        }

        Debug.Log("Loading player position:" + _saveData.PlayerSaveData.Position);
        GameManager.Instance.PlayerSave.Load(_saveData.PlayerSaveData);

        Health playerhealth = GameManager.Instance.GetComponent<Health>();
        if(playerhealth != null)
        {
            playerhealth.health = (int)_saveData.PlayerSaveData.Health;
        }
    }

    public static SaveData GetSaveData()
    {
        return _saveData;
    }

    private static void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // Unsubscribe from the event to prevent it from being called multiple times
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;

        // Apply the loaded player data after the scene is fully loaded
        HandleLoadData();

        GameObject player = GameObject.FindWithTag("Player"); // Assuming the player has the "Player" tag
        if (player != null)
        {
            // Get the PlayerSave component and apply the loaded data
            PlayerSave playerSave = player.GetComponent<PlayerSave>();
            if (playerSave != null)
            {
                playerSave.Load(_saveData.PlayerSaveData); // Apply saved player data
            }
            else
            {
                Debug.LogError("PlayerSave component not found on the Player object!");
            }
        }
        else
        {
            Debug.LogError("Player object not found in the scene!");
        }
    }
}

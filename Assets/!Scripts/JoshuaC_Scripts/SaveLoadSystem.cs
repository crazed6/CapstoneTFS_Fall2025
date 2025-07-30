using UnityEngine;
using System.IO;


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

    public static void Save(CheckpointSystem checkpointSystem) //CheckpointSystem is passed in so that it can save the checkpoint data to a file
    {
        HandleSaveData();
        string json = JsonUtility.ToJson(_saveData, true);


        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(_saveData, true)); //Replaces what is in the SaveFile and so overwrites it.
        Debug.Log("Saved Data: " + json); //Logging data to verify that saving is working
        Debug.Log("Save file path: " + Application.persistentDataPath);

        checkpointSystem.SaveCheckpointToFile(); //Saves the checkpoint data to a file, so that it can be loaded later

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

        _saveData.PlayerSaveData.Position = CharacterController.instance.transform.position; //Saves the current position of the player
        Debug.Log("Saving player position: " + _saveData.PlayerSaveData.Position);
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
        HandleLoadData();

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

    public static string GetSavedSceneName()
    {
        if (!File.Exists(SaveFileName()))
        {
            Debug.LogWarning("Save file does not exist! Cannot retrieve scene name.");
            return null;
        }
        string json = File.ReadAllText(SaveFileName());
       _saveData = JsonUtility.FromJson<SaveData>(json);
        return _saveData.PlayerSaveData.SceneName;
    }

    //Button Prompt on SaveLoad System instead of OnTrigger
}

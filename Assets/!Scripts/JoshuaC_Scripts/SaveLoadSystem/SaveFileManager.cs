using UnityEngine;
using System.IO;

public class SaveFileManager : MonoBehaviour
{
    public void DeleteAllSaveFiles()
    {
        // Delete main save file (.save)
        string saveFilePath = Application.persistentDataPath + "/save.save";
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath);
        }

        // Delete checkpoint file (.json)
        string checkpointFilePath = Application.persistentDataPath + "/checkpoint.json";
        if (File.Exists(checkpointFilePath))
        {
            File.Delete(checkpointFilePath);
        }

        // Clear all PlayerPrefs
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("All save files deleted!");
    }
}

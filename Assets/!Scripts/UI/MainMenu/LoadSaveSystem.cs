using System.IO;
using UnityEngine;

// SaveData class to hold the scene name and other data handling Json save and load operations.
public static class LoadSaveSystem
{
    private static string saveFilePath => Path.Combine(Application.persistentDataPath, "savefile.json");

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(saveFilePath, json);
    }

    public static SaveData Load()
    {
        if (!File.Exists(saveFilePath))
        {
            Debug.LogError("Save file not found!");
            return null;
        }

        string json = File.ReadAllText(saveFilePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static string GetSavedSceneName()
    {
        SaveData data = Load();
        return data?.sceneName;
    }
}

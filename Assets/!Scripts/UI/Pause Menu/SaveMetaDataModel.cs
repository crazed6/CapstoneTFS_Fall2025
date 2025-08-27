using System;
using System.IO;
using UnityEngine;

[Serializable]
public sealed class SaveMeta
{
    public string FileName;      // "save.save"
    public string AbsolutePath;  // full path on disk
    public DateTime SavedUtc;    // when the file was last written
    public string SceneName;     // optional: stored scene name
    //public int PlayerHealth;     // optional: stored player health

    public static SaveMeta FromFile(string path)
    {
        if (!File.Exists(path))
            return null;

        string json = File.ReadAllText(path);
        var saveData = JsonUtility.FromJson<SaveLoadSystem.SaveData>(json);

        return new SaveMeta
        {
            FileName = Path.GetFileName(path),
            AbsolutePath = path,
            SavedUtc = File.GetLastWriteTimeUtc(path),
            SceneName = saveData.PlayerSaveData.SceneName,
            //PlayerHealth = saveData.PlayerSaveData.Health
        };
    }
}

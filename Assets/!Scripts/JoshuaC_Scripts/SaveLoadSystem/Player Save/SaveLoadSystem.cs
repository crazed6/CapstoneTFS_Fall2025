using UnityEngine;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class SaveLoadSystem
{
    private static SaveData _saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public PlayerSaveData PlayerSaveData;
        public CheckpointData CheckpointData;
    }

    public static string SaveFolder => Application.persistentDataPath + "/Saves/";

    public static string SaveFileName(int slotIndex)
    {
        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);

        return Path.Combine(SaveFolder, $"save{slotIndex}.save");
    }

    // Find the next unused save slot (save1, save2, save3...)
    public static int GetNextSaveSlot()
    {
        if (!Directory.Exists(SaveFolder))
            Directory.CreateDirectory(SaveFolder);

        string[] files = Directory.GetFiles(SaveFolder, "save*.save");
        if (files.Length == 0) return 1;

        int maxSlot = files
            .Select(f => Path.GetFileNameWithoutExtension(f).Replace("save", ""))
            .Where(s => int.TryParse(s, out _))
            .Select(int.Parse)
            .DefaultIfEmpty(0)
            .Max();

        return maxSlot + 1;
    }

    public static void Save(CheckpointSystem checkpointSystem, int slotIndex = -1)
    {
        HandleSaveData();

        if (slotIndex < 0)
            slotIndex = GetNextSaveSlot();

        string path = SaveFileName(slotIndex);
        string json = JsonUtility.ToJson(_saveData, true);
        File.WriteAllText(path, json);

        GameSession.ActiveSaveSlot = slotIndex;
        GameSession.IsLoadedGame = true;
        GameSession.IsNewSession = false;

        Debug.Log($"Saved Data to {path}\nData: {json}");
    }

    private static void HandleSaveData()
    {
        if (GameManager.Instance == null || GameManager.Instance.PlayerSave == null)
        {
            Debug.LogError("Save failed: GameManager or PlayerSave missing!");
            return;
        }

        GameManager.Instance.PlayerSave.Save(ref _saveData.PlayerSaveData);
    }

    public static void Load(int slotIndex)
    {
        string path = SaveFileName(slotIndex);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Save file {path} does not exist!");
            return;
        }

        string saveContent = File.ReadAllText(path);
        _saveData = JsonUtility.FromJson<SaveData>(saveContent);
        Debug.Log($"Loaded Data from {path}\nData: {saveContent}");

        HandleLoadData();

        // Update GameSession state
        GameSession.ActiveSaveSlot = slotIndex;
        GameSession.IsLoadedGame = true;
        GameSession.IsNewSession = false;
    }

    private static void HandleLoadData()
    {
        if (GameManager.Instance.PlayerSave == null)
        {
            Debug.LogError("PlayerSave missing during load!");
            return;
        }

        GameManager.Instance.PlayerSave.Load(_saveData.PlayerSaveData);

        Health playerHealth = GameManager.Instance.GetComponent<Health>();
        if (playerHealth != null)
            playerHealth.health = (int)_saveData.PlayerSaveData.Health;
    }

    public static SaveData GetSaveData() => _saveData;

    public static List<string> GetAllSaves()
    {
        if (!Directory.Exists(SaveFolder))
            return new List<string>();

        return Directory.GetFiles(SaveFolder, "save*.save").OrderBy(f => f).ToList();
    }

    public static string GetSavedSceneName(int slotIndex)
    {
        string path = SaveFileName(slotIndex);
        if (!File.Exists(path))
        {
            Debug.LogWarning($"Save file {path} does not exist!");
            return null;
        }

        string json = File.ReadAllText(path);
        _saveData = JsonUtility.FromJson<SaveData>(json);
        return _saveData.PlayerSaveData.SceneName;
    }

    public static void Delete(int slotIndex)
    {
        string path = SaveFileName(slotIndex);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Deleted save file: {path}");
        }
        else
        {
            Debug.LogWarning($"Could not find save file to delete: {path}");
        }
    }

    public static void ClearMemoryData()
    {
        _saveData = new SaveData(); // reset everything
        Debug.Log("[SaveLoadSystem] Cleared in-memory save data for new game.");
    }

    public static void SetCheckpoint(Vector3 checkpoint)
    {
        _saveData.CheckpointData = new CheckpointData
        {
            x = checkpoint.x,
            y = checkpoint.y,
            z = checkpoint.z
        };
    }
}

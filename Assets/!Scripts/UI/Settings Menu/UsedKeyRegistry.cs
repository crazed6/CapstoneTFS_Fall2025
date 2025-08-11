using System.Collections.Generic;
using UnityEngine;

public class UsedKeyRegistry : MonoBehaviour
{
    private static HashSet<string> usedKeys = new HashSet<string>();

    public static void RefreshUsedKeys()
    {
        usedKeys.Clear();

        if (InputManager.Instance == null || !InputManager.Instance.IsInitialized)
        {
            Debug.LogWarning("[UsedKeyRegistry] InputManager is not ready. Skipping RefreshUsedKeys.");
            return;
        }

        var asset = InputManager.Instance.GetAsset();

        if (asset == null)
        {
            Debug.LogWarning("[UsedKeyRegistry] InputManager asset is null.");
            return;
        }

        foreach (var actionMap in asset.actionMaps)
        {
            foreach (var action in actionMap.actions)
            {
                foreach (var binding in action.bindings)
                {
                    string path = binding.effectivePath;
                    if (!string.IsNullOrEmpty(path))
                        usedKeys.Add(path);
                }
            }
        }
    }

    public static bool IsKeyUsed(string path)
    {
        return usedKeys.Contains(path);
    }

    public static void AddUsedKey(string path)
    {
        usedKeys.Add(path);
    }

    public static void RemoveUsedKey(string path)
    {
        usedKeys.Remove(path);
    }

    public static HashSet<string> GetAllUsedKeys()
    {
        return usedKeys;
    }
}

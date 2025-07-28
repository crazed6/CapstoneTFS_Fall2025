using System.Collections.Generic;
using UnityEngine;

public class UsedKeyRegistry : MonoBehaviour
{
    private static HashSet<string> usedKeys = new HashSet<string>();

    public static void RefreshUsedKeys()
    {
        usedKeys.Clear();
        var asset = InputManager.Instance.GetAsset();

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

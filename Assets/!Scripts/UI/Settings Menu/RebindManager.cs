using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public static class RebindManager
{
    public static void StartRebind(string actionName, int bindingIndx, TMP_Text displayTxt, Action onComplete = null)
    {
        var action = InputManager.Instance.FindAction(actionName);

        if (action == null)
        {
            Debug.LogError($"Action '{actionName}' not found.");
            return;
        }

        if (bindingIndx >= action.bindings.Count)
        {
            Debug.LogError($"Binding index {bindingIndx} is out of range for action '{actionName}'.");
            return;
        }

        if (displayTxt != null)
        {
            displayTxt.color = Color.white; // Reset color to white before rebinding
            displayTxt.text = "Press any key...";
        }

        // Disable the action before rebinding
        action.Disable();

        var originalPath = action.bindings[bindingIndx].effectivePath;

        action.PerformInteractiveRebinding(bindingIndx)
            .WithControlsExcluding("Mouse") // Exclude mouse ?? Confirm with Team group.
            .OnMatchWaitForAnother(0.1f)
            .OnComplete(operation =>
            {
                operation.Dispose();
                string newPath = action.bindings[bindingIndx].effectivePath;

                if (UsedKeyRegistry.IsKeyUsed(newPath) && newPath != originalPath)
                {
                    Debug.LogWarning($"Key {newPath} is already in use.");
                    action.RemoveBindingOverride(bindingIndx); // Revert
                    if (displayTxt != null)
                    {
                        displayTxt.color = Color.red;
                        displayTxt.text = "Key already in use!";
                    }
                    action.Enable();
                    return;
                }

                // Register new key
                UsedKeyRegistry.RemoveUsedKey(originalPath);
                UsedKeyRegistry.AddUsedKey(newPath);

                action.Enable();
                if (displayTxt != null)
                { 
                displayTxt.color = Color.white;
                displayTxt.text = InputControlPath.ToHumanReadableString(newPath, InputControlPath.HumanReadableStringOptions.OmitDevice);
                }
                InputManager.Instance.SaveRebinds();
                UsedKeyRegistry.RefreshUsedKeys(); // Update all other UIs
                onComplete?.Invoke();
            })
            .Start();
    }

    public static void PrintAllBindingIndexes()
    {
        if (InputManager.Instance == null)
        {
            Debug.LogWarning("[RebindManager] InputManager.Instance is null, cannot print bindings.");
            return;
        }

        var asset = InputManager.Instance.GetAsset();

        foreach (var actionMap in asset.actionMaps)
        {
            foreach (var action in actionMap.actions)
            {
                Debug.Log($"Action: {action.name}");
                for (int i = 0; i < action.bindings.Count; i++)
                {
                    Debug.Log($"[{i}] {action.bindings[i].name} => {action.bindings[i].path}");
                }
            }
        }
      }
    }


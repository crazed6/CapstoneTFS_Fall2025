using UnityEngine;


public class ResetBindings : MonoBehaviour
{
    public void ResetToDefault()
    {
        InputManager.Instance.ResetRebinds();
        Debug.Log("Bindings have been reset to default.");

        // Optional: Refresh displayed key texts
        RebindDisplayUpdater[] updaters = Object.FindObjectsByType<RebindDisplayUpdater>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (var updater in updaters)
        {
            updater.UpdateKeyDisplay();
        }
    }
}

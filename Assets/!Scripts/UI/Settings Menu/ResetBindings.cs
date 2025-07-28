using UnityEngine;


public class ResetBindings : MonoBehaviour
{
    public void ResetToDefault()
    {
        InputManager.Instance.ResetRebinds();
        InputManager.Instance.LoadRebinds(); // Reload the default bindings
        Debug.Log("Bindings have been reset to default.");

        // Refresh all RebindButtons in the scene
        foreach (var rebindBtn in Object.FindObjectsByType<RebindButton>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            rebindBtn.SendMessage("UpdateDisplay");
        }

        Debug.Log("Bindings reset to default and UI updated.");
    }
}

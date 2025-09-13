using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadButton : MonoBehaviour
{
    public void LoadGameFromSlot()
    {
        // Get the currently selected slot from the SaveSlotsManager
        int slotToLoad = GameSession.ActiveSaveSlot;
        if (slotToLoad <= 0)
        {
            Debug.LogError("No save slot selected to load!");
            return;
        }

        // Get the scene name from the save file WITHOUT loading all the data yet
        string sceneToLoad = SaveLoadSystem.GetSavedSceneName(slotToLoad);

        if (string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.LogError($"Could not find a scene name in save slot {slotToLoad}.");
            return;
        }

        // IMPORTANT: Set a flag to tell the next scene it needs to apply loaded data
        GameSession.IsLoadedGame = true;

        // Now, load the scene
        SceneManager.LoadScene(sceneToLoad);
    }
}
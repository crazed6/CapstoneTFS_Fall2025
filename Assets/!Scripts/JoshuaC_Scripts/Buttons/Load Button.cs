using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadButton : MonoBehaviour
{
    [SerializeField] private int slotIndex = 1;
    //Which save slot to load (set in Inspector per button)

    public void LoadGameButton()
    {
        SaveLoadSystem.Load(GameSession.ActiveSaveSlot); // Load from slot

        var saveData = SaveLoadSystem.GetSaveData();

        GameSession.IsNewSession = false;
        GameSession.IsLoadedGame = true;

        // Set player position
        CharacterController.instance.GetComponent<CharacterController>().enabled = false;

        if (saveData.CheckpointData.x != 0 || saveData.CheckpointData.y != 0 || saveData.CheckpointData.z != 0)
        {
            CharacterController.instance.transform.position = new Vector3(
                saveData.CheckpointData.x,
                saveData.CheckpointData.y + 1.5f, // Slight offset
                saveData.CheckpointData.z
            );
            Debug.Log("Loaded checkpoint position: " + CharacterController.instance.transform.position);
        }
        else
        {
            // Fallback if no checkpoint
            CharacterController.instance.transform.position = Vector3.zero; // or spawn point
        }

        CharacterController.instance.GetComponent<CharacterController>().enabled = true;

        // Load Player Data (health, inventory, etc.)
        PlayerSave playerSave = GameManager.Instance.GetComponent<PlayerSave>();
        if (playerSave != null)
        {
            playerSave.Load(saveData.PlayerSaveData);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        SceneManager.sceneLoaded -= OnSceneLoaded;

        // Apply saved data *after* the scene has fully loaded
        SaveLoadSystem.Load(GameSession.ActiveSaveSlot);
    }
}

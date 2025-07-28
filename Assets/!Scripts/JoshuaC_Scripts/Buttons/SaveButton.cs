using UnityEngine;

public class SaveButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveGame()
    {
        var checkpointSystem = FindFirstObjectByType<CheckpointSystem>();
        if (checkpointSystem != null)
        {
            SaveLoadSystem.Save(checkpointSystem);
            Debug.Log("Game Saved");
        }
        else
        {
            Debug.LogError("CheckpointSystem not found in the scene. Cannot save game.");
        }

        //SaveLoadSystem.Save(); To Revert, remove this line, and checkpointSystem parameters from SaveLoadSystem.Save method
        //Debug.Log("Game Saved");
    }
}

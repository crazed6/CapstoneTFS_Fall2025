using UnityEngine;
[System.Serializable]

// This Class represents the data structure for saving game data.
public class SaveData
{
    // The things we want to save in the game.
    public string sceneName;
    public float playerHealth;
    public Vector3 playerPosition;
    // Add other fields as necessary to save additional game state information.
}

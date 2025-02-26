using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class PlayerSave : MonoBehaviour
{
    private static PlayerSave instance;


    public void Save(ref PlayerSaveData data)
    {
        data.Position = transform.position;

        //Saving Health as well, saved to same object

        Health health = GetComponent<Health>();
        if (health != null)
        {
            data.Health = health.health; //Saves the current health
        }

        // Save Last Checkpoint
        CheckpointSystem checkpointSystem = GetComponent<CheckpointSystem>();
        if (checkpointSystem != null && checkpointSystem.hasCheckpoint)
        {
            data.lastCheckpoint = checkpointSystem.lastCheckpoint;
            data.hasCheckpoint = true;
        }
        else
        {
            data.hasCheckpoint = false;
        }

        //Saves the Scene name
        data.SceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    public void Load(PlayerSaveData data)
    {
        Debug.Log("Setting player position to:" + data.Position);
        GetComponent<CharacterController>().enabled = false;

        transform.position = data.Position; //Used to convert the Data position into the current Transform

        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.health = (int)data.Health; //Loading the health value from the file
        }

        // Load Checkpoint
        CheckpointSystem checkpointSystem = GetComponent<CheckpointSystem>();
        if (checkpointSystem != null && data.hasCheckpoint)
        {
            checkpointSystem.lastCheckpoint = data.lastCheckpoint;
            checkpointSystem.hasCheckpoint = true;
        }

        GetComponent<CharacterController>().enabled = true; //Re-enabling character controller so as to prevent the override of the player position on load

    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

[System.Serializable]
public struct PlayerSaveData
{
    public Vector3 Position;
    public Vector3 lastCheckpoint;
    public int Health;
    public bool hasCheckpoint;
    public string SceneName;

}
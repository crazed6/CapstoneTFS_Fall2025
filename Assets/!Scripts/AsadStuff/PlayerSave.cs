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
    }

    public void Load(PlayerSaveData data)
    {
        Debug.Log("Setting player position to:" + data.Position);
        GetComponent<CharacterController>().enabled = false;

        transform.position = data.Position; //Used to convert the Data position into the current Transform

        GetComponent<CharacterController>().enabled = true; //Re-enabling character controller so as to prevent the override of the player position on load

        Health health = GetComponent<Health>();
        if (health != null)
        {
            health.health = (int)data.Health; //Loading the health value from the file
        }

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

}
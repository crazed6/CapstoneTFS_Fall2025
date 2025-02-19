using UnityEngine;
using System.IO;
using System.Collections;


public class CheckpointSystem : MonoBehaviour
{
    private Vector3 lastCheckpoint;
    private bool hasCheckpoint = false;
    private string saveFilePath;
    public CharacterController controller;

    void Start()
    {
        saveFilePath = Application.persistentDataPath + "/checkpoint.json";
        // StartCoroutine(WaitForCheckpoint());

        /*
        IEnumerator WaitForCheckpoint()
        {
            yield return new WaitForSeconds(2f);  // Correct usage
            Debug.Log("Checkpoint reached!");
            yield return null;
            LoadCheckpoint();
        } 
        */
    }

    void FixedUpdate()
    {

        if (Input.GetKeyDown(KeyCode.X) && hasCheckpoint)
        {
            Debug.Log("Respawning at: " + lastCheckpoint);
            Respawn();
        }
    }

    public void Respawn()
    {
        controller.enabled = false;
        transform.position = lastCheckpoint + Vector3.up * 1.5f; // Move player slightly above the checkpoint
        controller.enabled = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            lastCheckpoint = other.transform.position;
            SaveCheckpoint();
            hasCheckpoint = true;
            Debug.Log("Checkpoint activated at: " + lastCheckpoint);
        }
    }

    private void SaveCheckpoint() // Add coments on functionality
    {
        PlayerPrefs.SetFloat("CheckpointX", lastCheckpoint.x);
        PlayerPrefs.SetFloat("CheckpointY", lastCheckpoint.y);
        PlayerPrefs.SetFloat("CheckpointZ", lastCheckpoint.z);
        PlayerPrefs.Save();

        CheckpointData data = new CheckpointData { x = lastCheckpoint.x, y = lastCheckpoint.y, z = lastCheckpoint.z };


        File.WriteAllText(saveFilePath, JsonUtility.ToJson(data));
        Debug.Log("Checkpoint saved to file: " + saveFilePath);
    }

    private void LoadCheckpoint() //add more coment
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            CheckpointData data = JsonUtility.FromJson<CheckpointData>(json);
            lastCheckpoint = new Vector3(data.x, data.y, data.z);
            hasCheckpoint = true;
            Debug.Log("Loaded checkpoint from file: " + lastCheckpoint);
        }
        else if (PlayerPrefs.HasKey("CheckpointX"))
        {
            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            float z = PlayerPrefs.GetFloat("CheckpointZ");
            lastCheckpoint = new Vector3(x, y, z);
            hasCheckpoint = true;
            Debug.Log("Loaded checkpoint from PlayerPrefs: " + lastCheckpoint);
        }
        else
        {
            lastCheckpoint = transform.position; // Default starting position
            Debug.Log("No checkpoint found, using default start position: " + lastCheckpoint);
        }
    }
}

[System.Serializable]
public class CheckpointData
{
    public float x, y, z;
}

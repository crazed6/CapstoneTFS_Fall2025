using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.UI;


public class CheckpointSystem : MonoBehaviour //CheckpointSystem script only has to be attached to the Player object.
    // The Checkpoint will still require the tag however.
{
    private Vector3 lastCheckpoint;
    private bool hasCheckpoint = false;
    private string saveFilePath;
    public CharacterController controller;

    private bool panelTriggered = false; //This is to check if the panel is active or has been triggered.
    public GameObject checkpointPanel; //This is the panel that will be shown when the player reaches a checkpoint.

    void Start()
    {
        saveFilePath = Application.persistentDataPath + "/checkpoint.json";
        //StartCoroutine(WaitForCheckpoint()); //This and below was commented out before


        //IEnumerator WaitForCheckpoint()
        //{
        //    yield return new WaitForSeconds(2f);  // Correct usage
        //    Debug.Log("Checkpoint reached!");
        //    yield return null;
        //    LoadCheckpoint();
        //} 

        if (checkpointPanel == null)
        {
            Debug.LogError("Checkpoint Panel is not assigned in Inspector!");
        }

        HideCheckpointPanel(); // Hide the checkpoint panel at the start

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

    private void OnTriggerEnter(Collider other) //this works when attached to the player object the rigiddbody, and not when attached to the parent.
    {
        if (other.CompareTag("Checkpoint") && !panelTriggered)
        {
            lastCheckpoint = other.transform.position;
            SaveCheckpoint();
            hasCheckpoint = true;
            ShowCheckpointPanel();
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

    void ShowCheckpointPanel()
    {
        if (checkpointPanel == null)
        {
            Debug.LogError("Checkpoint Panel is not assigned in Inspector!");
            return;
        }
        checkpointPanel.SetActive(true); // Show the checkpoint panel
        Time.timeScale = 0; // Pause the game
        panelTriggered = true; // Set the trigger to true to prevent multiple activations

        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true;                  // Make it visible

        Debug.Log("Checkpoint panel shown.");
    }

    public void HideCheckpointPanel()
    {
        checkpointPanel.SetActive(false); // Hide the checkpoint panel
        Time.timeScale = 1; // Resume the game
        panelTriggered = false; // Reset the trigger for next checkpoint

        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false;                  // Make it invisible

        Debug.Log("Checkpoint panel hidden.");
    }
}

[System.Serializable]
public class CheckpointData
{
    public float x, y, z;
}

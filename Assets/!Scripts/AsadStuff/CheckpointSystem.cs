using UnityEngine;
using System.IO;
using System.Collections;
using UnityEngine.UI;


public class CheckpointSystem : MonoBehaviour
{
    public Vector3 lastCheckpoint;
    public bool hasCheckpoint = false;
    private string saveFilePath;
    public CharacterController controller;

    [SerializeField] private GameObject checkpointPanel; // Assign in the Inspector
    private bool panelTriggered = false; // Prevents re-triggering while inside the checkpoint

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

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint") && !panelTriggered)
        {
            Debug.Log("Checkpoint Triggered!"); // Debugging
            panelTriggered = true; // Prevents multiple triggers while inside
            ShowCheckpointPanel();


            lastCheckpoint = other.transform.position;
            SaveCheckpoint();
            hasCheckpoint = true;
            Debug.Log("Checkpoint activated at: " + lastCheckpoint);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            Debug.Log("Checkpoint Left!"); // Debugging
            panelTriggered = false; // Reset when player exits checkpoint area
        }
    }

    public void SaveCheckpoint() // Add coments on functionality
    {
        PlayerPrefs.SetFloat("CheckpointX", lastCheckpoint.x);
        PlayerPrefs.SetFloat("CheckpointY", lastCheckpoint.y);
        PlayerPrefs.SetFloat("CheckpointZ", lastCheckpoint.z);
        PlayerPrefs.Save();

        CheckpointData data = new CheckpointData { x = lastCheckpoint.x, y = lastCheckpoint.y, z = lastCheckpoint.z };


        File.WriteAllText(saveFilePath, JsonUtility.ToJson(data));
        Debug.Log("Checkpoint saved to file: " + saveFilePath);
    }

    public void LoadCheckpoint() //add more coment
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

    private void ShowCheckpointPanel()
    {
        if (checkpointPanel != null)
        {
            Debug.Log("Checkpoint Menu activated!");
            checkpointPanel.SetActive(true);
            Time.timeScale = 0f; // Pause the game
        }
        else
        {
            Debug.LogWarning("Checkpoint Menu is null!");
        }
    }

    public void HideCheckpointPanel()
    {
        if (checkpointPanel != null)
        {
            checkpointPanel.SetActive(false);
            Time.timeScale = 1f; // Resume the game
        }
    }

    public void SaveGame()
    {
        SaveLoadSystem.Save(); // Calls your existing save function
        HideCheckpointPanel();
    }
}

[System.Serializable]
public class CheckpointData
{
    public float x, y, z;
}

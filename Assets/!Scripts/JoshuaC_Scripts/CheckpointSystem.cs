using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // For UI elements like panels

public class CheckpointSystem : MonoBehaviour //CheckpointSystem script only has to be attached to the Player object.
                                              // The Checkpoint will still require the tag however.
{
    private Vector3 lastCheckpoint;
    private bool hasCheckpoint = false;
    private string saveFilePath;
    public CharacterController controller;

    private bool CheckpointPanelTriggered = false; //This is to check if the panel is active or has been triggered.
    private bool GameOverPanelTriggered = false; //This is to check if the game over panel is aive or has been triggered.
    public GameObject checkpointPanel; //This is the panel that will be shown when the player reaches a checkpoint.
    public GameObject gameOverPanel; //This is the panel that will be shown when the player dies.
    private bool isInCheckpointZone = false; //This is to check if the player is in the checkpoint zone.

    //=== New Variables ===
    [Header("Initial Checkpoint Settings")]
    public bool useManualCheckpoint = false; // If true, the player must manually set a checkpoint
    public Vector3 initialCheckpointPosition; // Default position for the initial checkpoint
    public Transform initialCheckpointObject;
    private Vector3 spawnPosition; // This will be used to store the spawn position

    //=== Reference to Character Controller for Disabling Movement ===
    [SerializeField] GameObject player; // Reference to the player GameObject, assign in inspector
    private CharacterController characterController; // Reference to the CharacterController component

    private FadetoBlack fadetoBlack; // Reference to the FadetoBlack script for fade effects

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
        HideGameOverPanel(); // Hide the game over panel at the start


        if (GameSession.IsNewSession) //&& !File.Exists(saveFilePath))
        {
            DeleteCheckpointPrefs(); // Clear PlayerPrefs if new session
        }
        GameSession.IsNewSession = false; // Reset the session flag after the first run

        LoadCheckpoint(); // Load the last checkpoint position

        if (hasCheckpoint)
        {
            transform.position = lastCheckpoint + Vector3.up * 1.5f; // Move player slightly above the checkpoint
        }
        else
        {
            lastCheckpoint = spawnPosition; // If no checkpoint exists, use the spawn position
        }

        //For grabbing player controller component
        if (player != null)
        {
            characterController = player.GetComponent<CharacterController>();
        }

        //fadetoBlack = gameOverPanel.GetComponentInChildren<FadetoBlack>(true); // Get the FadetoBlack component from the GameOver panel, even if inactive
        fadetoBlack = FindFirstObjectByType<FadetoBlack>(FindObjectsInactive.Include); // Find the FadetoBlack component in the scene, even if inactive

        if (fadetoBlack = null)
        {
            Debug.LogError("FadetoBlack component is not assigned in Inspector!");
        }


    }

    //void FixedUpdate()
    //{

    //    if (Input.GetKeyDown(KeyCode.X) && hasCheckpoint)
    //    {
    //        Debug.Log("Respawning at: " + lastCheckpoint);
    //        Respawn();
    //    }
    //}

    private void Update()
    {

        //if (Input.GetKeyDown(KeyCode.P) && isInCheckpointZone)
        //{
        //    if (!CheckpointPanelTriggered)
        //    {
        //        ShowCheckpointPanel();
        //    }
        //    else
        //    {
        //        HideCheckpointPanel();
        //    }
        //}
    }

    public void ResetRespawn()
    {
        controller.enabled = false;
        transform.position = lastCheckpoint + Vector3.up * 1.5f; // Move player slightly above the checkpoint
        controller.enabled = true;
    }

    //private void OnTriggerEnter(Collider other) //this works when attached to the player object the rigiddbody, and not when attached to the parent.
    //{
    //    if (other.CompareTag("Checkpoint")) //removing "&& !panelTriggered" to facilitate new functionality.
    //    {
    //        lastCheckpoint = other.transform.position;
    //        SaveCheckpoint();
    //        hasCheckpoint = true;
    //        isInCheckpointZone = true; // Set the flag to true when entering the checkpoint zone
    //        //ShowCheckpointPanel();
    //        Debug.Log("Checkpoint activated at: " + lastCheckpoint);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            lastCheckpoint = other.transform.position;
            SaveCheckpoint();
            hasCheckpoint = true;
            isInCheckpointZone = true; // Player is inside checkpoint zone

            // Only show the panel if it hasn’t been triggered yet
            if (!CheckpointPanelTriggered)
            {
                ShowCheckpointPanel();
            }

            Debug.Log("Checkpoint activated at: " + lastCheckpoint);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            isInCheckpointZone = false; // Set the flag to false when exiting the checkpoint zone
            Debug.Log("Exited checkpoint zone.");
        }
    }

    private void ClosePanelWithButton()
    {
        HideCheckpointPanel(); // Hide the checkpoint panel when the button is clicked
        Debug.Log("Checkpoint panel closed with button.");
    }

    private void SaveCheckpoint() // Add coments on functionality
    {
        PlayerPrefs.SetFloat("CheckpointX", lastCheckpoint.x);
        PlayerPrefs.SetFloat("CheckpointY", lastCheckpoint.y);
        PlayerPrefs.SetFloat("CheckpointZ", lastCheckpoint.z);
        PlayerPrefs.Save();

        //CheckpointData data = new CheckpointData { x = lastCheckpoint.x, y = lastCheckpoint.y, z = lastCheckpoint.z };


        //File.WriteAllText(saveFilePath, JsonUtility.ToJson(data));
        //Debug.Log("Checkpoint saved to file: " + saveFilePath);
    }

    public void SaveCheckpointToFile()
    {
        CheckpointData data = new CheckpointData { x = lastCheckpoint.x, y = lastCheckpoint.y, z = lastCheckpoint.z };


        File.WriteAllText(saveFilePath, JsonUtility.ToJson(data));
        Debug.Log("Checkpoint saved to file: " + saveFilePath);
    }

    private void LoadCheckpoint()
    {
        bool checkpointLoaded = false; // Flag to check if a checkpoint was loaded

        if (PlayerPrefs.HasKey("CheckpointX")) // Check if PlayerPrefs has checkpoint data
        {
            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            float z = PlayerPrefs.GetFloat("CheckpointZ");
            lastCheckpoint = new Vector3(x, y, z);
            hasCheckpoint = true;
            checkpointLoaded = true; // Set the flag to true since a checkpoint was loaded from PlayerPrefs
            Debug.Log("Loaded checkpoint from PlayerPrefs: " + lastCheckpoint);
            return; // Exit early if checkpoint loaded from PlayerPrefs
        }
        else if (File.Exists(saveFilePath)) // Check if the checkpoint file exists
        {
            string json = File.ReadAllText(saveFilePath);
            CheckpointData data = JsonUtility.FromJson<CheckpointData>(json);
            lastCheckpoint = new Vector3(data.x, data.y, data.z);
            hasCheckpoint = true;
            checkpointLoaded = true; // Set the flag to true since a checkpoint was loaded from file
            Debug.Log("Loaded checkpoint from file: " + lastCheckpoint);
            Debug.Log("Checkpoint loaded from file: " + saveFilePath); //Just added
            return; // Exit early if checkpoint loaded from file
        }

        if (!checkpointLoaded)
        {

            //Fallback to initial checkpoint position if no saved checkpoint exists
            if (useManualCheckpoint)
            {
                lastCheckpoint = initialCheckpointPosition;
                Debug.Log("No checkpoint found, using manual initial position: " + lastCheckpoint);
            }
            else if (initialCheckpointObject != null)
            {
                lastCheckpoint = initialCheckpointObject.position;
                Debug.Log("Using initial checkpoint object from GameObject: " + lastCheckpoint);
            }
            else
            {
                lastCheckpoint = spawnPosition; // Default starting position
                Debug.Log("No checkpoint found, using default start position: " + lastCheckpoint);
            }

            hasCheckpoint = false; // Set hasCheckpoint to true even if no saved checkpoint exists

        }
        else
        {
            hasCheckpoint = true; // Ensure hasCheckpoint is true if a checkpoint was loaded
        }
    }

    public void ClearSavedCheckpoint()
    {
        if (File.Exists(saveFilePath))
        {
            File.Delete(saveFilePath); // Delete the checkpoint file
            Debug.Log("Checkpoint file deleted: " + saveFilePath);
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
        CheckpointPanelTriggered = true; // Set the trigger to true to prevent multiple activations

        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true;                  // Make it visible

        // Disable player movement while the checkpoint panel is active
        if (characterController != null)
        {
            player.GetComponent<CharacterController>().enabled = false; // Disable the CharacterController to stop player movement
        }


        Debug.Log("Checkpoint panel shown.");
    }

    public void HideCheckpointPanel()
    {
        checkpointPanel.SetActive(false); // Hide the checkpoint panel
        Time.timeScale = 1; // Resume the game
        CheckpointPanelTriggered = false; // Reset the trigger for next checkpoint

        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false;                  // Make it invisible

        Debug.Log("Checkpoint panel hidden.");

        // Enable player movement when the checkpoint panel is hidden
        if (characterController != null)
        {
            player.GetComponent<CharacterController>().enabled = true; // Re-enable the CharacterController to allow player movement
        }
    }

    public void ShowGameOverPanel()
    {
        Debug.Log("Attempting to show GameOver panel...");

        if (gameOverPanel == null)
        {
            Debug.LogError("Game Over Panel is not assigned in Inspector!");
            return;
        }

        GameOverPanelTriggered = true; // Set the trigger to true to prevent multiple activations

        gameOverPanel.SetActive(true); // Show the game over panel
        Time.timeScale = 0; // Pause the game

        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true;                  // Make it visible
        Debug.Log("GameOver panel shown.");
    }

    public void HideGameOverPanel()
    {
        gameOverPanel.SetActive(false); // Hide the checkpoint panel
        Time.timeScale = 1; // Resume the game
        GameOverPanelTriggered = false; // Reset the trigger for next checkpoint

        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor
        Cursor.visible = false;                  // Make it invisible

        Debug.Log("GameOver panel hidden.");
        

    }

    //public void RespawnFromGameOver()
    //{
    //    //Reset Player state
    //    Health playerHealth = GetComponent<Health>();
    //    playerHealth.ResetHealth(); // Reset the player's health

    //    //Respawn at last checkpoint
    //    Respawn();

    //    //Hide the game over panel
    //    HideGameOverPanel();

    //    //Respawn All Enemies After Death
    //    EnemyRespawner enemyRespawner = FindFirstObjectByType<EnemyRespawner>();
    //    if (enemyRespawner != null)
    //    {
    //        enemyRespawner.RespawnAllEnemies(); // Assuming you have an EnemyRespawner script to handle enemy respawning
    //        Debug.Log("All enemies respawned after player death.");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("No EnemyRespawner found in the scene!");
    //    }

    //}

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
        Debug.Log("Scene reloaded: " + SceneManager.GetActiveScene().name);
    }

    public void StartNew()
    {
        PlayerPrefs.DeleteAll(); // Clear all PlayerPrefs data
        File.Delete(saveFilePath); // Delete the checkpoint file
        SceneManager.LoadScene("tut_Josh"); // Load the scene ("tut_Josh")
    }

    public void ResumeFromDeath()
    {
        HideGameOverPanel(); // Hide the game over panel
        ReloadScene(); // Reload the current scene
    }

    public void DeleteCheckpointPrefs()
    {
        PlayerPrefs.DeleteKey("CheckpointX");
        PlayerPrefs.DeleteKey("CheckpointY");
        PlayerPrefs.DeleteKey("CheckpointZ");
        Debug.Log("PlayerPrefs cleared.");
    }
}

[System.Serializable]
public class CheckpointData
{
    public float x, y, z;
}

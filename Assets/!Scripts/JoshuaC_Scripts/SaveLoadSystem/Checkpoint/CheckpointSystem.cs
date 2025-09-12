using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class CheckpointSystem : MonoBehaviour
{
    private Vector3 lastCheckpoint;
    private bool hasCheckpoint = false;

    [Header("Initial Checkpoint Settings")]
    public bool useManualCheckpoint = false;
    public Vector3 initialCheckpointPosition;
    public Transform initialCheckpointObject;
    private Vector3 spawnPosition;

    private bool CheckpointPanelTriggered = false;
    private bool GameOverPanelTriggered = false;
    private bool isInCheckpointZone = false;

    [Header("References")]
    public CharacterController controller;
    [SerializeField] private GameObject player;
    private CharacterController characterController;

    [Header("UI Panels")]
    public GameObject checkpointPanel;
    public GameObject gameOverPanel;

    private FadetoBlack fadetoBlack;

    public Vector3 LastCheckpoint => lastCheckpoint;

    void Start()
    {
        // Determine default spawn position
        if (initialCheckpointObject != null) spawnPosition = initialCheckpointObject.position;
        else if (useManualCheckpoint) spawnPosition = initialCheckpointPosition;
        else spawnPosition = transform.position;

        // Ensure there is an active save slot for this session
        if (GameSession.ActiveSaveSlot <= 0)
        {
            GameSession.ActiveSaveSlot = SaveLoadSystem.GetNextSaveSlot();
            SaveLoadSystem.Save(this, GameSession.ActiveSaveSlot);
            Debug.Log($"[CheckpointSystem] Created new save slot {GameSession.ActiveSaveSlot}");
        }

        // Load checkpoint from the active slot
        LoadCheckpointFromSlot(GameSession.ActiveSaveSlot);

        // Apply spawn/checkpoint position
        controller.enabled = false;
        transform.position = lastCheckpoint + Vector3.up * 1.5f;
        controller.enabled = true;

        GameSession.IsNewSession = false;

        if (player != null) characterController = player.GetComponent<CharacterController>();
        fadetoBlack = FindFirstObjectByType<FadetoBlack>(FindObjectsInactive.Include);

        HideCheckpointPanel();
        HideGameOverPanel();
    }

    private void LoadCheckpointFromSlot(int slot)
    {
        string path = SaveLoadSystem.SaveFileName(slot);
        if (File.Exists(path))
        {
            SaveLoadSystem.Load(slot);
            var saveData = SaveLoadSystem.GetSaveData();

            if (saveData.CheckpointData != null &&
                (saveData.CheckpointData.x != 0 ||
                 saveData.CheckpointData.y != 0 ||
                 saveData.CheckpointData.z != 0))
            {
                lastCheckpoint = new Vector3(
                    saveData.CheckpointData.x,
                    saveData.CheckpointData.y,
                    saveData.CheckpointData.z
                );
                hasCheckpoint = true;
                Debug.Log($"[CheckpointSystem] Loaded checkpoint from slot {slot}: {lastCheckpoint}");
                return;
            }
        }

        lastCheckpoint = spawnPosition;
        hasCheckpoint = false;
        Debug.Log($"[CheckpointSystem] No checkpoint found in slot {slot}, using spawn: {lastCheckpoint}");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Checkpoint")) return;

        lastCheckpoint = other.transform.position;
        hasCheckpoint = true;

        Debug.Log($"Checkpoint activated at: {lastCheckpoint}");

        // Ensure ActiveSaveSlot exists
        if (GameSession.ActiveSaveSlot <= 0)
            GameSession.ActiveSaveSlot = SaveLoadSystem.GetNextSaveSlot();

        // Commit checkpoint to save system
        SaveLoadSystem.SetCheckpoint(lastCheckpoint);

        // Save everything
        SaveLoadSystem.Save(this, GameSession.ActiveSaveSlot);

        FindFirstObjectByType<CheckpointSaveText>()?.ShowCheckpointSaved();
        Debug.Log($"Checkpoint saved to slot {GameSession.ActiveSaveSlot}: {lastCheckpoint}");
    }

    public void ResetRespawn()
    {
        controller.enabled = false;
        transform.position = lastCheckpoint + Vector3.up * 1.5f;
        controller.enabled = true;
    }

    public void ResumeFromDeath()
    {
        HideGameOverPanel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #region UI Panels
    void ShowCheckpointPanel()
    {
        if (checkpointPanel == null)
        {
            Debug.LogError("Checkpoint Panel not assigned!");
            return;
        }

        checkpointPanel.SetActive(true);
        Time.timeScale = 0;
        CheckpointPanelTriggered = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        characterController.enabled = false;
        Debug.Log("Checkpoint panel shown.");
    }

    public void HideCheckpointPanel()
    {
        if (checkpointPanel != null) checkpointPanel.SetActive(false);

        Time.timeScale = 1;
        CheckpointPanelTriggered = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterController.enabled = true;
        Debug.Log("Checkpoint panel hidden.");
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel == null)
        {
            Debug.LogError("Game Over Panel not assigned!");
            return;
        }

        gameOverPanel.SetActive(true);
        Time.timeScale = 0;
        GameOverPanelTriggered = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("GameOver panel shown.");
    }

    public void HideGameOverPanel()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        Time.timeScale = 1;
        GameOverPanelTriggered = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("GameOver panel hidden.");
    }
    #endregion
}

[System.Serializable]
public class CheckpointData
{
    public float x, y, z;
}

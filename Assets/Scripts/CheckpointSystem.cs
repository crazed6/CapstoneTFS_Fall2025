using UnityEngine;

public class CheckpointSystem : MonoBehaviour
{
    private Vector3 lastCheckpoint;
    private bool hasCheckpoint = false;

    void Start()
    {
        if (PlayerPrefs.HasKey("CheckpointX"))
        {
            float x = PlayerPrefs.GetFloat("CheckpointX");
            float y = PlayerPrefs.GetFloat("CheckpointY");
            float z = PlayerPrefs.GetFloat("CheckpointZ");
            lastCheckpoint = new Vector3(x, y, z);
            hasCheckpoint = true;
            Debug.Log("Loaded checkpoint at: " + lastCheckpoint);
        }
        else
        {
            lastCheckpoint = transform.position; // Default starting position
            Debug.Log("No checkpoint found, using default start position: " + lastCheckpoint);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X) && hasCheckpoint)
        {
            Debug.Log("Respawning at: " + lastCheckpoint);
            Respawn();
        }
    }

    void Respawn()
    {
        transform.position = lastCheckpoint;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            lastCheckpoint = other.transform.position;
            PlayerPrefs.SetFloat("CheckpointX", lastCheckpoint.x);
            PlayerPrefs.SetFloat("CheckpointY", lastCheckpoint.y);
            PlayerPrefs.SetFloat("CheckpointZ", lastCheckpoint.z);
            PlayerPrefs.Save();
            hasCheckpoint = true;
            Debug.Log("Checkpoint activated at: " + lastCheckpoint);
        }
    }
}

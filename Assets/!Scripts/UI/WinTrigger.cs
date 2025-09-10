using UnityEngine;
using UnityEngine.SceneManagement; // Needed for scene loading

public class WinTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering has the Player tag
        if (other.CompareTag("Player"))
        {
            // Load the scene called "WinCondition"
            SceneManager.LoadScene("WinCondition");
        }
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartLevel : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            RestartCurrentLevel();
        }
    }

    void RestartCurrentLevel()
    {
        // Reset time scale to normal in case the game was paused or slowed
        Time.timeScale = 1f;

        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
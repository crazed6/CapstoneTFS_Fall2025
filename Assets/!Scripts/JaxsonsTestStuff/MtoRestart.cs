using UnityEngine;
using UnityEngine.SceneManagement;

public class MtoRestart : MonoBehaviour
{
   
    void Update()
    {
        ReloadScene();
    }

    void ReloadScene ()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
        }
    }
}

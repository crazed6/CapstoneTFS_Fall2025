using UnityEngine;

public class StopTimeScale : MonoBehaviour
{
    public GameObject fadeImage;
    public void StopTime()
    {
        // Stop the time scale by setting it to 0
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true;                  // Make it visible

        // Optionally, you can also set fixedDeltaTime to 0 to stop physics updates
        //Destroy(fadeImage); // Destroy the fade image to stop the fade effect
    }

}

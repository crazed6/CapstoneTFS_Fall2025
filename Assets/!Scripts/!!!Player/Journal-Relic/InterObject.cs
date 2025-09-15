using UnityEngine;

public class InteractableMenu : MonoBehaviour
{
    public GameObject menuUI;   // Assign your menu UI in the Inspector
    public float interactDistance = 3f; // How close player needs to be

    private Transform player;
    private bool menuOpen = false;

    // Global flag so pause menu knows when this menu is active
    public static bool InteractableMenuActive = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        menuUI.SetActive(false); // Make sure menu starts hidden
    }

    void Update()
    {
        // If menu is open, allow E or Esc to close it
        if (menuOpen)
        {
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Escape))
            {
                CloseMenu();
            }
            return; // Block all other input
        }

        // Otherwise, allow normal interaction
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                OpenMenu();
            }
        }
    }

    void OpenMenu()
    {
        menuUI.SetActive(true);
        Time.timeScale = 0f; // Pause game
        menuOpen = true;
        InteractableMenuActive = true; // Tell other scripts we are active
        Cursor.lockState = CursorLockMode.None; // Show mouse
        Cursor.visible = true;
    }

    void CloseMenu()
    {
        menuUI.SetActive(false);
        Time.timeScale = 1f; // Resume game
        menuOpen = false;
        InteractableMenuActive = false;
        Cursor.lockState = CursorLockMode.Locked; // Hide mouse if using FPS controls
        Cursor.visible = false;
    }
}


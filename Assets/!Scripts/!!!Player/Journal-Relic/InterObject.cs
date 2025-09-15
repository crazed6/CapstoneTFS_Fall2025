using UnityEngine;

public class InteractableMenu : MonoBehaviour
{
    public GameObject menuUI;   // Assign your menu UI in the Inspector
    public float interactDistance = 3f; // How close player needs to be

    private Transform player;
    private bool menuOpen = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        menuUI.SetActive(false); // Make sure menu starts hidden
    }

    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= interactDistance)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (menuOpen)
                {
                    CloseMenu();
                }
                else
                {
                    OpenMenu();
                }
            }
        }
    }

    void OpenMenu()
    {
        menuUI.SetActive(true);
        Time.timeScale = 0f; // Pause game
        menuOpen = true;
        Cursor.lockState = CursorLockMode.None; // Show mouse
        Cursor.visible = true;
    }

    void CloseMenu()
    {
        menuUI.SetActive(false);
        Time.timeScale = 1f; // Resume game
        menuOpen = false;
        Cursor.lockState = CursorLockMode.Locked; // Hide mouse if using FPS controls
        Cursor.visible = false;
    }
}
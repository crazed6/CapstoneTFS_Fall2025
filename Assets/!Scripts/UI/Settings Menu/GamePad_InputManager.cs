using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GamePad_InputManager : MonoBehaviour
{
    public void BackToMainMenu()
    {
        // Logic to return to the main menu
        SceneManager.LoadScene("Kadeem_MainMenu");
        Debug.Log("Returning to Main Menu...");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {

    }
}

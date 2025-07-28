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

    void DefaultGamePadBindings()
    {
        // Set default keyboard bindings here
        var inputManager = InputManager.Instance; // Access the singleton instance of InputManager
        if (inputManager != null)
        {
            var jumpAction = inputManager.FindAction("Jump");
            if (jumpAction != null)
            {
                var jump = inputManager.FindAction("Jump");
                if (jump != null)
                    jump.ApplyBindingOverride("<Gamepad>/buttonSouth");

                var attack = inputManager.FindAction("Attack");
                if (attack != null)
                    attack.ApplyBindingOverride("<Gamepad>/buttonWest");

                inputManager.SaveRebinds();
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DefaultGamePadBindings(); // Set default gamepad bindings when the script starts
    }

    // Update is called once per frame
    void Update()
    {

    }
}

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Keyboard_InputManager : MonoBehaviour
{
    public void BackToMainMenu()
    {
        // Logic to return to the main menu
        SceneManager.LoadScene("Kadeem_MainMenu");
        Debug.Log("Returning to Main Menu...");
    }

    //void DefaultKeyboardBindings()
    //{
    //    // Set default keyboard bindings here
    //    var inputManager = InputManager.Instance; // Access the singleton instance of InputManager
    //    if (inputManager != null)
    //    {
    //        var jumpAction = inputManager.FindAction("Jump");
    //        if (jumpAction != null)
    //        {
    //            jumpAction.ApplyBindingOverride("<Keyboard>/space");

    //            inputManager.SaveRebinds(); // Save the new binding to PlayerPrefs

    //            Debug.Log("Default binding for 'Jump' set to Space key.");
    //        }
    //        else
    //        {
    //            Debug.LogWarning("Action 'Jump' not found in InputManager.");
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("InputManager instance is null.");
    //    }
    //}

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //DefaultKeyboardBindings(); // Initialize default bindings
    }

    // Update is called once per frame
    void Update()
    {

    }
}

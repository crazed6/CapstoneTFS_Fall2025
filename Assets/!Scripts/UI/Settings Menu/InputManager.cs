using TMPro;
using UnityEngine;
using System.IO;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{

    // Variables to store the current input settings
    public static InputManager Instance { get; private set; }
    // get the generated class InputSystem and assign my callback variable to access them.
    private InputSystem_Actions playerControls; // Generated input actions class
    private string currntCntrlScheme;
    private const string RebindingFileName = "input_rebinds.json";
    public string CurrntCntrlScheme => currntCntrlScheme;
    public bool IsUsingKeyboard => currntCntrlScheme == "Keyboard";
    public bool IsUsingGamepad => currntCntrlScheme == "Gamepad";

    private string FilePath => Path.Combine(Application.persistentDataPath, RebindingFileName);

    public bool IsInitialized { get; private set; } = false;

    public event System.Action OnBindingsLoaded; // Event to notify when bindings are loaded

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerControls = new InputSystem_Actions();
        playerControls.Enable();

        DetectInputScheme();
        LoadRebinds();

        IsInitialized = true;
        OnBindingsLoaded?.Invoke(); // Notify listeners that bindings are loaded and ready 
    }

    private void Start()
    {
        RebindManager.PrintAllBindingIndexes(); // Optional for debug
    }

    private void DetectInputScheme()
    {
        InputSystem.onAnyButtonPress.CallOnce(control =>
        {
            if (control.device is Gamepad)
                currntCntrlScheme = "Gamepad";
            else if (control.device is Keyboard || control.device is Mouse)
                currntCntrlScheme = "Keyboard";

            DetectInputScheme(); // Keep listening
        });
    }

    public InputAction FindAction(string actionName) =>
        playerControls.asset.FindAction(actionName, throwIfNotFound: false);

    public void SaveRebinds()
    {
        string json = playerControls.asset.SaveBindingOverridesAsJson();
        File.WriteAllText(FilePath, json);
        Debug.Log($"[InputManager] Rebindings saved to {FilePath}");
    }

    public void LoadRebinds()
    {
        if (File.Exists(FilePath))
        {
            string json = File.ReadAllText(FilePath);
            playerControls.asset.LoadBindingOverridesFromJson(json);
            Debug.Log("[InputManager] Loaded input bindings from file.");
        }
        else
        {
            Debug.Log("[InputManager] No binding file found. Using default bindings.");
        }
        OnBindingsLoaded?.Invoke(); // Notify listeners that bindings are loaded
    }

    public void ResetRebinds()
    {
        playerControls.asset.RemoveAllBindingOverrides();
        if (File.Exists(FilePath))
            File.Delete(FilePath);

        Debug.Log("[InputManager] Rebinds reset to defaults and file deleted.");
    }

    // Manual method to call from UI button to reload rebinds
    public void ReloadBindings()
    {
        LoadRebinds();
        Debug.Log("[InputManager] Reloaded bindings from file.");
    }

    // Method to update jump key display in UI

    public InputActionAsset GetAsset() => playerControls.asset;
}

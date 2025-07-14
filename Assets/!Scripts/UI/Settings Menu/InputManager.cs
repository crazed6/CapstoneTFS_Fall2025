using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

/// IDEA:
/// Central access to all input actions and rebind management.
/// Uses Unity's generated input wrapper class for more modularity.

public class InputManager : MonoBehaviour
{

    // Variables to store the current input settings
    public static InputManager Instance { get; private set; }
    // get the generated class InputSystem and assign my callback variable to access them.
    private InputSystem_Actions playerControls; // Generated input actions class
    // Key used for rebinding
    private const string RebindingSaveKey = "RebindingKey";
    private string currntCntrlScheme;

    public string CurrntCntrlScheme => currntCntrlScheme;
    public bool IsUsingKeyboard => currntCntrlScheme == "Keyboard";
    public bool IsUsingGamepad => currntCntrlScheme == "Gamepad";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Delay asset access slightly after reloads
        if (!Application.isPlaying) return;

        playerControls = new InputSystem_Actions();
        playerControls.Enable();

        DetectInputScheme();
        LoadRebinds();
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
            {
                currntCntrlScheme = "Gamepad";
                Debug.Log("Switched to Gamepad");
            }
            else if (control.device is Keyboard || control.device is Mouse)
            {
                currntCntrlScheme = "Keyboard";
                Debug.Log("Switched to Keyboard & Mouse");
            }

            DetectInputScheme(); // Continue listening
        });
    }

    public InputAction FindAction(string actionName)
    {
        return playerControls.asset.FindAction(actionName, throwIfNotFound: false);
    }

    public void SaveRebinds()
    {
        string json = playerControls.asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(RebindingSaveKey, json);
    }

    public void LoadRebinds()
    {
        string json = PlayerPrefs.GetString(RebindingSaveKey, string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            playerControls.asset.LoadBindingOverridesFromJson(json);
        }
    }

    public void ResetRebinds()
    {
        playerControls.asset.RemoveAllBindingOverrides();
        PlayerPrefs.DeleteKey(RebindingSaveKey);
    }

    public InputActionAsset GetAsset()
    {
        return playerControls.asset;
    }
}

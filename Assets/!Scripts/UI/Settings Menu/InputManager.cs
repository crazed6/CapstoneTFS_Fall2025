using UnityEngine;
using UnityEngine.InputSystem;

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

    void Start()
    {
        RebindManager.PrintAllBindingIndexes();
    }


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

        LoadRebinds();
    }

    // Find input action by name.
    public InputAction FindAction(string actionName)
    {
        return playerControls.asset.FindAction(actionName, throwIfNotFound: false);
    }

    // Save rebinding overrides as JSON string.
    public void SaveRebinds()
    {
        string json = playerControls.asset.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(RebindingSaveKey, json);
    }

    // Load rebinding overrides from saved JSON string.
    public void LoadRebinds()
    {
        string json = PlayerPrefs.GetString(RebindingSaveKey, string.Empty);
        if (!string.IsNullOrEmpty(json))
        {
            playerControls.asset.LoadBindingOverridesFromJson(json);
        }
    }

    // Reset all rebinding overrides and clear saved data.
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

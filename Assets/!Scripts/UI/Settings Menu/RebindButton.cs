using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

// Attach to a Keybind row (e.g., Move Forward).  
// When the user clicks the button, starts an interactive rebind,
// updates the label, and persists the result.

public class RebindButton : MonoBehaviour
{
    [Header("Input Action")]
    [Tooltip("The action name exactly as it appears in the Input Actions asset")]
    public string actionName;

    [Tooltip("Which binding slot in that action this row represents")]
    public int bindingIndex;          // e.g., 1 = W (Move Forward)

    [Header("UI References")]
    public TMP_Text bindingDisplay;       // The TMP text that shows the key
    public Button triggerButton;        // The button the player clicks

    private void Awake()
    {
        // Hook click -> start rebind
        triggerButton.onClick.AddListener(() =>
        {
            RebindManager.StartRebind(actionName, bindingIndex, bindingDisplay, UpdateDisplay);
        });
    }

    private void OnEnable()
    {
       UsedKeyRegistry.RefreshUsedKeys(); // make sure used keys are up to date

        if (InputManager.Instance != null)
            UpdateDisplay();
        else
            StartCoroutine(WaitForInputManager());

        var action = InputManager.Instance.FindAction(actionName);
        if (action == null || bindingIndex >= action.bindings.Count) return;

        string path = action.bindings[bindingIndex].effectivePath;
        if (!string.IsNullOrEmpty(path) && UsedKeyRegistry.IsKeyUsed(path))
        {
            // Allow this one (it's this button's binding)
            return;
        }

        // If the key is used by another, gray out and disable
        if (UsedKeyRegistry.IsKeyUsed(path))
        {
            bindingDisplay.color = Color.gray;
            triggerButton.interactable = false;
        }
        else
        {
            bindingDisplay.color = Color.white;
            triggerButton.interactable = true;
        }
    }

    private System.Collections.IEnumerator WaitForInputManager()
    {
        yield return new WaitUntil(() => InputManager.Instance != null);
        UpdateDisplay();
    }

    /// Refresh the key label from the current binding.
    private void UpdateDisplay()
    {
        var action = InputManager.Instance.FindAction(actionName);
        if (action == null || bindingIndex >= action.bindings.Count) return;

        bindingDisplay.text = InputControlPath.ToHumanReadableString(
            action.bindings[bindingIndex].effectivePath,
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );
    }
}

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
        if (InputManager.Instance != null)
            UpdateDisplay();
        else
            StartCoroutine(WaitForInputManager());
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

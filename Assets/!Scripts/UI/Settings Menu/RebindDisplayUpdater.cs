using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindDisplayUpdater : MonoBehaviour
{
    public string actionName;
    public int bindingIndex;
    public TMP_Text displayText;

    private void Start()
    {
        if (displayText == null)
        {
            displayText = GetComponent<TMP_Text>();
        }

        if (string.IsNullOrEmpty(actionName) || bindingIndex < 0)
        {
            Debug.LogError("Action name or binding index is not set correctly.");
            return;
        }
        UpdateKeyDisplay();
    }

    public void UpdateKeyDisplay()
    {
        var action = InputManager.Instance.FindAction(actionName);
        if (action != null && bindingIndex < action.bindings.Count)
        {
            displayText.text = InputControlPath.ToHumanReadableString(
                action.bindings[bindingIndex].effectivePath,
                InputControlPath.HumanReadableStringOptions.OmitDevice
            );
        }
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
            UpdateKeyDisplay();
        else
            StartCoroutine(WaitForInputManager());
    }

    private System.Collections.IEnumerator WaitForInputManager()
    {
        yield return new WaitUntil(() => InputManager.Instance != null);
        UpdateKeyDisplay();
    }
}

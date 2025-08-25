using TMPro;
using UnityEngine;
using System.Collections;
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

            if (displayText == null)
            {
                Debug.LogError("TMP_Text component not found or assigned.");
                return;
            }
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
        else
        {
            Debug.LogWarning("Action not found or bindingIndex out of range.");
            Debug.LogError($"Action '{actionName}' not found or binding index {bindingIndex} is out of range.");
            displayText.text = "Not Bound";
        }
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
            UpdateKeyDisplay();
        else
            StartCoroutine(WaitForInputManager());
    }

    private IEnumerator WaitForInputManager()
    {
        yield return new WaitUntil(() => InputManager.Instance != null && InputManager.Instance.FindAction(actionName) != null);
        UpdateKeyDisplay();
    }
}

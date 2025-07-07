using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class RebindDisplayUpdater : MonoBehaviour
{
    public string actionName;
    public int bindingIndex;
    public TMP_Text displayText;

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
        UpdateKeyDisplay();
    }
}

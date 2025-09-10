using UnityEngine;
using UnityEngine.UI;

public class ToggleDropdownHighlighter : MonoBehaviour
{
    [Tooltip("Assign Template/Viewport/Content here (where the Toggle items live)")]
    public RectTransform content;

    [Tooltip("Sprite to show when an item is selected")]
    public Sprite highlightSprite;

    [Tooltip("Sprite to use for unselected items (optional)")]
    public Sprite defaultSprite;

    [Tooltip("Assign the dropdown's Arrow Image if you want it tinted white automatically")]
    public Image arrowImage;

    [Tooltip("Checkmark tint (set to white)")]
    public Color checkmarkColor = Color.white;

    void Start()
    {
        if (content == null)
        {
            Debug.LogError("Content not assigned. Drag Template/Viewport/Content into the inspector.", this);
            return;
        }

        // Subscribe to toggles and set checkmark color
        Toggle[] toggles = content.GetComponentsInChildren<Toggle>(true);
        foreach (var t in toggles)
        {
            var captured = t; // capture for lambda
            captured.onValueChanged.AddListener((isOn) => OnToggleChanged(captured, isOn));

            // Prefer using the Toggle.graphic (checkmark) if assigned
            if (captured.graphic != null)
            {
                var img = captured.graphic as Image;
                if (img != null) img.color = checkmarkColor;
            }
            else // fallback: try finding child named "Item Checkmark"
            {
                var check = captured.transform.Find("Item Checkmark");
                if (check != null)
                {
                    var img = check.GetComponent<Image>();
                    if (img != null) img.color = checkmarkColor;
                }
            }
        }

        if (arrowImage != null) arrowImage.color = Color.white;

        // initialize visuals
        foreach (var t in toggles)
            SetHighlight(t, t.isOn);
    }

    void OnToggleChanged(Toggle togg, bool isOn)
    {
        if (isOn)
        {
            // Clear others (useful if toggles are single-select)
            foreach (Transform child in content)
            {
                var childToggle = child.GetComponent<Toggle>();
                if (childToggle != null && childToggle != togg)
                    SetHighlight(childToggle, false);
            }
            SetHighlight(togg, true);
        }
        else
        {
            SetHighlight(togg, false);
        }
    }

    void SetHighlight(Toggle togg, bool on)
    {
        Image bgImage = null;

        // Prefer the Toggle's targetGraphic if it's set to the Item Background
        if (togg.targetGraphic != null) bgImage = togg.targetGraphic as Image;

        // Otherwise search for a child named "Item Background"
        if (bgImage == null)
        {
            var bg = togg.transform.Find("Item Background");
            if (bg != null) bgImage = bg.GetComponent<Image>();
        }

        if (bgImage != null)
            bgImage.sprite = on && highlightSprite != null ? highlightSprite : defaultSprite;
    }
}

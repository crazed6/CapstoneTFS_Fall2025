using UnityEngine;
using TMPro;

public class LookAtPlayerTextSmooth : MonoBehaviour
{
    public GameObject player;                 // Reference to the player
    public TextMeshPro textMeshPro;           // The TextMeshPro component (3D Text)
    public float rotationSpeed = 5f;          // Rotation speed for text facing player
    public float fadeSpeed = 2f;              // How fast text fades in/out
    public float visibleDistance = 70f;       // Distance threshold for visibility

    private Color originalColor;
    private float targetAlpha = 0f;           // Desired alpha (0 = invisible, 1 = fully visible)

    void Start()
    {
        // Store original text color but start invisible
        originalColor = textMeshPro.color;
        textMeshPro.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);

        // ✅ Shadow (using outline)
        textMeshPro.fontMaterial.SetFloat(ShaderUtilities.ID_OutlineWidth, 0.2f);
        textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor, new Color(0f, 0f, 0f, 0.6f));
    }

    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);

        // Decide target visibility
        targetAlpha = (distance <= visibleDistance) ? 1f : 0f;

        // Smooth fade
        Color currentColor = textMeshPro.color;
        float newAlpha = Mathf.Lerp(currentColor.a, targetAlpha, fadeSpeed * Time.deltaTime);
        textMeshPro.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);

        // Apply same alpha to shadow
        Color outlineColor = textMeshPro.fontMaterial.GetColor(ShaderUtilities.ID_OutlineColor);
        textMeshPro.fontMaterial.SetColor(ShaderUtilities.ID_OutlineColor,
            new Color(outlineColor.r, outlineColor.g, outlineColor.b, newAlpha * 0.6f));

        // Rotate text toward player if visible
        if (newAlpha > 0.01f)
        {
            LookAtPlayer();
        }
    }

    void LookAtPlayer()
    {
        Vector3 directionToPlayer = player.transform.position - textMeshPro.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
        targetRotation *= Quaternion.Euler(0, 180, 0); // flip
        textMeshPro.transform.rotation = Quaternion.Slerp(textMeshPro.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}

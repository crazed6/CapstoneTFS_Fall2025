using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// Diego & Joshua's segmented Momentum UI
public class MomentumBar : MonoBehaviour
{
    [Header("References")]
    public RectTransform maskTransform;  // The moving mask
    public Image[] slots;                // 10 slot images inside the mask
    public CharacterController playerController;

    [Header("Speed Settings")]
    public float maxSpeed = 30f;

    [Header("Color Settings")]
    public Color normalColor = Color.green;
    public Color maxedColor = Color.cyan;

    private float originalWidth;

    void Start()
    {
        if (maskTransform != null)
            originalWidth = maskTransform.sizeDelta.x;

        if (playerController == null)
            playerController = CharacterController.instance;

        if (playerController != null)
            maxSpeed = playerController.maxSpeed;

        // Initialize slot colors
        foreach (var slot in slots)
        {
            if (slot != null)
                slot.color = normalColor;
        }
    }

    void Update()
    {
        if (playerController == null || maskTransform == null || slots == null || slots.Length == 0)
            return;

        // Get horizontal speed
        Vector3 velocity = playerController.rb.linearVelocity;
        Vector2 horizontalVelocity = new Vector2(velocity.x, velocity.z);
        float speed = horizontalVelocity.magnitude;

        // Normalize (0–1)
        float normalized = Mathf.Clamp01(speed / maxSpeed);

        // Resize mask like HealthUI
        float targetWidth = originalWidth * normalized;
        maskTransform.sizeDelta = new Vector2(targetWidth, maskTransform.sizeDelta.y);

        // Smoothly update slot colors
        Color targetColor = Color.Lerp(normalColor, maxedColor, normalized);
        foreach (var slot in slots)
        {
            if (slot != null)
                slot.DOColor(targetColor, 0.1f);
        }
    }
}

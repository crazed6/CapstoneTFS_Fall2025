using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MomentumBar : MonoBehaviour
{
    // Reference to the Image component used for the fill amount (inside the mask)
    public Image fillImage;

    // Reference to the CharacterController script to access the player's speed
    public CharacterController playerController;

    [Header("Speed Settings")]
    // The maximum speed that the player can reach; used to normalize current speed
    public float maxSpeed = 30f;

    [Header("Color Settings")]
    // Color of the bar when at low or normal speed
    public Color normalColor = Color.green;

    // Color of the bar when at max speed (momentum maxed)
    public Color maxedColor = Color.cyan;

    private void Start()
    {
        // Auto-assign player controller if it's not set in the Inspector
        if (playerController == null)
            playerController = CharacterController.instance;

        // If we successfully found the player controller, use its maxSpeed
        if (playerController != null)
            maxSpeed = playerController.maxSpeed;

        // Set the initial color of the bar
        fillImage.color = normalColor;
    }

    void Update()
    {
        // Return early if required references are missing
        if (playerController == null || fillImage == null) return;

        // Get the player's velocity (from their Rigidbody)
        Vector3 velocity = playerController.rb.linearVelocity;

        // Calculate horizontal speed only (ignore vertical velocity like jumping/falling)
        Vector2 horizontalVelocity = new Vector2(velocity.x, velocity.z);
        float speed = horizontalVelocity.magnitude;

        // Normalize the speed between 0 and 1
        float normalized = Mathf.Clamp01(speed / maxSpeed);

        // Animate the fill amount of the bar using DOTween (smooth transition)
        fillImage.DOFillAmount(normalized, 0.1f);

        // Lerp the color between normalColor and maxedColor based on normalized speed
        Color targetColor = Color.Lerp(normalColor, maxedColor, normalized);

        // Animate the color change smoothly using DOTween
        fillImage.DOColor(targetColor, 0.1f);
    }
}

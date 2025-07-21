using UnityEngine;
using UnityEngine.UI;

public class UISpeedEffect : MonoBehaviour
{
    public CharacterController playerController;
    public float speedThreshold = 45f;
    public float pulseSpeed = 2f; // Controls how fast the pulse oscillates
    public float pulseScaleAmount = 0.2f; // How much the image scales during pulse
    public float fadeSpeed = 5f;

    private Image speedImage;
    private Vector3 baseScale;

    void Start()
    {
        speedImage = GetComponent<Image>();
        baseScale = transform.localScale;

        if (playerController == null)
            playerController = CharacterController.instance;
    }

    void Update()
    {
        if (playerController == null || speedImage == null)
            return;

        // Get player horizontal speed
        Vector3 velocity = playerController.rb.linearVelocity;
        float speed = new Vector2(velocity.x, velocity.z).magnitude;

        if (speed >= speedThreshold)
        {
            // Pulsate effect by modifying scale with sine wave
            float pulse = 1 + Mathf.Sin(Time.time * pulseSpeed) * pulseScaleAmount;
            transform.localScale = baseScale * pulse;

            // Fade in the image
            speedImage.color = Color.Lerp(speedImage.color, new Color(1, 1, 1, 0.5f), Time.deltaTime * fadeSpeed);
        }
        else
        {
            // Reset to base scale
            transform.localScale = baseScale;

            // Fade out the image
            speedImage.color = Color.Lerp(speedImage.color, new Color(1, 1, 1, 0f), Time.deltaTime * fadeSpeed);
        }
    }
}

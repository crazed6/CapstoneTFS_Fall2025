using UnityEngine;
using UnityEngine.UI;

public class CrosshairSwitcher : MonoBehaviour
{
    public Rigidbody playerRigidbody; // Reference to the player's Rigidbody
    public CharacterController characterController; // Reference to CharacterController
    public Image crosshair1; // First crosshair image
    public Image crosshair2; // Second crosshair image
    public float speedThreshold = 10f; // Speed threshold to switch crosshairs

    private Vector3 lastPosition;
    private float currentSpeed;

    void Start()
    {
        lastPosition = transform.position;
    }

    void Update()
    {
        if (playerRigidbody != null)
        {
            currentSpeed = playerRigidbody.linearVelocity.magnitude;
        }
        else if (characterController != null)
        {
            // Calculate speed manually for CharacterController
            currentSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
            lastPosition = transform.position;
        }

        // Toggle crosshairs based on speed
        if (currentSpeed > speedThreshold)
        {
            crosshair1.enabled = false;
            crosshair2.enabled = true;
        }
        else
        {
            crosshair1.enabled = true;
            crosshair2.enabled = false;
        }
    }
}

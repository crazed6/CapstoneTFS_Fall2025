using UnityEngine;
using System.Collections;

public class OilSlickTrap : MonoBehaviour
{
    public float slipFactor = 0.5f; // How much the player's speed is reduced (sliding effect)
    private bool playerOnSlick = false;
    private CharacterController characterController;
    private Rigidbody playerRb;
    private float originalSpeed;
    private bool isSliding = false; // To track if the player is sliding
    private Vector3 initialSlidingDirection; // Store the direction the player is looking when they enter the trap

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            characterController = other.GetComponent<CharacterController>();
            playerRb = other.GetComponent<Rigidbody>();

            if (characterController != null)
            {
                originalSpeed = characterController.baseSpeed; // Store the original speed
                playerOnSlick = true;

                // Store the direction the player is facing when they enter the trap
                initialSlidingDirection = characterController.transform.forward;
                isSliding = true;
                Debug.Log("Player entered slick, sliding starts.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerOnSlick = false;
            StopSliding(); // Stop sliding when player exits
            Debug.Log("Player exited slick, sliding stops.");
        }
    }

    // Apply sliding velocity in the initial direction while the player is on the slick
    private void ApplySlidingVelocity()
    {
        if (playerRb != null && characterController != null)
        {
            // Apply velocity only in the direction the player was facing when they entered the trap
            Vector3 intendedVelocity = initialSlidingDirection * (originalSpeed * slipFactor);

            // Ensure that there is no backward or forward movement based on input
            // Set the horizontal velocity to zero, only applying sliding effect
            playerRb.linearVelocity = new Vector3(intendedVelocity.x, playerRb.linearVelocity.y, intendedVelocity.z);

            // Debug for checking
            Debug.Log("Player is slipping");
        }
    }

    // Stop the sliding effect and reset velocity
    private void StopSliding()
    {
        if (isSliding)
        {
            if (characterController != null)
            {
                characterController.baseSpeed = originalSpeed; // Reset the original speed
            }

            if (playerRb != null)
            {
                // Stop any lingering sliding velocity
                playerRb.linearVelocity = new Vector3(playerRb.linearVelocity.x, playerRb.linearVelocity.y, 0);
            }

            isSliding = false;
        }
    }

    // Continuously apply the sliding effect when the player is on the slick
    void Update()
    {
        if (playerOnSlick && isSliding)
        {
            // Apply sliding velocity while the player is on the slick
            ApplySlidingVelocity();
        }
    }
}
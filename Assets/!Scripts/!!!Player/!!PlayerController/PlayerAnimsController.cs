using UnityEngine;

public class PlayerAnimsController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;       // Reference to the Animator
    public Rigidbody rb;            // Reference to the Rigidbody

    [Header("Settings")]
    public float speedThreshold = 0.1f; // Minimum speed to count as running

    void Reset()
    {
        // Auto-assign references if not set
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Get horizontal movement speed (ignoring vertical velocity)
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float speed = horizontalVelocity.magnitude;

        // Pass speed to Animator (could be a float or bool)
        animator.SetFloat("Velo", speed);

    }
}
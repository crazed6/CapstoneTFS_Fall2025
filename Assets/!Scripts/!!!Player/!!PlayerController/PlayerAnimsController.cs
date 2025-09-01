using UnityEngine;

public class PlayerAnimsController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;       // Reference to the Animator
    public Rigidbody rb;            // Reference to the Rigidbody
    public CharacterController cc;

    [Header("Settings")]
    public float speedThreshold = 0.1f; // Minimum speed to count as running
    void Reset()
    {
        // Auto-assign references if not set
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (cc == null) cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Get horizontal movement speed (ignoring vertical velocity)
        Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float speed = horizontalVelocity.magnitude;

        animator.SetFloat("Velo", speed);

        animator.SetBool("IsSliding", cc.IsSliding);

        if (cc.isVaulting)
        {
            animator.Play("KF_PoleVault");
        }
    }
}
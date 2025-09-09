using UnityEngine;

public class PlayerAnimsController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;       // Reference to the Animator
    public Rigidbody rb;            // Reference to the Rigidbody
    public CharacterController cc;

    [Header("Settings")]
    public float speedThreshold = 0.1f; // Minimum speed to count as running

    private bool fallingTriggered;
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

        animator.SetFloat("Velo", cc.IsGrounded ? speed : 0f);

        animator.SetBool("IsSliding", cc.IsSliding);

        animator.SetBool("IsWallLeft", cc.IsWallRunLeft);

        animator.SetBool("IsWallRight", cc.IsWallRunRight);

        animator.SetBool("IsJumping", cc.IsJumping);

        if (cc.isVaulting)
        {
            animator.Play("KF_PoleVault");
        }

        if (!cc.IsGrounded && rb.linearVelocity.y < -0.1f)
        { 
            animator.SetBool("IsFalling", true);
        }
        else
        {
            animator.SetBool("IsFalling", false);
        }



        if (cc.IsGrounded)
        {
            animator.SetBool("IsGrounded", true);
        }
        else
        {
            animator.SetBool("IsGrounded", false);
        }
    }
}
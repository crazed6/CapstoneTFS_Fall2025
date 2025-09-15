using UnityEngine;
using System.Collections;

public class PlayerAnimsController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;       // Reference to the Animator
    public Rigidbody rb;            // Reference to the Rigidbody
    public CharacterController cc;
    public PlayerJavelinThrow jt;

    [Header("Sticks")]
    public JavelinRespawnVFXController backStick; // VFX-based back stick
    public GameObject handStick;

    private bool isVaultingPrev = false;
    private bool isThrowPrev = false;
    private bool isAttackingPrev = false;

    [Header("Settings")]
    public float speedThreshold = 0.1f; // Minimum speed to count as running

    void Reset()
    {
        // Auto-assign references if not set
        if (animator == null) animator = GetComponent<Animator>();
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (cc == null) cc = GetComponent<CharacterController>();
        if (jt == null) jt = GetComponent<PlayerJavelinThrow>();
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
        animator.SetBool("IsVaulting", cc.isVaulting);
        animator.SetBool("IsThrow", jt.IsHolding);
        animator.SetBool("IsDashing", cc.IsDashing);
        animator.SetBool("IsDashForward", cc.IsDashForward);

        // Handle stick visibility based on player state
        HandleStickVisibility();

        // Falling/Grounded logic
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

    private void HandleStickVisibility()
    {
        // Vaulting state change
        if (cc.isVaulting != isVaultingPrev)
        {
            if (cc.isVaulting)
            {
                // Hide back stick with VFX, show hand stick
                if (backStick != null) backStick.HideInstant();
                if (handStick != null) handStick.SetActive(true);
            }
            else if (!jt.IsHolding) // Only show back stick if not currently throwing
            {
                // Show back stick with dissolve effect, hide hand stick
                if (backStick != null) backStick.Respawn();
                if (handStick != null) handStick.SetActive(false);
            }
            isVaultingPrev = cc.isVaulting;
        }

        // Throwing state change
        if (jt.IsHolding != isThrowPrev)
        {
            if (jt.IsHolding)
            {
                // Hide back stick with VFX, show hand stick
                if (backStick != null) backStick.HideInstant();
                if (handStick != null) handStick.SetActive(true);
            }
            else if (!cc.isVaulting) // Only show back stick if not currently vaulting
            {
                // Show back stick with dissolve effect, hide hand stick
                if (backStick != null) backStick.Respawn();
                if (handStick != null) handStick.SetActive(false);
            }
            isThrowPrev = jt.IsHolding;
        }



        // Detect when dashing starts
        if (cc.IsDashing && !isAttackingPrev)
        {
            // Hide back stick instantly, show hand stick
            if (backStick != null) backStick.HideInstant();
            if (handStick != null) handStick.SetActive(true);

            // Start coroutine to wait for dash animation to end
            StartCoroutine(WaitForDashToEnd("KF_Dash_Attack 0"));

            isAttackingPrev = true;
        }

        // Reset state tracker when dash fully ends
        if (!cc.IsDashing && isAttackingPrev == true)
        {
            isAttackingPrev = false;
        }


    }

    private IEnumerator WaitForDashToEnd(string dashAnimation)
    {
        // Wait until the animator is actually playing the dash anim
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(dashAnimation));

        // Stay in hand stick until dash animation finishes
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).IsName(dashAnimation) &&
                                         animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);

        // Only restore stick if player isn’t vaulting
        if (!cc.IsDashing)
        {
            if (backStick != null) backStick.Respawn();
            if (handStick != null) handStick.SetActive(false);
        }
    }
}
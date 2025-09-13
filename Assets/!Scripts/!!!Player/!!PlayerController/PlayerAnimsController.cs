using UnityEngine;
using System.Collections;

public class PlayerAnimsController : MonoBehaviour
{
    [Header("References")]
    public Animator animator;       // Reference to the Animator
    public Rigidbody rb;            // Reference to the Rigidbody
    public CharacterController cc;
    public PlayerJavelinThrow jt;

    public GameObject backStick;
    public GameObject handStick;

    private bool isVaultingPrev = false;
    private bool isThrowPrev = false;

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

        // Only swap when state changes
        if (cc.isVaulting != isVaultingPrev)
        {
            if (cc.isVaulting)
            {
                // Hide back stick, show hand stick
                backStick.SetActive(false);
                handStick.SetActive(true);
            }
            else
            {
                // Show back stick, hide hand stick
                backStick.SetActive(true);
                handStick.SetActive(false);
            }

            isVaultingPrev = cc.isVaulting;
        }

        // Only swap when state changes
        if (jt.IsHolding != isThrowPrev)
        {
            if (jt.IsHolding)
            {
                // Hide back stick, show hand stick
                backStick.SetActive(false);
                handStick.SetActive(true);
            }
            else
            {
                // Show back stick, hide hand stick
                backStick.SetActive(true);
                handStick.SetActive(false);
            }

            isThrowPrev = jt.IsHolding;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("enemy"))
        {
            // Play animation trigger
            animator.SetTrigger("HitEnemy");

            // Only handle stick swap if animation really plays
            StartCoroutine(HandleStickSwap("KF_Dash_Attack 0"));
        }
    }

    private IEnumerator HandleStickSwap(string animationName)
    {
        // Wait until animator switches to the animation
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName(animationName));

        // Now the animation is playing → swap sticks
        backStick.SetActive(false);
        handStick.SetActive(true);

        // Wait until animation finishes
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);

        // Reset sticks
        backStick.SetActive(true);
        handStick.SetActive(false);
    }


}
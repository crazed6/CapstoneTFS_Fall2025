
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FootIKController : MonoBehaviour
{
    private Animator animator;

    [Header("IK Settings")]
    public bool enableIK = true;
    public float footRaycastDistance = 1.2f;
    public LayerMask groundLayer;

    // Optional: offsets
    public float footHeightOffset = 0.1f;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null || !enableIK) return;

        AdjustFootIK(AvatarIKGoal.LeftFoot);
        AdjustFootIK(AvatarIKGoal.RightFoot);
    }

    private void AdjustFootIK(AvatarIKGoal foot)
    {
        Vector3 footPosition;
        Quaternion footRotation;

        // get current foot position from animator
        footPosition = animator.GetIKPosition(foot);
        footRotation = animator.GetIKRotation(foot);

        // cast ray downward to find the ground
        RaycastHit hit;
        if (Physics.Raycast(footPosition + Vector3.up, Vector3.down, out hit, footRaycastDistance, groundLayer))
        {
            Vector3 hitPoint = hit.point;
            hitPoint.y += footHeightOffset;

            animator.SetIKPositionWeight(foot, 1f);
            animator.SetIKRotationWeight(foot, 1f);
            animator.SetIKPosition(foot, hitPoint);
            animator.SetIKRotation(foot, Quaternion.LookRotation(transform.forward, hit.normal));
        }
        else
        {
            // No ground found – lower weights
            animator.SetIKPositionWeight(foot, 0);
            animator.SetIKRotationWeight(foot, 0);
        }
    }
}
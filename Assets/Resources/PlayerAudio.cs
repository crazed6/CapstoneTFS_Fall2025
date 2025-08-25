using System.Collections;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    [Header("Run / Footsteps")]
    public string runStepId = "RunF";   // CSV key for footsteps
    public float baseStepInterval = 0.40f;
    public float minStepInterval = 0.20f;
    public float speedForMinInterval = 20f;

    private bool runLoopActive = false;
    private Coroutine runLoopRoutine;

    // One-shot hooks
    public void PlayLandSFX() => AudioManager.instance.PlaySFX("Land");
    public void PlaySlideSFX() => AudioManager.instance.PlaySFX("Slide");
    public void PlayDashSFX() => AudioManager.instance.PlaySFX("Dash");
    public void PlayPoleVaultSFX() => AudioManager.instance.PlaySFX("PoleVault");

    // Called from CharacterController.Move()
    public void StartRunLoop()
    {
        if (runLoopActive) return;
        runLoopActive = true;
        runLoopRoutine = StartCoroutine(RunLoop());
    }

    public void StopRunLoop()
    {
        if (!runLoopActive) return;
        runLoopActive = false;
        if (runLoopRoutine != null)
        {
            StopCoroutine(runLoopRoutine);
            runLoopRoutine = null;
        }
    }

    private IEnumerator RunLoop()
    {
        CharacterController ctrl = GetComponent<CharacterController>();

        while (runLoopActive)
        {
            if (ctrl == null)
            {
                yield return null;
                continue;
            }

            // Horizontal speed
            Vector3 hv = ctrl.displacement;
            hv.y = 0f;
            float speed = hv.magnitude;

            // Only play footsteps if moving and grounded
            if (speed > 0.1f && ctrl.IsGrounded)
            {
                // Faster movement → shorter interval
                float t = Mathf.InverseLerp(0f, speedForMinInterval, speed);
                float stepInterval = Mathf.Lerp(baseStepInterval, minStepInterval, t);

                AudioManager.instance.PlaySFX(runStepId);

                yield return new WaitForSeconds(stepInterval);
            }
            else
            {
                // Not moving or not grounded → wait a short time before checking again
                yield return null;
            }
        }
    }
}
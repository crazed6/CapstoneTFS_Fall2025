using UnityEngine;
using Cinemachine;

//Aiden Witjes

public class CinemachineCameraController : MonoBehaviour
{
    [Header("References")]
    public CinemachineVirtualCamera leftWallCam;
    public CinemachineVirtualCamera rightWallCam;
    public CinemachineFreeLook freeLookCam;
    public Transform cameraFollowTarget;
    public Transform lookAtTarget;
    public Transform leftWallLookAtTarget;
    public Transform rightWallLookAtTarget;
    public Transform playerTransform;

    [Header("FOV Settings")]
    public float defaultFOV = 60f;
    public float slideFOV = 75f;

    [Header("Lerp Speeds")]
    public float fovLerpSpeed = 5f;
    public float horizontalSyncSpeed = 10f; // How fast to sync horizontal rotation

    [Header("Wall Running Camera Settings")]
    public float wallRunTiltAngle = 20f;
    public float tiltLerpSpeed = 3f;

    private float targetFOV;
    private float targetLeftDutch = 0f;
    private float targetRightDutch = 0f;

    public enum PlayerState
    {
        Default,
        Sliding,
        WallRunningLeft,
        WallRunningRight
    }

    private PlayerState currentState = PlayerState.Default;

    void Start()
    {
        if (leftWallCam == null || rightWallCam == null || freeLookCam == null)
        {
            Debug.LogError("One or more cameras are not assigned.");
            return;
        }

        // Assign Follow/LookAt
        leftWallCam.Follow = cameraFollowTarget;
        leftWallCam.LookAt = leftWallLookAtTarget != null ? leftWallLookAtTarget : lookAtTarget;
        rightWallCam.Follow = cameraFollowTarget;
        rightWallCam.LookAt = rightWallLookAtTarget != null ? rightWallLookAtTarget : lookAtTarget;
        freeLookCam.Follow = cameraFollowTarget;
        freeLookCam.LookAt = lookAtTarget;

        SetupFreeLookCamera();
        SetCameraPriorities(); // Start with FreeLook
        targetFOV = defaultFOV;
    }

    void Update()
    {
        if (freeLookCam.Priority > 0)
        {
            SyncHorizontalToPlayer();

            if (currentState == PlayerState.Sliding)
            {
                freeLookCam.m_Lens.FieldOfView = Mathf.Lerp(freeLookCam.m_Lens.FieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
            }
        }

        // Smooth Dutch angle transitions for wall cameras
        leftWallCam.m_Lens.Dutch = Mathf.Lerp(leftWallCam.m_Lens.Dutch, targetLeftDutch, Time.deltaTime * tiltLerpSpeed);
        rightWallCam.m_Lens.Dutch = Mathf.Lerp(rightWallCam.m_Lens.Dutch, targetRightDutch, Time.deltaTime * tiltLerpSpeed);
    }

    private void SyncHorizontalToPlayer()
    {
        freeLookCam.m_XAxis.Value = playerTransform.eulerAngles.y;
    }

    private void SetupFreeLookCamera()
    {
        // Configure horizontal axis - we'll control this programmatically
        freeLookCam.m_XAxis.m_InputAxisName = ""; // Disable mouse input for horizontal
        freeLookCam.m_XAxis.m_MaxSpeed = 0f; // No automatic speed
        freeLookCam.m_XAxis.m_Wrap = true; // Allow wrapping for 360 degree rotation
        freeLookCam.m_XAxis.m_MinValue = -180f; // Standard range
        freeLookCam.m_XAxis.m_MaxValue = 180f;
        freeLookCam.m_XAxis.m_Recentering.m_enabled = false; // Don't auto-center

        // Vertical input (Mouse Y) - this remains normal for up/down look
        freeLookCam.m_YAxis.m_InputAxisName = "Mouse Y";
        freeLookCam.m_YAxis.m_InputAxisValue = 0.5f; // Start in middle
        freeLookCam.m_YAxis.m_MaxSpeed = 2f;
        freeLookCam.m_YAxis.m_AccelTime = 0.1f;
        freeLookCam.m_YAxis.m_DecelTime = 0.1f;
        freeLookCam.m_YAxis.m_MinValue = 0f;
        freeLookCam.m_YAxis.m_MaxValue = 1f;
        freeLookCam.m_YAxis.m_Wrap = false;
        freeLookCam.m_YAxis.m_Recentering.m_enabled = false;

        // FOV and binding
        freeLookCam.m_Lens.FieldOfView = defaultFOV;
        freeLookCam.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;

        // Initialize X-axis to match player's current rotation
        if (playerTransform != null)
        {
            freeLookCam.m_XAxis.Value = playerTransform.eulerAngles.y;
        }
    }

    public void UpdateCameraState(PlayerState newState)
    {
        if (newState == currentState) return;
        currentState = newState;

        switch (newState)
        {
            case PlayerState.Sliding:
                SetCameraPriorities(); // FreeLook stays active
                targetFOV = slideFOV;
                break;

            case PlayerState.WallRunningLeft:
                SetCameraPriorities(leftWallCam);
                targetLeftDutch = wallRunTiltAngle; // Tilt clockwise
                break;

            case PlayerState.WallRunningRight:
                SetCameraPriorities(rightWallCam);
                targetRightDutch = -wallRunTiltAngle; // Tilt counter-clockwise
                break;

            case PlayerState.Default:
            default:
                SetCameraPriorities();
                targetFOV = defaultFOV;
                // Reset Dutch angles
                targetLeftDutch = 0f;
                targetRightDutch = 0f;
                break;
        }
    }

    private void SetCameraPriorities(CinemachineVirtualCamera activeCam = null)
    {
        freeLookCam.Priority = (activeCam == null) ? 10 : 0;
        leftWallCam.Priority = (activeCam == leftWallCam) ? 10 : 0;
        rightWallCam.Priority = (activeCam == rightWallCam) ? 10 : 0;
    }

    public void SetFollowTarget(Transform newTarget)
    {
        if (newTarget == null) return;

        cameraFollowTarget = newTarget;
        freeLookCam.Follow = newTarget;
        leftWallCam.Follow = newTarget;
        rightWallCam.Follow = newTarget;
    }

    public void SetLookAtTarget(Transform newTarget)
    {
        lookAtTarget = newTarget;
        freeLookCam.LookAt = newTarget;

        // Only update wall cameras if they don't have dedicated look at targets
        if (leftWallLookAtTarget == null)
            leftWallCam.LookAt = newTarget;
        if (rightWallLookAtTarget == null)
            rightWallCam.LookAt = newTarget;
    }

    public void SetLeftWallLookAtTarget(Transform newTarget)
    {
        leftWallLookAtTarget = newTarget;
        leftWallCam.LookAt = newTarget != null ? newTarget : lookAtTarget;
    }

    public void SetRightWallLookAtTarget(Transform newTarget)
    {
        rightWallLookAtTarget = newTarget;
        rightWallCam.LookAt = newTarget != null ? newTarget : lookAtTarget;
    }

    // Public method to adjust sync speed at runtime
    public void SetHorizontalSyncSpeed(float speed)
    {
        horizontalSyncSpeed = speed;
    }

    // Public method to adjust tilt settings at runtime
    public void SetWallRunTiltAngle(float angle)
    {
        wallRunTiltAngle = angle;
    }

    public void SetTiltLerpSpeed(float speed)
    {
        tiltLerpSpeed = speed;
    }
}
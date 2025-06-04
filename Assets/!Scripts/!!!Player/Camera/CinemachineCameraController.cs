using UnityEngine;
using Cinemachine;

public class CinemachineCameraController : MonoBehaviour
{
    
    [Header("References")]
    public CinemachineVirtualCamera vCam;
    public Transform cameraFollowTarget;     // Pivot used for horizontal movement (usually player root or pivot)
    public Transform lookAtTarget;           // This is the target the camera aims at
    public Transform playerTransform;
    public Transform cameraPivot;

    [Header("Camera Offsets")]
    public Vector3 defaultOffset = new Vector3(0, 4f, -8f);
    public Vector3 slideOffset = new Vector3(0, 1.2f, -5.5f);
    public Vector3 wallRunLeftOffset = new Vector3(2.5f, 2.2f, -3.5f);
    public Vector3 wallRunRightOffset = new Vector3(-2.5f, 2.2f, -3.5f);

    [Header("FOV Settings")]
    public float defaultFOV = 60f;
    public float slideFOV = 75f;
    public float wallRunFOV = 70f;

    [Header("Lerp Speeds")]
    public float offsetLerpSpeed = 5f;
    public float fovLerpSpeed = 5f;
    public float verticalLookSpeed = 100f;

    [Header("Vertical Look Clamp")]
    public float verticalMinAngle = -25f;
    public float verticalMaxAngle = 45f;
    private float verticalAngle = 0f;

    private CinemachineTransposer transposer;
    private Vector3 targetOffset;
    private float targetFOV;
    private float verticalOffset = 0f;

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
        transposer = vCam.GetCinemachineComponent<CinemachineTransposer>();

        if (transposer == null)
        {
            Debug.LogError("Cinemachine Transposer not found on virtual camera.");
        }

        vCam.Follow = cameraFollowTarget;
        vCam.LookAt = lookAtTarget;

        targetOffset = defaultOffset;
        targetFOV = defaultFOV;
    }

    void Update()
    {
        HandleVerticalLook();

        // Lerp follow offset and FOV for smooth state transitions
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, targetOffset, Time.deltaTime * offsetLerpSpeed);
        vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, targetFOV, Time.deltaTime * fovLerpSpeed);
    }

    private void HandleVerticalLook()
    {
        float mouseY = Input.GetAxis("Mouse Y") * verticalLookSpeed * Time.deltaTime;

        verticalAngle -= mouseY;
        verticalAngle = Mathf.Clamp(verticalAngle, verticalMinAngle, verticalMaxAngle);

        if (cameraPivot != null)
        {
            cameraPivot.localEulerAngles = new Vector3(verticalAngle, 0f, 0f);
            Debug.Log($"Vertical Look Angle: {verticalAngle}");
        }
    }

    public void UpdateCameraState(PlayerState newState)
    {
        if (newState == currentState) return;
        currentState = newState;

        switch (newState)
        {
            case PlayerState.Sliding:
                targetOffset = slideOffset;
                targetFOV = slideFOV;
                break;

            case PlayerState.WallRunningLeft:
                targetOffset = wallRunLeftOffset;
                targetFOV = wallRunFOV;
                break;

            case PlayerState.WallRunningRight:
                targetOffset = wallRunRightOffset;
                targetFOV = wallRunFOV;
                break;

            case PlayerState.Default:
            default:
                targetOffset = defaultOffset;
                targetFOV = defaultFOV;
                break;
        }
    }

    public void SetFollowTarget(Transform newTarget)
    {
        if (newTarget != null)
            vCam.Follow = newTarget;
    }
}

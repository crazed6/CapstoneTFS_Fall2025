using UnityEngine;
using Cinemachine;

public class AimingCameraController : MonoBehaviour
{
    [Header("Follow Targets")]
    public Transform leftShoulderAnchor;              // Camera anchor for wallrunning on right wall -_-
    public Transform rightShoulderAnchor;             // Camera anchor for wallrunning on left wall -_-
    public Transform defaultShoulderAnchor;           // Default third-person shoulder anchor -_-

    [Header("References")]
    public Transform playerBody;                      // The player's body used for yaw rotation -_-
    public PlayerJavelinThrow javelinThrow;           // Reference to javelin throw script -_-

    [Header("Main Camera (Cinemachine Brain Host)")]
    [SerializeField] private Camera debugRayCamera;   // Used for raycasting (main camera with CinemachineBrain) -_-

    [Header("Rotation Settings")]
    public float mouseSensitivity = 300f;             // Sensitivity for camera rotation -_-

    [Header("Clamp Settings")]
    public float horizontalClamp = 45f;               // Max left/right clamp for wallrun yaw -_-
    public float verticalClamp = 80f;                 // Max up/down clamp for pitch -_-

    [Header("FOV Settings")]
    public float normalFOV = 60f;                     // Default camera field of view -_-
    public float aimingFOV = 40f;                     // Field of view while aiming -_-
    public float fovLerpSpeed = 10f;                  // Speed of FOV transition -_-

    private Transform currentAnchor;                  // Active shoulder anchor -_-
    private float verticalRotation = 0f;              // Current camera pitch -_-
    private float baseYaw = 0f;                       // Locked yaw at start of wallrun -_-
    private bool wallrunLocked = false;               // Whether yaw is clamped due to wallrun -_-
    private bool wallOnRight = false;                 // Is wallrun happening on the right wall -_-

    private CinemachineVirtualCamera vCam;            // This virtual camera component -_-
    private CinemachineComponentBase vCamBody;        // Not used but kept for expansion -_-

    void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>(); // Cache the vCam component -_-
        if (!vCam)
        {
            Debug.LogError("AimingCameraController: No CinemachineVirtualCamera found!"); // Warn if missing -_-
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;     // Lock the cursor for gameplay -_-
        Cursor.visible = false;                       // Hide the cursor -_-
        currentAnchor = defaultShoulderAnchor;        // Set initial camera anchor -_-

        vCam.Follow = null;                           // We control position manually -_-
        vCam.LookAt = null;                           // Rotation also handled via script -_-
    }

    void LateUpdate()
    {
        if (playerBody == null || javelinThrow == null) return; // Skip if references are missing -_-

        HandleWallrunState();                         // Decide which shoulder to use -_-
        HandleRotation();                             // Handle pitch/yaw input -_-
        SnapToAnchor();                               // Move camera to selected anchor -_-
        HandleFOV();                                  // Smooth FOV zoom based on aiming -_-
    }

    void HandleWallrunState()
    {
        var cc = CharacterController.instance;

        if (cc.IsWallRunning)
        {
            wallOnRight = cc.IsWallOnRight;

            if (wallOnRight && leftShoulderAnchor != null)
            {
                currentAnchor = leftShoulderAnchor;
                wallrunLocked = true;                 // Use left shoulder when on right wall -_-
            }
            else if (!wallOnRight && rightShoulderAnchor != null)
            {
                currentAnchor = rightShoulderAnchor;
                wallrunLocked = true;                 // Use right shoulder when on left wall -_-
            }
            else
            {
                currentAnchor = defaultShoulderAnchor;
                wallrunLocked = false;                // Fallback to default if needed -_-
            }

            if (!wallrunLocked) return;
            baseYaw = playerBody.eulerAngles.y;       // Lock yaw reference for clamp -_-
        }
        else
        {
            wallrunLocked = false;
            currentAnchor = defaultShoulderAnchor;    // Reset to normal shoulder when not wallrunning -_-
        }
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.unscaledDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.unscaledDeltaTime;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalClamp, verticalClamp); // Clamp pitch -_-

        if (javelinThrow.IsAiming())
        {
            float nextYaw = playerBody.eulerAngles.y + mouseX;

            if (wallrunLocked)
            {
                float deltaYaw = Mathf.DeltaAngle(baseYaw, nextYaw); // Difference from locked yaw -_-
                deltaYaw = wallOnRight
                    ? Mathf.Clamp(deltaYaw, -horizontalClamp, 0)     // Clamp left only
                    : Mathf.Clamp(deltaYaw, 0, horizontalClamp);     // Clamp right only

                nextYaw = baseYaw + deltaYaw;
            }

            playerBody.rotation = Quaternion.Euler(0f, nextYaw, 0f); // Apply clamped yaw to player body -_-
        }

        transform.rotation = Quaternion.Euler(verticalRotation, playerBody.eulerAngles.y, 0f); // Pitch + yaw to camera -_-
    }

    void SnapToAnchor()
    {
        if (currentAnchor == null) return;
        transform.position = currentAnchor.position;   // Snap camera to current shoulder position -_-
    }

    void HandleFOV()
    {
        if (vCam == null) return;

        float targetFOV = javelinThrow.IsAiming() ? aimingFOV : normalFOV;

        if (vCam.m_Lens.FieldOfView != targetFOV)
        {
            vCam.m_Lens.FieldOfView = Mathf.Lerp(
                vCam.m_Lens.FieldOfView,
                targetFOV,
                Time.unscaledDeltaTime * fovLerpSpeed
            ); // Smoothly change FOV -_-
        }
    }

    public Camera GetCamera()
    {
        return debugRayCamera; // Return the active camera with CinemachineBrain for raycasting -_-
    }
}


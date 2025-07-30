// Ritwik 
using UnityEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;

public class AimingCameraController : MonoBehaviour
{
    [Header("Follow Targets")]
    public Transform leftShoulderAnchor;       // Anchor when wall is on the right side -_-
    public Transform rightShoulderAnchor;      // Anchor when wall is on the left side -_-
    public Transform defaultShoulderAnchor;    // Default anchor when not wallrunning -_-

    [Header("References")]
    public Transform playerBody;               // Player body transform to rotate in non-wallrun aiming -_-
    public PlayerJavelinThrow javelinThrow;    // Reference to aiming state -_-

    [Header("Main Camera (Cinemachine Brain Host)")]
    [SerializeField] private Camera debugRayCamera; // Camera that has CinemachineBrain for blending and raycasts -_-

    [Header("Rotation Settings")]
    public float mouseSensitivity = 300f;      // Mouse sensitivity for aim camera -_-

    [Header("Clamp Settings")]
    public float horizontalClamp = 45f;        // Yaw clamp while wallrunning (relative to base yaw) -_-
    public float verticalClamp = 80f;          // Pitch clamp always -_-

    [Header("FOV Settings")]
    public float normalFOV = 60f;              // FOV when not aiming -_-
    public float aimingFOV = 40f;              // FOV when aiming -_-
    public float fovLerpSpeed = 10f;           // Lerp speed for FOV (uses unscaled time) -_-

    private Transform currentAnchor;           // Current shoulder anchor -_-
    private float cameraYaw = 0f;              // Independent camera yaw used only during wallrun+aim -_-
    private float cameraPitch = 0f;            // Independent camera pitch -_-
    private float baseYaw = 0f;                // Player yaw captured when wallrun begins (for clamp) -_-
    private bool wallrunLocked = false;        // If true, we clamp yaw while aiming -_-
    private bool wallOnRight = false;          // Wall side information -_-

    private CinemachineVirtualCamera vCam;     // Aiming virtual camera reference -_-

    void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>(); // Cache virtual camera -_-
        if (!vCam) Debug.LogError("AimingCameraController: No CinemachineVirtualCamera found!"); // Warn if missing -_-
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;  // Lock cursor -_-
        Cursor.visible = false;                    // Hide cursor -_-
        currentAnchor = defaultShoulderAnchor;     // Set default anchor -_-

        // We position & rotate this Transform directly for tight control -_-
        vCam.Follow = null;                        // Not using Cinemachine body for follow -_-
        vCam.LookAt = null;                        // Not using Cinemachine aim for look -_-

        // Start aligned with player to avoid a pop the first time -_-
        cameraYaw = playerBody.eulerAngles.y;      // Initialize yaw -_-
        cameraPitch = 0f;                          // Initialize pitch -_-
        transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f); // Apply initial rotation -_-
    }

    void LateUpdate()
    {
        if (playerBody == null || javelinThrow == null) return; // Guard missing refs -_-

        HandleWallrunState(); // Update anchor and lock state -_-
        HandleRotation();     // Apply either free-look (wallrun+aim) or normal rotation -_-
        SnapToAnchor();       // Place camera at shoulder anchor -_-
        HandleFOV();          // Lerp FOV in unscaled time -_-
    }

    void HandleWallrunState()
    {
        var cc = CharacterController.instance; // Access your player controller singleton -_-

        bool wasWallrunLocked = wallrunLocked; // Remember previous frame lock state -_-

        if (cc.IsWallRunning)
        {
            wallOnRight = cc.IsWallOnRight; // Cache wall side -_-

            // Choose shoulder opposite the wall so camera looks into open space -_-
            Transform desiredAnchor = defaultShoulderAnchor; // Fallback -_-
            bool canLock = false; // Whether we can lock/clamp yaw this frame -_-

            if (wallOnRight && leftShoulderAnchor != null)
            {
                desiredAnchor = leftShoulderAnchor; // Right wall => left shoulder -_-
                canLock = true;                     // We have a valid shoulder, allow clamp -_-
            }
            else if (!wallOnRight && rightShoulderAnchor != null)
            {
                desiredAnchor = rightShoulderAnchor; // Left wall => right shoulder -_-
                canLock = true;                      // Valid shoulder -_-
            }

            currentAnchor = desiredAnchor; // Apply anchor -_-

            // Transition detect: we just entered the "locked" state this frame -_-
            if (canLock && !wasWallrunLocked)
            {
                baseYaw = playerBody.eulerAngles.y; // Capture base yaw once on enter -_-
                cameraYaw = baseYaw;                // Start free‑look from base yaw -_-
            }

            wallrunLocked = canLock; // Commit lock state -_-
        }
        else
        {
            // Leaving wallrun: if we were locked last frame, gently realign camera yaw to player to avoid a pop -_-
            if (wasWallrunLocked)
            {
                cameraYaw = playerBody.eulerAngles.y; // Sync camera yaw back to player yaw -_-
            }

            wallrunLocked = false;                 // Unlock when not wallrunning -_-
            currentAnchor = defaultShoulderAnchor; // Back to default anchor -_-
        }
    }

    void HandleRotation()
    {
        // Mouse input in unscaled time so slow‑mo doesn’t affect responsiveness -_-
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.unscaledDeltaTime; // Horizontal mouse -_-
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.unscaledDeltaTime; // Vertical mouse -_-

        // Always clamp pitch from camera perspective -_-
        cameraPitch = Mathf.Clamp(cameraPitch - mouseY, -verticalClamp, verticalClamp); // Update pitch -_-

        if (javelinThrow.IsAiming())
        {
            if (wallrunLocked)
            {
                // --- AIMING + WALLRUNNING => FREE‑LOOK: rotate only the camera, clamp around baseYaw -_-
                float nextYaw = cameraYaw + mouseX;                        // Proposed camera yaw -_-
                float deltaFromBase = Mathf.DeltaAngle(baseYaw, nextYaw);  // Offset from base yaw -_-
                deltaFromBase = wallOnRight
                    ? Mathf.Clamp(deltaFromBase, -horizontalClamp, 0f)     // Right wall: allow look left only -_-
                    : Mathf.Clamp(deltaFromBase, 0f, horizontalClamp);     // Left wall: allow look right only -_-
                cameraYaw = baseYaw + deltaFromBase;                        // Commit clamped yaw -_-

                // Apply to camera only — DO NOT rotate the player -_-
                transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f); // Camera free‑look -_-
            }
            else
            {
                // --- AIMING but NOT wallrunning => rotate player normally and align camera to player -_-
                float nextPlayerYaw = playerBody.eulerAngles.y + mouseX;   // Player yaw change -_-
                playerBody.rotation = Quaternion.Euler(0f, nextPlayerYaw, 0f); // Apply to player -_-

                cameraYaw = playerBody.eulerAngles.y;                      // Keep camera yaw synced to player -_-
                transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f); // Apply camera rot -_-
            }
        }
        else
        {
            // --- NOT AIMING => camera follows player yaw (standard third‑person feel) -_-
            cameraYaw = playerBody.eulerAngles.y;                          // Match player yaw -_-
            transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f); // Apply rotation -_-
        }
    }

    void SnapToAnchor()
    {
        if (currentAnchor == null) return;           // Guard -_-
        transform.position = currentAnchor.position; // Place camera at shoulder -_-
    }

    void HandleFOV()
    {
        if (vCam == null) return; // Guard -_-
        float targetFOV = javelinThrow.IsAiming() ? aimingFOV : normalFOV; // Choose target FOV -_-
        vCam.m_Lens.FieldOfView = Mathf.Lerp(vCam.m_Lens.FieldOfView, targetFOV, Time.unscaledDeltaTime * fovLerpSpeed); // Lerp FOV -_-
    }

    public Camera GetCamera()
    {
        return debugRayCamera; // Use this for raycasts (has CinemachineBrain) -_-
    }

    public void AlignCameraBehindPlayer()
    {
        // Optional helper to quickly align camera to player before switching, if you want -_-
        cameraPitch = 0f;                                     // Reset pitch -_-
        cameraYaw = playerBody.eulerAngles.y;                 // Match player yaw -_-
        transform.rotation = Quaternion.Euler(0f, cameraYaw, 0f); // Apply rotation -_-
    }
}
// Ritwik 
using UnityEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;
using System;

public class AimingCameraController : MonoBehaviour
{
    [Header("Follow Targets")]
    public Transform leftShoulderAnchor;   // Shoulder pivot when wall is on the RIGHT (camera sits on LEFT) -_-
    public Transform rightShoulderAnchor;  // Shoulder pivot when wall is on the LEFT  (camera sits on RIGHT) -_-
    public Transform defaultShoulderAnchor;// Shoulder pivot when not wallrunning -_-

    [Header("References")]
    public Transform playerBody;           // Player body (we rotate this when aiming off-wall) -_-
    public PlayerJavelinThrow javelinThrow;// To query "are we aiming?" -_-

    [Header("Main Camera (Cinemachine Brain Host)")]
    [SerializeField] private Camera debugRayCamera; // The Camera that has a CinemachineBrain (also used for hit raycasts) -_-

    [Header("Rotation Settings")]
    public float mouseSensitivity = 300f;  // Per-second sensitivity; we multiply by unscaledDeltaTime -_-

    [Header("Clamp Settings")]
    public float horizontalClamp = 45f;    // Horizontal clamp while wallrunning (relative to baseYaw) -_-
    public float verticalClamp = 80f;      // Vertical pitch clamp at all times -_-

    [Header("FOV Settings")]
    public float normalFOV = 60f;          // Default FOV when not aiming -_-
    public float aimingFOV = 40f;          // Narrow FOV when aiming -_-
    public float fovLerpSpeed = 10f;       // FOV lerp speed (uses unscaled time) -_-

    // --- Runtime state (camera space) -_-
    private Transform currentAnchor;       // Which shoulder we’re currently snapped to -_-
    private float cameraYaw = 0f;          // Independent camera yaw (used during wallrun aim) -_-
    private float cameraPitch = 0f;        // Independent camera pitch -_-
    private float baseYaw = 0f;            // Player yaw captured at wallrun start (clamp around this) -_-
    private bool wallrunLocked = false;    // True: we clamp camera yaw; we don't rotate the player -_-
    private bool wallOnRight = false;      // True if current wall is on player’s right -_-

    // --- Cinemachine refs & saved brain state -_-
    private CinemachineVirtualCamera vCam; // This Aiming vCam (we drive its transform manually) -_-
    private CinemachineBrain brain;        // The Brain on debugRayCamera -_-

    // Saved brain blend state so we can do an UN-SCALED default blend during wall-aim and restore after -_-
    private CinemachineBlendDefinition savedDefaultBlend;
    private bool savedValid = false;
    private bool savedIgnoreTimeScale = false;

    void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        if (!vCam) Debug.LogError("AimingCameraController: No CinemachineVirtualCamera found!");

        // Keep the vCam “hot” even when inactive so the first activation doesn’t start from a stale pose -_-
        vCam.m_StandbyUpdate = CinemachineVirtualCameraBase.StandbyUpdateMode.Always;

        // Cache the CinemachineBrain from the camera we use for rendering & raycasts -_-
        if (debugRayCamera) brain = debugRayCamera.GetComponent<CinemachineBrain>();
    }

    void Start()
    {
        // Standard input capture setup -_-
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Default shoulder until wallrun says otherwise -_-
        currentAnchor = defaultShoulderAnchor;

        // We control the vCam transform manually; ensure Body/Aim are not driving it via Follow/LookAt -_-
        vCam.Follow = null;
        vCam.LookAt = null;

        // Initialize camera orientation to player so there is no pop on first frame -_-
        cameraYaw = playerBody.eulerAngles.y;
        cameraPitch = 0f;
        transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
    }

    void LateUpdate()
    {
        if (!playerBody || !javelinThrow) return; // Safety guards -_-

        HandleWallrunState(); // Choose shoulder & set lock/clamp state -_-
        HandleRotation();     // Apply yaw/pitch either to camera (wallrun aim) or player (grounded aim) -_-
        SnapToAnchor();       // Keep camera position welded to chosen shoulder -_-
        HandleFOV();          // Lerp vCam FOV using unscaled time -_-
    }

    // --- Decide anchor & clamp mode based on wallrun state -_-
    void HandleWallrunState()
    {
        var cc = CharacterController.instance;
        bool wasWallrunLocked = wallrunLocked;

        if (cc.IsWallRunning)
        {
            wallOnRight = cc.IsWallOnRight;

            // Pick the shoulder opposite the wall so camera looks out into open space -_-
            Transform desiredAnchor = defaultShoulderAnchor;
            bool canLock = false;

            if (wallOnRight && leftShoulderAnchor) { desiredAnchor = leftShoulderAnchor; canLock = true; }
            else if (!wallOnRight && rightShoulderAnchor) { desiredAnchor = rightShoulderAnchor; canLock = true; }

            currentAnchor = desiredAnchor;

            // Just entered the lock state: capture baseYaw so our clamp is stable -_-
            if (canLock && !wasWallrunLocked)
            {
                baseYaw = playerBody.eulerAngles.y;
                cameraYaw = baseYaw; // start camera yaw from player yaw -_-
            }

            wallrunLocked = canLock;
        }
        else
        {
            // Leaving wallrun: snap camera yaw back to player yaw to avoid a pop -_-
            if (wasWallrunLocked) cameraYaw = playerBody.eulerAngles.y;

            wallrunLocked = false;
            currentAnchor = defaultShoulderAnchor;
        }
    }

    // --- Read mouse & apply rotation. Uses UN-SCALED delta so slow-mo doesn’t affect sensitivity -_-
    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.unscaledDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.unscaledDeltaTime;

        // Pitch: clamp straight from camera perspective -_-
        cameraPitch = Mathf.Clamp(cameraPitch - mouseY, -verticalClamp, verticalClamp);

        if (javelinThrow.IsAiming())
        {
            if (wallrunLocked)
            {
                // Wallrun + Aiming => camera-only free look clamped around baseYaw -_-
                float nextYaw = cameraYaw + mouseX;
                float deltaFromBase = Mathf.DeltaAngle(baseYaw, nextYaw);

                // If wall on RIGHT: allow left look (negative), else allow right look (positive) -_-
                deltaFromBase = wallOnRight
                    ? Mathf.Clamp(deltaFromBase, -horizontalClamp, 0f)
                    : Mathf.Clamp(deltaFromBase, 0f, horizontalClamp);

                cameraYaw = baseYaw + deltaFromBase;
                transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
            }
            else
            {
                // Grounded (or not locked): rotate the PLAYER yaw and keep camera aligned -_-
                float nextPlayerYaw = playerBody.eulerAngles.y + mouseX;
                playerBody.rotation = Quaternion.Euler(0f, nextPlayerYaw, 0f);

                cameraYaw = playerBody.eulerAngles.y;
                transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
            }
        }
        else
        {
            // Not aiming: treat camera as following the player's yaw -_-
            cameraYaw = playerBody.eulerAngles.y;
            transform.rotation = Quaternion.Euler(cameraPitch, cameraYaw, 0f);
        }
    }

    // --- Keep camera position glued to current shoulder anchor -_-
    void SnapToAnchor()
    {
        if (!currentAnchor) return;
        transform.position = currentAnchor.position;
    }

    // --- FOV animation: UN-SCALED so slow-mo doesn’t slow the zoom -_-
    void HandleFOV()
    {
        if (!vCam) return;
        float targetFOV = javelinThrow.IsAiming() ? aimingFOV : normalFOV;
        vCam.m_Lens.FieldOfView =
            Mathf.Lerp(vCam.m_Lens.FieldOfView, targetFOV, Time.unscaledDeltaTime * fovLerpSpeed);
    }

    public Camera GetCamera() => debugRayCamera; // Provide the camera that hosts CinemachineBrain for raycasts -_-

    public void AlignCameraBehindPlayer()
    {
        // Utility to zero pitch and align yaw with the player -_-
        cameraPitch = 0f;
        cameraYaw = playerBody.eulerAngles.y;
        transform.rotation = Quaternion.Euler(0f, cameraYaw, 0f);
    }

    // ================= UN-SCALED DEFAULT BLEND (reliable for any vCam pair) =================

    /// <summary>
    /// During the next activation (e.g., main -> aim while wallrunning), force the Brain to use an
    /// unscaled Default Blend (so slow-mo cannot stretch it). Automatically restores after 'duration' seconds (unscaled). -_-
    /// </summary>
    public void BeginUnscaledDefaultBlend(float duration = 0.15f,
                                          CinemachineBlendDefinition.Style style = CinemachineBlendDefinition.Style.EaseInOut)
    {
        BeginUnscaledDefaultBlendAsync(duration, style).Forget(); // Fire-and-forget; uses GetCancellationTokenOnDestroy -_-
    }

    private async UniTaskVoid BeginUnscaledDefaultBlendAsync(float duration,
                                                             CinemachineBlendDefinition.Style style)
    {
        if (!brain) return;

        // Save current Brain state once, so we can restore after the short window -_-
        if (!savedValid)
        {
            savedDefaultBlend = brain.m_DefaultBlend;
            savedIgnoreTimeScale = brain.m_IgnoreTimeScale;
            savedValid = true;
        }

        // Critical: ignore Time.timeScale during the blend so slow-mo doesn’t slow the transition -_-
        brain.m_IgnoreTimeScale = true;

        // Apply our unscaled default blend (affects any from->to pair, so it’s robust with FreeLook children) -_-
        brain.m_DefaultBlend = new CinemachineBlendDefinition(style, Mathf.Max(0f, duration));

        // Wait in UN-SCALED time for the exact duration, then restore defaults -_-
        var token = this.GetCancellationTokenOnDestroy();
        await UniTask.Delay(TimeSpan.FromSeconds(Mathf.Max(0f, duration)), ignoreTimeScale: true, cancellationToken: token);

        RestoreBlendDefaults();
    }

    /// <summary>
    /// Restore CinemachineBrain default blend & timescale behavior immediately (also auto-called after duration). -_-
    /// </summary>
    public void RestoreBlendDefaults()
    {
        if (!brain || !savedValid) return;

        brain.m_DefaultBlend = savedDefaultBlend;
        brain.m_IgnoreTimeScale = savedIgnoreTimeScale;
        savedValid = false;
    }
}

//Ritwik
using Cinemachine;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

public class PlayerJavelinThrow : MonoBehaviour
{
    [Header("Javelin Settings")]
    public GameObject javelinPrefab;        // Prefab of the javelin projectile -_-
    public Transform javelinSpawnPoint;     // (Unused here; kept for reference) -_-
    public float throwForce = 50f;          // (Unused here; you drive movement in JavelinController) -_-
    public float cooldownTime = 2f;         // Delay before you can aim/throw again -_-

    [Header("Throw Direction")]
    public bool useCameraDirection = false; // If you decide to force raw camera forward instead of raycast hit -_-

    [Header("Throw Points")]
    public Transform rightHandThrowPoint;   // Spawn parent when wall is on LEFT -> throw from RIGHT hand -_-
    public Transform leftHandThrowPoint;    // Spawn parent when wall is on RIGHT -> throw from LEFT hand -_-
    public Transform defaultThrowPoint;     // Spawn parent when not wallrunning -_-

    [Header("UI Settings")]
    public GameObject crosshair;            // Crosshair UI toggled during aiming -_-

    [Header("Slow Motion Settings")]
    public float slowMotionTimeScale = 0.1f; // Target time scale when slow-mo is fully engaged -_-
    public float slowMotionDuration = 1f;    // How long slow-mo stays active before ramping back -_-
    public float timeSlowInSpeed = 5f;       // Lerp speed into slow-mo (unscaled) -_-
    public float timeResetSpeed = 5f;        // Lerp speed back to 1x (unscaled) -_-

    [Header("Camera Reference")]
    public AimingCameraController aimingCameraController; // For camera utilities and unscaled blend -_-

    [Header("Camera Switching")]
    public CinemachineFreeLook mainVirtualCam;           // Main gameplay camera (FreeLook) -_-
    public CinemachineVirtualCamera aimingVirtualCam;    // Aiming camera (we drive its transform in AimingCameraController) -_-

    // --- Runtime state -_-
    private GameObject currentJavelin;      // The in-hand javelin while aiming -_-
    private bool isAiming = false;          // Aiming flag -_-
    private float cooldownTimer = 0f;       // Cooldown timer (counts down in UN-SCALED time) -_-
    private bool isEnteringSlowMotion = false; // Flag while lerping into slow-mo -_-
    private bool isSlowMotionActive = false;   // True when slow-mo is fully engaged -_-
    private float slowMotionTimer = 0f;        // Counts down the "hold" in slow-mo (unscaled) -_-
    private Transform currentThrowPoint;        // The transform we parent the spawned javelin to -_-
    private Camera playerCamera;                // Cache for raycasting -_-
    private PlayerInputActions input;           // Input actions (new Input System) -_-

    private bool isHolding = false; // Anims for jav throw - Colton
    public bool IsHolding => isHolding; // Anims for jav throw - Colton

    void Awake()
    {
        input = new PlayerInputActions(); // Create input actions -_-
    }

    void OnEnable()
    {
        // Enable and hook input callbacks -_-
        input.Player.Enable();
        input.Player.Aim.started += OnAimStarted;
        input.Player.Throw.canceled += OnThrowReleased;
    }

    void OnDisable()
    {
        // Disable and unhook input -_-
        input.Player.Disable();
        input.Player.Aim.started -= OnAimStarted;
        input.Player.Throw.canceled -= OnThrowReleased;
    }

    void Start()
    {
        playerCamera = aimingCameraController.GetCamera(); // Use the camera that hosts CinemachineBrain -_-
        if (crosshair) crosshair.SetActive(false);         // Hide until aiming -_-
    }

    void Update()
    {
        HandleCooldown();   // Count down aim cooldown (UN-SCALED) -_-
        HandleSlowMotion(); // Enter/hold/exit slow-mo (all UN-SCALED timings) -_-
    }

    // --- RMB pressed (Aim) -_-
    void OnAimStarted(InputAction.CallbackContext context)
    {
        if (cooldownTimer <= 0)
        {
            StartAiming().Forget(); // Fire-and-forget async flow -_-
        }
    }

    // --- Throw button released (end of aim + launch) -_-
    void OnThrowReleased(InputAction.CallbackContext context)
    {
        if (isAiming)
        {
            isHolding = false; // Anims for jav throw - Colton
            ThrowJavelin();
            ResetTimeScaleInstantly(); // If slow-mo is mid-ramp, snap back to normal -_-
        }
    }

    // --- Begin the aiming flow: spawn/parent javelin, switch vCam, stage slow-mo eligibility -_-
    async UniTaskVoid StartAiming()
    {
        if (crosshair) crosshair.SetActive(true);

        if (javelinPrefab)
        {
            UpdateCurrentThrowPoint(); // Pick hand based on wall side (or default) -_-

            // Create the "in-hand" javelin and parent it -_-
            currentJavelin = Instantiate(javelinPrefab, currentThrowPoint.position, currentThrowPoint.rotation);
            currentJavelin.transform.SetParent(currentThrowPoint);
            isAiming = true;
            isHolding = true; // Anims for jav throw - Colton

            // Disable javelin colliders while aiming so it doesn't collide with the player -_-
            currentJavelin.GetComponent<JavelinController>()?.SetAimingMode(true);

            // Switch to aiming vCam (with special UN-SCALED blend if we are wallrunning) -_-
            SwitchToAimingCamera();

            // Small UN-SCALED staging delay before enabling slow-mo (feel polish) -_-
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.3f), ignoreTimeScale: true);

            // If we meet the movement criteria, start lerping into slow-mo -_-
            if (IsEligibleForSlowMotion())
            {
                isEnteringSlowMotion = true;

                // Keep Physics stepping consistent with the target time scale we’re heading into -_-
                Time.timeScale = 1f; // start from 1 so our unscaled lerp is deterministic -_-
                Time.fixedDeltaTime = 0.02f * slowMotionTimeScale;
            }
        }
    }

    // --- Complete the throw, launch the projectile, and return to main camera -_-
    void ThrowJavelin()
    {
        if (currentJavelin)
        {
            currentJavelin.transform.SetParent(null); // Detach from hand -_-

            // Compute aim direction from center-screen ray (depth-agnostic) -_-
            Camera cam = aimingCameraController.GetCamera();
            Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));

            Vector3 finalDirection;

            // Prefer a hit point for exact target; fall back to forward direction -_-
            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, ~0))
            {
                Vector3 targetPoint = hit.point;
                finalDirection = (targetPoint - currentJavelin.transform.position).normalized;
            }
            else
            {
                finalDirection = ray.direction.normalized;
            }

            // Hand off to the projectile controller -_-
            var javelin = currentJavelin.GetComponent<JavelinController>();
            if (javelin != null)
            {
                Vector3 refForward = aimingCameraController.GetCamera().transform.forward;

                // Hard guard: if ray is actually behind, start from forward -_-
                if (Vector3.Dot(finalDirection, refForward) <= 0f)
                    finalDirection = refForward;

                // Clamp to forward hemisphere (≤ 89°) -_-
                // Set flattenY=false to keep it unbiased in 3D (prevents the "drifts right" issue) -_-
                finalDirection = ClampDirectionWithinCone(finalDirection, refForward, 89f, flattenY: false);

                javelin.SetAimingMode(false); // Re-enable colliders & layer -_-
                javelin.transform.rotation = Quaternion.LookRotation(finalDirection);
                javelin.SetDirection(finalDirection); // Begin flight -_-
            }

            currentJavelin = null;
            isAiming = false;

            // Return to main vCam -_-
            SwitchToMainCamera();

            // If we temporarily overrode CinemachineBrain’s default blend, restore it now -_-
            if (aimingCameraController != null)
                aimingCameraController.RestoreBlendDefaults();

            // UI & cooldown -_-
            if (crosshair) crosshair.SetActive(false);
            cooldownTimer = cooldownTime;
        }
    }

    // --- Emergency: reset time immediately (e.g., if you cancel slow-mo mid-flow) -_-
    void ResetTimeScaleInstantly()
    {
        if (isSlowMotionActive || isEnteringSlowMotion)
        {
            isEnteringSlowMotion = false;
            isSlowMotionActive = false;
            slowMotionTimer = 0f;
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
    }

    Vector3 GetThrowDirection()
    {
        // Debug helper: visualize center ray; returns forward direction -_-
        Camera cam = aimingCameraController.GetCamera();
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);
        return ray.direction.normalized;
    }

    // --- Count cooldown in UN-SCALED time so slow-mo doesn’t slow your ability to re-aim -_-
    void HandleCooldown()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.unscaledDeltaTime;
    }

    // --- All slow-mo transitions are driven by UN-SCALED time to keep feel consistent -_-
    void HandleSlowMotion()
    {
        // Phase 1: Enter slow-mo (lerp 1f -> target) -_-
        if (isEnteringSlowMotion)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, slowMotionTimeScale,
                                        timeSlowInSpeed * Time.unscaledDeltaTime);

            // Close enough: snap to exact target, set fixedDeltaTime for physics, start hold timer -_-
            if (Mathf.Abs(Time.timeScale - slowMotionTimeScale) < 0.005f)
            {
                Time.timeScale = slowMotionTimeScale;
                Time.fixedDeltaTime = 0.02f * slowMotionTimeScale;
                isEnteringSlowMotion = false;
                isSlowMotionActive = true;
                slowMotionTimer = slowMotionDuration; // unscaled countdown -_-
            }
        }

        // Phase 2: Hold slow-mo, then exit (lerp target -> 1f) -_-
        if (isSlowMotionActive && !isEnteringSlowMotion)
        {
            slowMotionTimer -= Time.unscaledDeltaTime;

            if (slowMotionTimer <= 0f)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, 1f,
                                            timeResetSpeed * Time.unscaledDeltaTime);

                if (Mathf.Abs(Time.timeScale - 1f) < 0.005f)
                {
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.02f;
                    isSlowMotionActive = false;
                }
            }
        }
    }

    public bool IsAiming() => isAiming; // External query for camera logic -_-

    // --- Movement gating: only allow slow-mo in air or on wall; not while dashing/sliding -_-
    bool IsEligibleForSlowMotion()
    {
        var cc = CharacterController.instance;
        return (!cc.IsGrounded || cc.IsWallRunning)
               && !cc.IsDashing
               && !cc.IsSliding
               && currentJavelin != null;
    }

    // --- Switch to Aiming vCam; while wallrunning, make the blend UN-SCALED so slow-mo can’t stretch it -_-
    void SwitchToAimingCamera()
    {
        var cc = CharacterController.instance;
        if (cc != null && cc.IsWallRunning && aimingCameraController != null)
        {
            // You chose 0.5s EaseInOut here: longer, more cinematic transition that still ignores slow-mo -_-
            // Tweak to taste (e.g., 0.12–0.20 for snappier, 0.5 for dramatic) -_-
            aimingCameraController.BeginUnscaledDefaultBlend(duration: 0.5f,
                style: CinemachineBlendDefinition.Style.EaseInOut);
        }

        // Priority swap drives the Brain to show the aiming vCam -_-
        mainVirtualCam.Priority = 10;
        aimingVirtualCam.Priority = 20;
        playerCamera = aimingCameraController.GetCamera();
    }

    // --- Switch back to Main vCam -_-
    void SwitchToMainCamera()
    {
        aimingVirtualCam.Priority = 5;
        mainVirtualCam.Priority = 20;
    }

    // --- Pick which hand to parent the javelin to, based on wall side (or default when grounded) -_-
    void UpdateCurrentThrowPoint()
    {
        var cc = CharacterController.instance;

        if (cc.IsWallRunning)
        {
            currentThrowPoint = cc.IsWallOnRight ? leftHandThrowPoint : rightHandThrowPoint;
        }
        else
        {
            currentThrowPoint = defaultThrowPoint != null ? defaultThrowPoint : rightHandThrowPoint;
        }
    }

    // Clamp 'dir' to be within 'maxAngleDeg' of 'referenceForward' using RotateTowards -_-
    // Set flattenY=true to only enforce the clamp horizontally (keeps vertical freedom) -_-
    static Vector3 ClampDirectionWithinCone(Vector3 dir, Vector3 referenceForward, float maxAngleDeg, bool flattenY)
    {
        if (flattenY)
        {
            // Work in horizontal (planar) space to avoid messing with vertical aim
            Vector3 refPlanar = referenceForward; refPlanar.y = 0f;
            Vector3 dirPlanar = dir; dirPlanar.y = 0f;

            if (refPlanar.sqrMagnitude < 1e-6f) refPlanar = Vector3.forward;
            if (dirPlanar.sqrMagnitude < 1e-6f) dirPlanar = refPlanar;

            Vector3 clampedPlanar = Vector3.RotateTowards(
                refPlanar.normalized,
                dirPlanar.normalized,
                maxAngleDeg * Mathf.Deg2Rad,
                float.PositiveInfinity
            );

            // Reapply original vertical component, then renormalize -_-
            Vector3 result = new Vector3(clampedPlanar.x, dir.y, clampedPlanar.z).normalized;
            return result;
        }
        else
        {
            if (referenceForward.sqrMagnitude < 1e-6f) referenceForward = Vector3.forward;
            if (dir.sqrMagnitude < 1e-6f) dir = referenceForward;

            Vector3 clamped3D = Vector3.RotateTowards(
                referenceForward.normalized,
                dir.normalized,
                maxAngleDeg * Mathf.Deg2Rad,
                float.PositiveInfinity
            );
            return clamped3D.normalized;
        }
    }

}

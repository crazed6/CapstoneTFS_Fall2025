//Ritwik
using Cinemachine;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

public class PlayerJavelinThrow : MonoBehaviour
{
    [Header("Javelin Settings")]
    public GameObject javelinPrefab; // Prefab for the javelin -_-
    public Transform javelinSpawnPoint; // Unused in current logic -_-
    public float throwForce = 50f; // Stored for future use if needed -_-
    public float cooldownTime = 2f; // Cooldown time between javelin throws -_-

    [Header("Throw Direction")]
    public bool useCameraDirection = false; // Option to use camera for direction (unused) -_-

    [Header("Throw Points")]
    public Transform rightHandThrowPoint; // Throw point when wallrunning on left wall -_-
    public Transform leftHandThrowPoint; // Throw point when wallrunning on right wall -_-
    public Transform defaultThrowPoint; // Default throw point if not wallrunning -_-

    [Header("UI Settings")]
    public GameObject crosshair; // Crosshair UI shown during aiming -_-

    [Header("Slow Motion Settings")]
    public float slowMotionTimeScale = 0.1f; // Target time scale for slow motion -_-
    public float slowMotionDuration = 1f; // How long slow motion should last -_-
    public float timeSlowInSpeed = 5f; // Lerp speed into slow motion -_-
    public float timeResetSpeed = 5f; // Lerp speed to return to normal time -_-

    [Header("Camera Reference")]
    public AimingCameraController aimingCameraController; // Reference to camera anchor system -_-

    [Header("Camera Switching")]
    public CinemachineFreeLook mainVirtualCam; // Main free look camera -_-
    public CinemachineVirtualCamera aimingVirtualCam; // Aiming camera -_-

    private GameObject currentJavelin; // The currently held javelin -_-
    private bool isAiming = false; // Whether the player is in aiming mode -_-
    private float cooldownTimer = 0f; // Countdown for next throw -_-
    private bool isEnteringSlowMotion = false; // If currently lerping into slow motion -_-
    private bool isSlowMotionActive = false; // If slow motion is active -_-
    private float slowMotionTimer = 0f; // Countdown timer for slow motion -_-
    private Transform currentThrowPoint; // Selected throw point based on wall state -_-
    private Camera playerCamera; // Cached camera reference -_-

    private PlayerInputActions input; // Input system instance -_-

    void Awake()
    {
        input = new PlayerInputActions(); // Create input asset instance -_-
    }

    void OnEnable()
    {
        input.Player.Enable(); // Enable Player action map -_-
        input.Player.Aim.started += OnAimStarted; // Register event for aim press -_-
        input.Player.Throw.canceled += OnThrowReleased; // Register event for throw release -_-
    }

    void OnDisable()
    {
        input.Player.Disable(); // Disable Player input map -_-
        input.Player.Aim.started -= OnAimStarted; // Unregister event -_-
        input.Player.Throw.canceled -= OnThrowReleased; // Unregister event -_-
    }

    void Start()
    {
        playerCamera = aimingCameraController.GetCamera(); // Cache camera reference -_-
        if (crosshair) crosshair.SetActive(false); // Hide crosshair initially -_-
    }

    void Update()
    {
        HandleCooldown(); // Handle throw cooldown -_-
        HandleSlowMotion(); // Handle slow motion lerping -_-
    }

    void OnAimStarted(InputAction.CallbackContext context)
    {
        if (cooldownTimer <= 0)
        {
            StartAiming().Forget(); // Begin aiming if not on cooldown -_-
        }
    }

    void OnThrowReleased(InputAction.CallbackContext context)
    {
        if (isAiming)
        {
            ThrowJavelin(); // Triggered on right-click release -_-
        }
    }

    async UniTaskVoid StartAiming()
    {
        if (crosshair) crosshair.SetActive(true); // Show crosshair -_-

        if (javelinPrefab && javelinSpawnPoint)
        {
            UpdateCurrentThrowPoint(); // Determine which hand to use -_-

            currentJavelin = Instantiate(javelinPrefab, currentThrowPoint.position, currentThrowPoint.rotation); // Spawn javelin -_-
            currentJavelin.transform.SetParent(currentThrowPoint); // Parent to hand -_-
            isAiming = true; // Set aiming state -_-

            SwitchToAimingCamera(); // Switch to aim cam -_-

            await UniTask.Delay(System.TimeSpan.FromSeconds(0.3f), ignoreTimeScale: true); // Wait for camera transition -_-

            if (IsEligibleForSlowMotion())
            {
                isEnteringSlowMotion = true;
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f * slowMotionTimeScale;
            }
        }
    }

    void ThrowJavelin()
    {
        if (currentJavelin)
        {
            currentJavelin.transform.SetParent(null); // Unparent the javelin -_-
            Vector3 throwDirection = GetThrowDirection(); // Get direction -_-
            currentJavelin.GetComponent<JavelinController>().SetDirection(throwDirection); // Throw it -_-

            currentJavelin = null;
            isAiming = false;

            SwitchToMainCamera(); // Switch back to main cam -_-
            if (crosshair) crosshair.SetActive(false); // Hide crosshair -_-

            cooldownTimer = cooldownTime; // Reset cooldown -_-

            if (isSlowMotionActive || isEnteringSlowMotion)
            {
                isEnteringSlowMotion = false;
                slowMotionTimer = 0f;
                isSlowMotionActive = true;
            }
        }
    }

    Vector3 GetThrowDirection()
    {
        Camera cam = aimingCameraController.GetCamera(); // Use aiming cam -_-
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f)); // Cast from screen center -_-
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f); // Debug ray -_-
        return ray.direction.normalized; // Return direction -_-
    }

    void HandleCooldown()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.unscaledDeltaTime; // Count down cooldown -_-
    }

    void HandleSlowMotion()
    {
        if (isEnteringSlowMotion)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, slowMotionTimeScale, timeSlowInSpeed * Time.unscaledDeltaTime); // Lerp into slow motion -_-

            if (Mathf.Abs(Time.timeScale - slowMotionTimeScale) < 0.005f)
            {
                Time.timeScale = slowMotionTimeScale;
                Time.fixedDeltaTime = 0.02f * slowMotionTimeScale;
                isEnteringSlowMotion = false;
                isSlowMotionActive = true;
                slowMotionTimer = slowMotionDuration;
            }
        }

        if (isSlowMotionActive && !isEnteringSlowMotion)
        {
            slowMotionTimer -= Time.unscaledDeltaTime; // Count down slow motion -_-

            if (slowMotionTimer <= 0f)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, timeResetSpeed * Time.unscaledDeltaTime); // Lerp back to normal -_-

                if (Mathf.Abs(Time.timeScale - 1f) < 0.005f)
                {
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.02f;
                    isSlowMotionActive = false;
                }
            }
        }
    }

    public bool IsAiming() => isAiming; // Returns if aiming is active -_-

    bool IsEligibleForSlowMotion()
    {
        var cc = CharacterController.instance;
        return (!cc.IsGrounded || cc.IsWallRunning) && !cc.IsDashing && !cc.IsSliding; // Conditions for slow motion -_-
    }

    void SwitchToAimingCamera()
    {
        mainVirtualCam.Priority = 10; // Lower priority of main cam -_-
        aimingVirtualCam.Priority = 20; // Raise priority of aiming cam -_-
        playerCamera = aimingCameraController.GetCamera(); // Refresh camera -_-
    }

    void SwitchToMainCamera()
    {
        aimingVirtualCam.Priority = 5; // Lower aim cam priority -_-
        mainVirtualCam.Priority = 20; // Restore main cam -_-
    }

    void UpdateCurrentThrowPoint()
    {
        var cc = CharacterController.instance;

        if (cc.IsWallRunning)
        {
            currentThrowPoint = cc.IsWallOnRight ? leftHandThrowPoint : rightHandThrowPoint; // Flip based on wall side -_-
        }
        else
        {
            currentThrowPoint = defaultThrowPoint != null ? defaultThrowPoint : rightHandThrowPoint; // Fallback to default -_-
        }
    }
}
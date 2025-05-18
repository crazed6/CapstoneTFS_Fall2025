using Cinemachine;
using UnityEngine;
using UnityEngine.UI;

public class PlayerJavelinThrow : MonoBehaviour
{
    [Header("Javelin Settings")]
    public GameObject javelinPrefab;                    // Prefab of the javelin to be instantiated -_-
    public Transform javelinSpawnPoint;                 // Point where javelin will be instantiated from -_-
    public float throwForce = 50f;                      // Not used currently, but kept for future physical throw logic -_-
    public float cooldownTime = 2f;                     // Time between javelin throws -_-

    [Header("Throw Direction")]
    public bool useCameraDirection = false;             // Toggle to determine throw direction source -_-

    [Header("Throw Points")]
    public Transform rightHandThrowPoint;               // Javelin spawn point when wallrunning on left wall -_-
    public Transform leftHandThrowPoint;                // Javelin spawn point when wallrunning on right wall -_-
    public Transform defaultThrowPoint;                 // Default spawn point when not wallrunning -_-

    [Header("UI Settings")]
    public GameObject crosshair;                        // Crosshair UI shown while aiming -_-

    [Header("Slow Motion Settings")]
    public float slowMotionTimeScale = 0.1f;            // Time scale during slow motion -_-
    public float slowMotionDuration = 1f;               // Duration of slow motion effect -_-
    public float timeSlowInSpeed = 5f;                  // Speed at which slow motion kicks in -_-
    public float timeResetSpeed = 5f;                   // Speed at which normal time resumes -_-

    [Header("Camera Reference")]
    public AimingCameraController aimingCameraController; // Reference to the aiming camera logic -_-

    [Header("Camera Switching")]
    public CinemachineVirtualCamera mainVirtualCam;     // Reference to main gameplay camera -_-
    public CinemachineVirtualCamera aimingVirtualCam;   // Reference to javelin aiming camera -_-

    private GameObject currentJavelin;                  // Currently held (unthrown) javelin -_-
    private bool isAiming = false;                      // If the player is currently aiming -_-
    private float cooldownTimer = 0f;                   // Internal cooldown timer -_-
    private bool isEnteringSlowMotion = false;          // If slow motion is being activated -_-
    private bool isSlowMotionActive = false;            // If slow motion is currently active -_-
    private float slowMotionTimer = 0f;                 // Timer for how long slow motion lasts -_-
    private Transform currentThrowPoint;                // Throw point selected based on wall side or default -_-

    private Camera playerCamera;                        // Active camera used for screen raycasting -_-

    void Start()
    {
        playerCamera = aimingCameraController.GetCamera(); // Assign main camera with Cinemachine brain for raycasts -_-

        if (crosshair) crosshair.SetActive(false);         // Disable crosshair at start -_-
    }

    void Update()
    {
        HandleCooldown();                                  // Update throw cooldown -_-

        if (Input.GetMouseButtonDown(1) && cooldownTimer <= 0)
        {
            StartAiming();                                 // Start aiming when right-click is pressed -_-
        }

        if (Input.GetMouseButtonUp(1) && isAiming)
        {
            ThrowJavelin();                                // Throw the javelin when right-click is released -_-
        }

        HandleSlowMotion();                                // Manage slow motion transitions -_-
    }

    void StartAiming()
    {
        if (crosshair) crosshair.SetActive(true);          // Enable crosshair UI -_-

        if (javelinPrefab && javelinSpawnPoint)
        {
            UpdateCurrentThrowPoint();                     // Determine correct hand to spawn from -_-

            currentJavelin = Instantiate(javelinPrefab, currentThrowPoint.position, currentThrowPoint.rotation); // Spawn javelin -_-
            currentJavelin.transform.SetParent(currentThrowPoint);                                               // Attach it to hand -_-

            isAiming = true;                               // Mark aiming as active -_-

            SwitchToAimingCamera();                        // Raise priority to activate aiming camera -_-

            if (IsEligibleForSlowMotion())                 // Check if conditions meet slow motion trigger -_-
            {
                isEnteringSlowMotion = true;               // Begin slow motion lerp -_-
                Time.timeScale = 1f;                       // Reset time scale to begin lerp properly -_-
                Time.fixedDeltaTime = 0.02f * slowMotionTimeScale; // Set physics step for slowed time -_-
            }
        }
    }

    void ThrowJavelin()
    {
        if (currentJavelin)
        {
            currentJavelin.transform.SetParent(null);      // Detach javelin from hand -_-

            Vector3 throwDirection = GetThrowDirection();  // Get direction from crosshair ray -_-
            currentJavelin.GetComponent<JavelinController>().SetDirection(throwDirection); // Initiate arc flight -_-

            currentJavelin = null;
            isAiming = false;

            SwitchToMainCamera();                          // Return to main gameplay cam -_-
            if (crosshair) crosshair.SetActive(false);     // Disable crosshair -_-
            cooldownTimer = cooldownTime;                  // Start throw cooldown -_-

            if (isSlowMotionActive || isEnteringSlowMotion)
            {
                isEnteringSlowMotion = false;
                slowMotionTimer = 0f;
                isSlowMotionActive = true;                 // Allow time reset to begin -_-
            }
        }
    }

    Vector3 GetThrowDirection()
    {
        Camera cam = aimingCameraController.GetCamera();   // Use active camera with brain -_-
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f)); // Cast ray from screen center -_-
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);                     // Debug the ray in scene -_-
        return ray.direction.normalized;                // Return direction vector for throw -_-
    }

    void HandleCooldown()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.unscaledDeltaTime;    // Countdown based on real time -_-
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
                slowMotionTimer = slowMotionDuration;   // Begin countdown of slow motion -_-
            }
        }

        if (isSlowMotionActive && !isEnteringSlowMotion)
        {
            slowMotionTimer -= Time.unscaledDeltaTime;  // Countdown slow motion timer -_-

            if (slowMotionTimer <= 0f)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, timeResetSpeed * Time.unscaledDeltaTime); // Lerp back to normal time -_-

                if (Mathf.Abs(Time.timeScale - 1f) < 0.005f)
                {
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.02f;
                    isSlowMotionActive = false;          // End slow motion completely -_-
                }
            }
        }
    }

    public bool IsAiming() => isAiming;                 // Getter for current aiming state -_-

    bool IsEligibleForSlowMotion()
    {
        var cc = CharacterController.instance;
        return (!cc.IsGrounded || cc.IsWallRunning) && !cc.IsDashing && !cc.IsSliding; // Check for air/wallrun, but not dash or slide -_-
    }

    void SwitchToAimingCamera()
    {
        mainVirtualCam.Priority = 10;                   // Lower main camera priority -_-
        aimingVirtualCam.Priority = 20;                 // Raise aiming cam priority to activate it -_-
        playerCamera = aimingCameraController.GetCamera(); // Refresh raycast cam reference -_-
    }

    void SwitchToMainCamera()
    {
        aimingVirtualCam.Priority = 5;                  // Lower aiming cam priority -_-
        mainVirtualCam.Priority = 20;                   // Raise main cam back to top -_-
    }

    void UpdateCurrentThrowPoint()
    {
        var cc = CharacterController.instance;

        if (cc.IsWallRunning)
        {
            currentThrowPoint = cc.IsWallOnRight ? leftHandThrowPoint : rightHandThrowPoint; // Flip throw side based on wall -_-
        }
        else
        {
            currentThrowPoint = defaultThrowPoint != null ? defaultThrowPoint : rightHandThrowPoint; // Use default if set, fallback to right -_-
        }
    }
}


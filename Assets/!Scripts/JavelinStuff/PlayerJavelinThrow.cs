using UnityEngine;
using UnityEngine.UI;

public class PlayerJavelinThrow : MonoBehaviour
{
    [Header("Javelin Settings")]
    public GameObject javelinPrefab;
    public Transform javelinSpawnPoint;
    public float throwForce = 50f;
    public float cooldownTime = 2f;

    [Header("Throw Direction")]
    public bool useCameraDirection = false;

    [Header("Throw Points")]
    public Transform rightHandThrowPoint;
    public Transform leftHandThrowPoint;
    public Transform defaultThrowPoint;

    [Header("UI Settings")]
    public GameObject crosshair;

    [Header("Slow Motion Settings")]
    public float slowMotionTimeScale = 0.1f;
    public float slowMotionDuration = 1f;
    public float timeSlowInSpeed = 5f;
    public float timeResetSpeed = 5f;

    [Header("Camera Reference")]
    public AimingCameraController aimingCameraController;

    [Header("Camera Switching")]
    public Camera mainCamera;
    public Camera aimingCamera;

    private GameObject currentJavelin;
    private bool isAiming = false;
    private float cooldownTimer = 0f;
    private bool isEnteringSlowMotion = false;
    private bool isSlowMotionActive = false;
    private float slowMotionTimer = 0f;
    private Transform currentThrowPoint;

    private Camera playerCamera;

    void Start()
    {
        playerCamera = aimingCameraController.GetCamera();

        // Initial camera setup
        if (mainCamera != null) mainCamera.gameObject.SetActive(true);
        if (aimingCamera != null) aimingCamera.gameObject.SetActive(false);

        if (crosshair) crosshair.SetActive(false);
    }

    void Update()
    {
        HandleCooldown();

        if (Input.GetMouseButtonDown(1) && cooldownTimer <= 0)
        {
            StartAiming();
        }

        if (Input.GetMouseButtonUp(1) && isAiming)
        {
            ThrowJavelin();
        }

        HandleSlowMotion();
    }

    void StartAiming()
    {
        if (crosshair) crosshair.SetActive(true);

        if (javelinPrefab && javelinSpawnPoint)
        {
            UpdateCurrentThrowPoint();

            currentJavelin = Instantiate(javelinPrefab, currentThrowPoint.position, currentThrowPoint.rotation);
            currentJavelin.transform.SetParent(currentThrowPoint);

            isAiming = true;

            SwitchToAimingCamera();

            if (IsEligibleForSlowMotion())
            {
                isEnteringSlowMotion = true;
                Time.timeScale = 1f; // Ensure normal start
                Time.fixedDeltaTime = 0.02f * slowMotionTimeScale;
            }
        }
    }

    void ThrowJavelin()
    {
        if (currentJavelin)
        {
            currentJavelin.transform.SetParent(null);

            Vector3 throwDirection = useCameraDirection
                ? GetThrowDirection()
                : CharacterController.instance.transform.forward;

            currentJavelin.GetComponent<JavelinController>().SetDirection(throwDirection);

            currentJavelin = null;
            isAiming = false;
            SwitchToMainCamera();

            if (crosshair) crosshair.SetActive(false);

            cooldownTimer = cooldownTime;

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
        Camera cam = aimingCameraController.GetCamera();
        if (cam == null)
        {
            Debug.LogWarning("Aiming Camera is not available — defaulting to forward.");
            return transform.forward;
        }

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return (hit.point - javelinSpawnPoint.position).normalized;
        }
        else
        {
            return ray.direction;
        }
    }

    void HandleCooldown()
    {
        if (cooldownTimer > 0)
            cooldownTimer -= Time.unscaledDeltaTime;
    }

    void HandleSlowMotion()
    {
        if (isEnteringSlowMotion)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, slowMotionTimeScale, timeSlowInSpeed * Time.unscaledDeltaTime);

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
            slowMotionTimer -= Time.unscaledDeltaTime;

            if (slowMotionTimer <= 0f)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, timeResetSpeed * Time.unscaledDeltaTime);

                if (Mathf.Abs(Time.timeScale - 1f) < 0.005f)
                {
                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.02f;
                    isSlowMotionActive = false;
                }
            }
        }
    }

    public bool IsAiming() => isAiming;

    bool IsEligibleForSlowMotion()
    {
        var cc = CharacterController.instance;
        return (!cc.IsGrounded || cc.IsWallRunning) && !cc.IsDashing && !cc.IsSliding;

    }

    void SwitchToAimingCamera()
    {
        if (mainCamera != null) mainCamera.gameObject.SetActive(false);
        if (aimingCamera != null)
        {
            aimingCamera.gameObject.SetActive(true);
            playerCamera = aimingCamera.GetComponent<Camera>();
        }
    }

    void SwitchToMainCamera()
    {
        if (aimingCamera != null) aimingCamera.gameObject.SetActive(false);
        if (mainCamera != null) mainCamera.gameObject.SetActive(true);
    }

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
}

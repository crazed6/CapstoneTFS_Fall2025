//Ritwik
using Cinemachine;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

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
    public CinemachineFreeLook mainVirtualCam;
    public CinemachineVirtualCamera aimingVirtualCam;

    private GameObject currentJavelin;
    private bool isAiming = false;
    private float cooldownTimer = 0f;
    private bool isEnteringSlowMotion = false;
    private bool isSlowMotionActive = false;
    private float slowMotionTimer = 0f;
    private Transform currentThrowPoint;
    private Camera playerCamera;
    private PlayerInputActions input;

    void Awake()
    {
        input = new PlayerInputActions();
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Player.Aim.started += OnAimStarted;
        input.Player.Throw.canceled += OnThrowReleased;
    }

    void OnDisable()
    {
        input.Player.Disable();
        input.Player.Aim.started -= OnAimStarted;
        input.Player.Throw.canceled -= OnThrowReleased;
    }

    void Start()
    {
        playerCamera = aimingCameraController.GetCamera();
        if (crosshair) crosshair.SetActive(false);
    }

    void Update()
    {
        HandleCooldown();
        HandleSlowMotion();
    }

    void OnAimStarted(InputAction.CallbackContext context)
    {
        if (cooldownTimer <= 0)
        {
            StartAiming().Forget();
        }
    }

    void OnThrowReleased(InputAction.CallbackContext context)
    {
        if (isAiming)
        {
            ThrowJavelin();
            ResetTimeScaleInstantly();
        }
    }

    async UniTaskVoid StartAiming()
    {
        if (crosshair) crosshair.SetActive(true);

        if (javelinPrefab)
        {
            UpdateCurrentThrowPoint();

            currentJavelin = Instantiate(javelinPrefab, currentThrowPoint.position, currentThrowPoint.rotation);
            currentJavelin.transform.SetParent(currentThrowPoint);
            isAiming = true;

            SwitchToAimingCamera();
            await UniTask.Delay(System.TimeSpan.FromSeconds(0.3f), ignoreTimeScale: true);

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
            currentJavelin.transform.SetParent(null);
            Vector3 throwDirection = GetThrowDirection();
            currentJavelin.GetComponent<JavelinController>().SetDirection(throwDirection);

            currentJavelin = null;
            isAiming = false;

            SwitchToMainCamera();
            if (crosshair) crosshair.SetActive(false);
            cooldownTimer = cooldownTime;
        }
    }

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
        Camera cam = aimingCameraController.GetCamera();
        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f));
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 2f);
        return ray.direction.normalized;
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
        mainVirtualCam.Priority = 10;
        aimingVirtualCam.Priority = 20;
        playerCamera = aimingCameraController.GetCamera();
    }

    void SwitchToMainCamera()
    {
        aimingVirtualCam.Priority = 5;
        mainVirtualCam.Priority = 20;
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

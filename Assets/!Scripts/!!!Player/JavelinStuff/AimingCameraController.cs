// Ritwik 
using UnityEngine;
using Cinemachine;
using Cysharp.Threading.Tasks;

public class AimingCameraController : MonoBehaviour
{
    [Header("Follow Targets")]
    public Transform leftShoulderAnchor;
    public Transform rightShoulderAnchor;
    public Transform defaultShoulderAnchor;

    [Header("References")]
    public Transform playerBody;
    public PlayerJavelinThrow javelinThrow;

    [Header("Main Camera (Cinemachine Brain Host)")]
    [SerializeField] private Camera debugRayCamera;

    [Header("Rotation Settings")]
    public float mouseSensitivity = 300f;

    [Header("Clamp Settings")]
    public float horizontalClamp = 45f;
    public float verticalClamp = 80f;

    [Header("FOV Settings")]
    public float normalFOV = 60f;
    public float aimingFOV = 40f;
    public float fovLerpSpeed = 10f;

    private Transform currentAnchor;
    private float verticalRotation = 0f;
    private float baseYaw = 0f;
    private bool wallrunLocked = false;
    private bool wallOnRight = false;

    private CinemachineVirtualCamera vCam;
    private CinemachineComponentBase vCamBody;

    void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
        if (!vCam)
        {
            Debug.LogError("AimingCameraController: No CinemachineVirtualCamera found!");
        }
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentAnchor = defaultShoulderAnchor;

        vCam.Follow = null;
        vCam.LookAt = null;
    }

    void LateUpdate()
    {
        if (playerBody == null || javelinThrow == null) return;

        HandleWallrunState();
        HandleRotation();
        SnapToAnchor();
        HandleFOV();
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
                wallrunLocked = true;
            }
            else if (!wallOnRight && rightShoulderAnchor != null)
            {
                currentAnchor = rightShoulderAnchor;
                wallrunLocked = true;
            }
            else
            {
                currentAnchor = defaultShoulderAnchor;
                wallrunLocked = false;
            }

            if (!wallrunLocked) return;
            baseYaw = playerBody.eulerAngles.y;
        }
        else
        {
            wallrunLocked = false;
            currentAnchor = defaultShoulderAnchor;
        }
    }

    void HandleRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.unscaledDeltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.unscaledDeltaTime;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalClamp, verticalClamp);

        if (javelinThrow.IsAiming())
        {
            float nextYaw = playerBody.eulerAngles.y + mouseX;

            if (wallrunLocked)
            {
                float deltaYaw = Mathf.DeltaAngle(baseYaw, nextYaw);
                deltaYaw = wallOnRight
                    ? Mathf.Clamp(deltaYaw, -horizontalClamp, 0)
                    : Mathf.Clamp(deltaYaw, 0, horizontalClamp);

                nextYaw = baseYaw + deltaYaw;
            }

            playerBody.rotation = Quaternion.Euler(0f, nextYaw, 0f);
        }

        transform.rotation = Quaternion.Euler(verticalRotation, playerBody.eulerAngles.y, 0f);
    }

    void SnapToAnchor()
    {
        if (currentAnchor == null) return;
        transform.position = currentAnchor.position;
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
            );
        }
    }

    public Camera GetCamera()
    {
        return debugRayCamera;
    }
}

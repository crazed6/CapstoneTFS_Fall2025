using UnityEngine;

public class AimingCameraController : MonoBehaviour
{
    [Header("Follow Targets")]
    public Transform leftShoulderAnchor;
    public Transform rightShoulderAnchor;
    public Transform defaultShoulderAnchor;

    [Header("References")]
    public Transform playerBody;
    public PlayerJavelinThrow javelinThrow;

    [Header("Rotation Settings")]
    public float mouseSensitivity = 300f;

    [Header("Clamp Settings")]
    [Tooltip("Max horizontal camera swing (left/right) from player's forward direction.")]
    public float horizontalClamp = 45f;

    [Tooltip("Max vertical camera tilt (up/down).")]
    public float verticalClamp = 80f;

    [Header("FOV Settings")]
    public float normalFOV = 60f;
    public float aimingFOV = 40f;
    public float fovLerpSpeed = 10f;

    private Camera cam;
    private Transform currentAnchor;

    private float verticalRotation = 0f;
    private float baseYaw = 0f;
    private bool wallrunLocked = false;
    private bool wallOnRight = false;

    void Start()
    {
        cam = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        currentAnchor = defaultShoulderAnchor;
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
                Debug.LogWarning("Missing correct shoulder anchor! Using default.");
            }

            // Save base yaw only once when locking begins
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

        // Pitch clamp
        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalClamp, verticalClamp);

        // Clamp player yaw if wallrunning
        if (javelinThrow.IsAiming())
        {
            float nextYaw = playerBody.eulerAngles.y + mouseX;

            if (wallrunLocked)
            {
                float deltaYaw = Mathf.DeltaAngle(baseYaw, nextYaw);

                if (wallOnRight)
                {
                    deltaYaw = Mathf.Clamp(deltaYaw, -horizontalClamp, 0); // Clamp left only
                }
                else
                {
                    deltaYaw = Mathf.Clamp(deltaYaw, 0, horizontalClamp); // Clamp right only
                }

                nextYaw = baseYaw + deltaYaw;
            }

            playerBody.rotation = Quaternion.Euler(0f, nextYaw, 0f);
        }

        // Camera follows player yaw + vertical aim
        transform.rotation = Quaternion.Euler(verticalRotation, playerBody.eulerAngles.y, 0f);
    }

    void SnapToAnchor()
    {
        if (currentAnchor == null) return;
        transform.position = currentAnchor.position; // No lerp, snappy & clean
    }

    void HandleFOV()
    {
        float targetFOV = javelinThrow.IsAiming() ? aimingFOV : normalFOV;
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFOV, Time.unscaledDeltaTime * fovLerpSpeed);
    }

    public Camera GetCamera()
    {
        return cam;
    }
}
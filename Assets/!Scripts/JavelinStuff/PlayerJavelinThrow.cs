using UnityEngine;
using UnityEngine.UI;

public class PlayerJavelinThrow : MonoBehaviour
{
    [Header("Javelin Settings")]
    public GameObject javelinPrefab;
    public Transform javelinSpawnPoint;
    public float throwForce = 50f;
    public float cooldownTime = 2f;

    [Header("UI Settings")]
    public GameObject crosshair;

    [Header("Slow Motion Settings")]
    public float slowMotionTimeScale = 0.1f;
    public float timeResetSpeed = 5f;
    public float slowMotionDuration = 1f;        // Time to stay in slow-mo
    public float timeSlowInSpeed = 5f;           // How quickly to enter slow-mo

    private GameObject currentJavelin;
    private bool isAiming = false;
    private float cooldownTimer = 0f;
    private float slowMotionTimer = 0f;
    private bool isSlowMotionActive = false;
    private bool canTriggerSlowMotion = false;
    private bool isEnteringSlowMotion = false;

    private Camera playerCamera;
    private Rigidbody playerRigidbody;
    private bool wasGrounded;

    void Start()
    {
        playerCamera = CharacterController.instance.playercam;
        playerRigidbody = CharacterController.instance.rb;
        wasGrounded = CharacterController.instance.IsGrounded;

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

        // Smoothly enter slow motion
        if (isEnteringSlowMotion && Time.timeScale > slowMotionTimeScale)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, slowMotionTimeScale, timeSlowInSpeed * Time.unscaledDeltaTime);

            if (Mathf.Abs(Time.timeScale - slowMotionTimeScale) < 0.01f)
            {
                Time.timeScale = slowMotionTimeScale;
                isEnteringSlowMotion = false;
                isSlowMotionActive = true;
                slowMotionTimer = slowMotionDuration;
            }
        }

        // Smoothly exit slow motion -_-
        if (isSlowMotionActive && !isEnteringSlowMotion)
        {
            slowMotionTimer -= Time.unscaledDeltaTime;

            if (slowMotionTimer <= 0f)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, 1f, timeResetSpeed * Time.unscaledDeltaTime);
                if (Mathf.Abs(Time.timeScale - 1f) < 0.01f)
                {
                    Time.timeScale = 1f;
                    isSlowMotionActive = false;
                }
            }
        }

        // Track grounded state -_-
        if (wasGrounded && !CharacterController.instance.IsGrounded)
        {
            OnJumpDetected();
        }

        // Reset slow motion ONLY when the player touches the ground -_-
        if (!wasGrounded && CharacterController.instance.IsGrounded)
        {
            Debug.Log("Player landed — reset slow motion");
            canTriggerSlowMotion = false;
        }

        wasGrounded = CharacterController.instance.IsGrounded;
    }

    void StartAiming()
    {
        if (crosshair) crosshair.SetActive(true);

        if (javelinPrefab && javelinSpawnPoint)
        {
            currentJavelin = Instantiate(javelinPrefab, javelinSpawnPoint.position, javelinSpawnPoint.rotation);
            currentJavelin.transform.SetParent(javelinSpawnPoint);
            isAiming = true;

            // Only trigger slow motion if a jump was detected -_-
            if (canTriggerSlowMotion && !IsWallRunningOrSliding())
            {
                Debug.Log("Starting slow motion transition...");
                isEnteringSlowMotion = true;
                Time.timeScale = 1f; // Start from full speed (just in case)
            }
        }
    }

    void ThrowJavelin()
    {
        if (currentJavelin)
        {
            currentJavelin.transform.SetParent(null);

            // Get throw direction from crosshair -_-
            Vector3 throwDirection = GetThrowDirection();
            currentJavelin.GetComponent<JavelinController>().SetDirection(throwDirection);

            currentJavelin = null;
            isAiming = false;

            if (crosshair) crosshair.SetActive(false);

            cooldownTimer = cooldownTime;

            if (isSlowMotionActive)
            {
                isSlowMotionActive = true;
            }
        }
    }

    Vector3 GetThrowDirection()
    {
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return (hit.point - javelinSpawnPoint.position).normalized;
        }
        else
        {
            return ray.direction;
        }
    }

    bool IsWallRunningOrSliding()
    {
        return CharacterController.instance.IsWallRunning || CharacterController.instance.IsSliding;
    }

    void OnJumpDetected()
    {
        Debug.Log("Jump detected!");
        canTriggerSlowMotion = true; // Persist until player lands -_-
    }

    void HandleCooldown()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.unscaledDeltaTime;
        }
    }
}

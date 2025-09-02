using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class CharacterController : MonoBehaviour
{

    //Jaxson Vignal 

    //player can reattach to same wall need to change that will check for prev wall normal 
    //change slide so that it cant activate in air 

    public static CharacterController instance; // it's me -_-
    public PlayerJavelinThrow pjt;
    //JoshuaC here :3, just adding my thing near the top for easy access

    [Header("Combat")]
    public DamageProfile dashDamageProfile; // Reference to the DashDamage ScriptableObject

    //My part ends here :3, nice to see you Jaxson!

    [HideInInspector] public Rigidbody rb;

    [Header("Movement")]
    public float baseSpeed = 8f;
    private bool isGrounded;
    public float maxSpeed = 30;
    private bool isDashOnCooldown = false;
    public float dashCooldown = 1.5f; // Cooldown duration in seconds

    [Header("Player cam")]
    public CinemachineCameraController cameraController;

   
    [Header("Ground & Wall Checkers")]
    public CollisionDetectorRaycast bottomCollider;
    public CollisionDetectorRaycast rightCollider;
    public CollisionDetectorRaycast leftCollider;

    [Header("Jumping")]
    public float jumpForce = 10f;
    bool jumpInitiated = false;
    float lastSpeedBeforeTakeoff;
    bool isFalling = false;
    private float currentDownwardForce = 0f;
    public float downwardGravityForce;
    public float downwardForceLerpSpeed;
    bool isJumping = false; // For jump animation - Colton

    [Header("Jump Buffer")]
    public float jumpBufferTime = 0.15f; // Duration for buffering input
    private float jumpBufferTimer = -1f; // Timer for jump buffer

    [Header("Coyote Time")]
    public float coyoteTime = 0.1f; // Optional: Allow jump shortly after leaving ground
    private float coyoteTimer = 0f; // Timer for coyote time

    [Header("Look Around")]
    public Transform cameraHolder;
    public float mouseSensitivity = 300f;
    public float verticalClampAngle = 90f;
    public float horizontalClampAngle = 90f;
    float verticalRotation = 0f;

    [Header("Sliding")]
    public float slideSpeedThreshold = 6f; // Minimum speed needed to begin sliding
    public float addedSlideSpeed = 3f; // Adding flat speed + also add a percentage of horizontal speed up to 66% of base speed
    public float slideSpeedDampening = 0.99f; // The speed will be multiplied by this every frame (to stop in ~3 seconds)
    public float keepSlidingSpeedThreshold = 15f; // As long as speed is above this, keep sliding
    public float slideSteeringPower = 0.5f; // How much you can steer around while sliding
    public float slideCooldownTime = 1.5f; // Time in seconds to wait before sliding again
    private float slideCooldownTimer = 0f; // Tracks the cooldown time

    private KnockbackReceiver knockbackReceiver;

    bool isSliding = false;
    bool slideInitiated = false;


    [Header ("Attack settings")]

    public float coneAngle = 30f; // Angle in degrees
    public float coneRange = 10f; // How far the cone reaches
    public LayerMask enemyLayer;

    [Header("Wall Running")]
    // --- Aiden & Kaylani's : bool to lock rotation and editable variable to lock camera angle ---
    public float wallRunLookAwayAngle = 20f; // ADD THIS: Sets the fixed camera angle away from the wall.
    private bool isRotationLocked = false;  // Add this line: Tracks if rotation is locked
    [SerializeField] private float wallRunCamRotationSpeed = 8.0f;
    [SerializeField] private float wallRunSideJumpFactor = 1.5f;
    [SerializeField] private float wallRunUpwardBoost = 1.5f; // Multiplies the vertical jump force
    bool isWallRunLeft = false; // For wall run animation - Colton
    bool isWallRunRight = false; // For wall run animation - Colton

    public bool fallWhileWallRunning; // Slowly fall character while wall running
    public float keepWallRunningSpeedThreshold = 3f; // If speed drops below this, stop wall running
    public Transform playerCameraZRotator; // To be able to rotate the camera on the Z axis without affecting other rotations
    float wallRunStartingSpeed; // The speed that you begin wall running with, will be maintained while you keep wall running
    public float wallrunJumpSpeedBoost = 1.2f;
    public float wallrunStartSpeedBoost = 1.2f;
    public float wallrunJumpforce;
    bool jumpPressedThisFrame = false;

    bool lastWallWasRight = false;
    bool recentlyWallRan = false;
    float wallRunCooldown = 1.0f;
    float lastWallRunTime = -999f;



    bool isWallRunning = false;
    bool onRightWall = false;


    [Header("Ledge Vaulting")]
    public float ledgeDetectDistance = 1f;     // How far forward to check for a wall
    public float ledgeCheckHeight = 1.5f;      // From player's origin, how high to start check
    public float ledgeCheckDown = 2f;          // How far down to check for top of ledge
    public float vaultUpForce = 8f;
    public float vaultForwardForce = 4f;
    public LayerMask ledgeLayerMask;           // Layers to check against (e.g. Default, Environment)

    [Header("Pole Vault")]
    public float upForce;
    public float forwardForce;
    public bool isOnPoleVaultPad;
    public bool isVaulting; // For Pole Vault Animation - Colton

    // Displacement Calculation
    Vector3 lastPosition;
    [HideInInspector] public Vector3 displacement;

    [Header("Player cam")]
    public GameObject playerCamera;

    [Header("attack settings")]
    public Camera playercam;
    public float maxDistance;
    private Vector3 targetPosition;
    public float dashSpeed;
    private bool isMoving;
    public float enemyExitForce;
    [SerializeField] private float dashOvershoot = 5f; // How far past the enemy to dash
    private float dashTimer = 0f;
    private Collider playerCollider;
    private PhysicsMaterial originalMaterial;

    [SerializeField] private float dashForce = 25f;  // Speed of the dash
    [SerializeField] private float dashDuration = 0.2f;  // Duration of the dash

    private bool isDashing = false;  // Flag to check if we're already dashing


    [Header("Visual")]
    public GameObject playerVisual; // Used to rotate/tilt/move player model without affecting the colliders etc.

    public bool IsWallRunning => isWallRunning; // Public getter for isWallRunning -_-
    public bool IsSliding => isSliding; // Public getter for isSliding -_-
    public bool JumpInitiated => jumpInitiated; // Public getter for jumpInitiated -_-
    public bool IsGrounded => isGrounded; // Public getter for isGrounded -_-
    public bool IsDashing => isMoving; // Already tracked as isMoving -_-
    public bool IsWallOnRight => onRightWall; // Public getter for wallRuning directions -_-
    public bool IsJumping => isJumping; // Public getter for jump animation - Colton
    public bool IsWallRunLeft => isWallRunLeft; // Public getter for wallRuning directions - Colton
    public bool IsWallRunRight => isWallRunRight; // Public getter for wallRuning directions - Colton

    public bool IsDashAttackActive => isDashing || isMoving; // Public getter for DashAttack -_-

    [Header("Cutscene Cameras")]
    public GameObject thirdPersonCamera;
    public GameObject cutSceneCamera;
    public GameObject cutScenePlayerCamera;

    void Awake()
    {
        knockbackReceiver = GetComponent<KnockbackReceiver>();

        if (instance == null)
        {
            instance = this;
        }
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;
    }

    void Start()
    {
        isVaulting = false;
        isOnPoleVaultPad = false;
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<Collider>();
        originalMaterial = playerCollider.material;
        lastPosition = transform.position;
        wallrunJumpforce = jumpForce * 1.25f;
        pjt = GetComponent<PlayerJavelinThrow>();
    }

    void Update()
    {
        Vector3 forwardDirection = transform.forward;

        // Update the cooldown timer for sliding
        if (slideCooldownTimer > 0f)
        {
            slideCooldownTimer -= Time.deltaTime;
        }

        // When Space is pressed, set jump buffer timer and flag jumpInitiated
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferTime;  // Start jump buffer timer
            jumpInitiated = true;               // Indicate a jump was initiated
        }

        // Handle slide input with cooldown
        if (Input.GetKeyDown(KeyCode.LeftShift) && slideCooldownTimer <= 0f)
        {
            slideInitiated = true;
            slideCooldownTimer = slideCooldownTime;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            StopSliding();
        }

        // LookUpAndDownWithCamera(); // Uncomment if needed
        RotateBodyHorizontally();  // Smooth horizontal rotation
        poleVault();

        CheckEnemyInCrosshair();

        // Dash if E pressed, and not dashing, wall running or sliding
        if (Input.GetKeyDown(KeyCode.E) && !isDashing && !isWallRunning && !isSliding)
        {
            DashForward();
        }
    }

    void FixedUpdate()
{
        if (knockbackReceiver != null && knockbackReceiver.isBeingKnocked)
        {
            return; // Stop here and let the knockback take over
        }

        SetIsGrounded(bottomCollider.IsColliding);

    if (jumpBufferTimer > 0) jumpBufferTimer -= Time.fixedDeltaTime;
    if (isGrounded) coyoteTimer = coyoteTime;
    else coyoteTimer -= Time.fixedDeltaTime;

    Jump();
    CheckForLedgeVault(); //<-- right here
    WallRun();
    Move();
    Slide();
    
    displacement = (transform.position - lastPosition) * 50;
    lastPosition = transform.position;
    LimitVelocity(maxSpeed);
}
    void Move()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 inputDirection = (transform.right * x + transform.forward * z).normalized;

        // Handle idle grounded movement: stop sliding on slope
        if (isGrounded && inputDirection == Vector3.zero && !isWallRunning && !isSliding)
        {
            // Apply small downward force to stay grounded without bouncing
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
            rb.AddForce(-transform.up * 30f, ForceMode.Acceleration); // Stick to slope
            return;
        }
        if (isWallRunning)
        {
            return; // Wall running handles its own movement
        }

        // Get slope normal
        Vector3 groundNormal = Vector3.up;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1.2f, LayerMask.GetMask("Default")))
        {
            groundNormal = hit.normal;
        }

        // Project input onto slope
        Vector3 slopeAdjustedDirection = Vector3.ProjectOnPlane(inputDirection, groundNormal).normalized;

        float horizontalSpeed = new Vector3(displacement.x, 0, displacement.z).magnitude;
        float speedToApply = Mathf.Max(baseSpeed, horizontalSpeed);

        if (!isGrounded) speedToApply = lastSpeedBeforeTakeoff;
        if (speedToApply > baseSpeed) speedToApply *= isGrounded ? 0.985f : 0.99f;

        Vector3 newVelocity = slopeAdjustedDirection * speedToApply;
        newVelocity.y = rb.linearVelocity.y;

        if (isGrounded)
        {
            rb.linearVelocity = newVelocity;
        }
        else
        {
            rb.AddForce(slopeAdjustedDirection * speedToApply, ForceMode.Force);

            if (horizontalSpeed > baseSpeed)
            {
                Vector3 newHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                newHorizontalVelocity *= 0.98f;
                rb.linearVelocity = new Vector3(newHorizontalVelocity.x, rb.linearVelocity.y, newHorizontalVelocity.z);
            }
        }
    }



    void Jump()
    {
        // Only jump if jump buffer active AND coyote time active AND NOT wallrunning
        if (jumpBufferTimer > 0 && coyoteTimer > 0 && !isWallRunning)
        {
            jumpBufferTimer = -1f; // consume jump buffer
            coyoteTimer = 0f;

            // rest of your jump code...
            currentDownwardForce = 0f;

            lastSpeedBeforeTakeoff = displacement.magnitude;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            isJumping = true; // For jump animation - Colton
        }

        // Custom gravity handling (affects both ascent and descent)
        if (rb.linearVelocity.y > 0) // Going up
        {
            float upwardGravity = downwardGravityForce * 0.5f; // Use half gravity when rising
            rb.AddForce(Vector3.down * upwardGravity, ForceMode.Acceleration);
        }
        else // Falling
        {
            if (rb.linearVelocity.y < 0)
            {
                currentDownwardForce = Mathf.Lerp(currentDownwardForce, downwardGravityForce, Time.deltaTime * downwardForceLerpSpeed);
                rb.AddForce(Vector3.down * currentDownwardForce, ForceMode.Acceleration);

                isJumping = false; // For jump animation - Colton
                isFalling = true;
            }
        }
    }



    // ---- Jaxson's original code: Look Up and Down with Camera ----
    //void RotateBodyHorizontally()
    //{
    //    // Get mouse input for horizontal rotation
    //    float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

    //    // Rotate the player horizontally
    //    transform.Rotate(0f, mouseX, 0f);

    //    // If sliding, allow steering slightly
    //    if (isSliding)
    //    {
    //        Vector3 newVelocity = rb.linearVelocity;
    //        newVelocity = Quaternion.Euler(0, mouseX * slideSteeringPower, 0) * newVelocity;
    //        rb.linearVelocity = newVelocity;
    //    }
    //}


    // Aiden & Kaylani's code: Rotate body horizontally with mouse input
    void RotateBodyHorizontally()
    {
        // --- Aiden & Kaylani's : Check if the player is currently wall running ---
        // If rotation is locked (during a wall run), exit the function immediately.
        if (isRotationLocked)
        {
            return;
        }

        // --- Jaxson's : Normal Rotation Logic ---
        // Get mouse input for horizontal rotation
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;

        // Rotate the player horizontally
        transform.Rotate(0f, mouseX, 0f);

        // If sliding, allow steering slightly
        if (isSliding)
        {
            Vector3 newVelocity = rb.linearVelocity;
            newVelocity = Quaternion.Euler(0, mouseX * slideSteeringPower, 0) * newVelocity;
            rb.linearVelocity = newVelocity;
        }
    }
    void SetIsGrounded(bool state)
    {
        isGrounded = state;
        if (PlayerStatesManager.instance) PlayerStatesManager.instance.SetGroundedState(isGrounded);
        if (!isGrounded && isSliding) StopSliding();
        
    }

    #region Sliding

    void Slide()
    {
        if (slideInitiated)
        {
            if (!isGrounded) return; // Don't cancel the state to slide as soon as you land

            // -- INITIATE --
            slideInitiated = false;

            // TODO If going backwards, or not moving, don't slide (not moving handled by speed threshold)

            // If already sliding... return;
            if (isSliding) return;

            // Can only slide if the "horizontal" speed (X & Z) is above a threshold
            float horizontalSpeed = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z).magnitude;
            if (horizontalSpeed < slideSpeedThreshold) return;

            StartSliding();
        }

        // While sliding, slowly lose momentum until it ends
        if (isSliding)
        {
            // Dampen the speed
            Vector3 newVelocity = rb.linearVelocity * slideSpeedDampening;

            // If the speed is still above the threshold, keep sliding
            if (newVelocity.magnitude > keepSlidingSpeedThreshold) rb.linearVelocity = newVelocity;
            else StopSliding();
        }
    }
    void StartSliding()
    {
        slideInitiated = false;
        SetIsSliding(true);

        GetComponent<PlayerAudio>()?.PlaySlide(); //audio hook

        // Move camera down 1 unit
        //cameraHolder.DOLocalMoveY(cameraHolder.localPosition.y - 0.4f, 0.2f);

        //change this so that it squish the collider and pushes into the ground 
        cameraController.UpdateCameraState(CinemachineCameraController.PlayerState.Default);

        
        //playerVisual.transform.DOLocalRotate(new Vector3(-45, 0, 0), 0.2f);
        //playerVisual.transform.DOLocalMoveY(-0.2f, 0.2f);

        // Add bonus speed
        // TODO: Boost should "dampen" out as the players speed increases the max cap
        float currSpeedModifier = Mathf.Clamp(rb.linearVelocity.magnitude / 40, 0, 1); // Maximum speed at 20
        float boost = addedSlideSpeed + Mathf.Lerp(0, addedSlideSpeed * 2f, currSpeedModifier); // Boost the speed (up to 40% more speed with Y speed)

        // Get direction, get speed, amplify speed, apply
        Vector3 direction = rb.linearVelocity.normalized;

        //? Using displacement.magnitude as the "base" speed, so if player runs into wall etc. momentum resets
        rb.linearVelocity = direction * (displacement.magnitude + boost);

        // Debug.Log($"SLIDE [START] (boost: {boost}, y speed: {Mathf.Abs(rb.velocity.y)} ");
    }

    void StopSliding()
    {
        SetIsSliding(false);

        // Move camera back up
        //cameraHolder.DOLocalMoveY(cameraHolder.localPosition.y + 0.4f, 0.2f);
        cameraController.UpdateCameraState(CinemachineCameraController.PlayerState.Default);
        //playerVisual.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.2f);
        //playerVisual.transform.DOLocalMoveY(0f, 0.2f);

        // Debug.Log("SLIDE [END]");
    }

    void SetIsSliding(bool state)
    {
        isSliding = state;
        if (PlayerStatesManager.instance) PlayerStatesManager.instance.SetSlidingState(isSliding);
    }

    #endregion

    #region Wall Running

    void WallRun()
    {
        // Reset wallrun lockout when grounded or after cooldown
        if (isGrounded || Time.time - lastWallRunTime > wallRunCooldown)
        {
            recentlyWallRan = false;
        }

        if (jumpInitiated && !isGrounded)
        {
            if (isWallRunning)
            {
                int directionCount = 1;

                // Jump off the wall
                Vector3 wallNormal = onRightWall ? rightCollider.outHit.normal : leftCollider.outHit.normal;
                Vector3 jumpDirection = Vector3.up * wallRunUpwardBoost;

                // Push away from wall
                jumpDirection += wallNormal * wallRunSideJumpFactor;

                // Optional forward movement
                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    jumpDirection += transform.forward;
                    directionCount++;
                }

                float magnitude = 1 + (directionCount - 1) * 0.25f;
                jumpDirection = jumpDirection.normalized * magnitude;

                rb.AddForce(jumpDirection * wallrunJumpforce, ForceMode.Impulse);
                rb.linearVelocity = rb.linearVelocity * wallrunJumpSpeedBoost;

                StopWallRunning();

                jumpInitiated = false;
            }
            else
            {
                // Check for starting wallrun on allowed wall only
                if (leftCollider.IsColliding && (!recentlyWallRan || lastWallWasRight))
                {
                    StartWallRunning(false); // left wall
                    jumpInitiated = false;
                    isWallRunLeft = true; // For wall run animation - Colton
                }
                else if (rightCollider.IsColliding && (!recentlyWallRan || !lastWallWasRight))
                {
                    StartWallRunning(true); // right wall
                    jumpInitiated = false;
                    isWallRunRight = true; // For wall run animation - Colton
                }
                else
                {
                    jumpInitiated = false;
                }
            }
        }

        if (isWallRunning)
        {
            if (isGrounded)
            {
                StopWallRunning();
                return;
            }

            if (!leftCollider.IsColliding && !rightCollider.IsColliding)
            {
                StopWallRunning();
                return;
            }

            onRightWall = rightCollider.IsColliding;
            var col = onRightWall ? rightCollider : leftCollider;
            Vector3 wallNormal = col.outHit.normal;

            // NEW: Continuously update rotation based on current wall normal
            UpdateWallRunRotation(wallNormal, onRightWall);

            Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

            if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude)
            {
                wallForward = -wallForward;
            }

            float ySpeed = rb.linearVelocity.y;

            rb.linearVelocity = wallForward * wallRunStartingSpeed;

            if (rb.linearVelocity.magnitude < keepWallRunningSpeedThreshold)
            {
                StopWallRunning();
                return;
            }

            if (fallWhileWallRunning && ySpeed < 0)
                rb.linearVelocity += new Vector3(0, ySpeed * 0.75f, 0);

            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
    }

    // NEW METHOD (Aiden) for continuous rotation updates
    private void UpdateWallRunRotation(Vector3 wallNormal, bool rightWall)
    {
        // Calculate the direction parallel to the wall
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);
        if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        // Create a rotation that looks parallel to the wall
        Quaternion wallRunRotation = Quaternion.LookRotation(wallForward);

        // Add the "look away" angle based on which side the wall is on
        float lookAngle = rightWall ? wallRunLookAwayAngle : -wallRunLookAwayAngle;
        Quaternion lookAwayRotation = Quaternion.Euler(0, lookAngle, 0);

        // Smoothly rotate to the new orientation to avoid jarring transitions
        Quaternion targetRotation = wallRunRotation * lookAwayRotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * wallRunCamRotationSpeed);
    }



    // initiate our wallrun movement (rotation logic moved to continuous handling in the above method)
    void StartWallRunning(bool rightWall)
    {
        isRotationLocked = true; //lock mouse input (Aiden & Kaylani's code)
        SetIsWallRunning(true);

        if (!fallWhileWallRunning)
            rb.useGravity = false;

        // NOTE: Rotation logic moved to UpdateWallRunRotation() which is called continuously during wall running
        // The initial rotation will be set on the first frame of wall running

        // --- VISUALS AND STATE MANAGEMENT (Jaxson's original code) ---
        playerCameraZRotator.DOLocalRotate(new Vector3(0, 0, rightWall ? 20 : -20), 0.2f);
        //playerVisual.transform.DOLocalRotate(new Vector3(0, 0, rightWall ? 20 : -20), 0.2f);

        wallRunStartingSpeed = rb.linearVelocity.magnitude * 1.2f;

        // Set lockout data
        lastWallWasRight = rightWall;
        recentlyWallRan = true;
        lastWallRunTime = Time.time;

        cameraController.UpdateCameraState(rightWall
            ? CinemachineCameraController.PlayerState.WallRunningRight
            : CinemachineCameraController.PlayerState.WallRunningLeft);
    }

    // stop our wallrun movement
    void StopWallRunning()
    {
        isRotationLocked = false; // Unlock the rotation (Aiden & Kaylani's code)
        SetIsWallRunning(false);
        isWallRunLeft = false; // For wall run animation - Colton
        isWallRunRight = false; // For wall run animation - Colton

        if (!fallWhileWallRunning)
            rb.useGravity = true;

        playerCameraZRotator.DOLocalRotate(new Vector3(0, 0, 0), 0.2f);
        playerVisual.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.2f);

        cameraController.UpdateCameraState(CinemachineCameraController.PlayerState.Default);
    }

    // manages switching to and from wallrun state
    void SetIsWallRunning(bool state)
    {
        isWallRunning = state;
        if (PlayerStatesManager.instance)
            PlayerStatesManager.instance.SetWallRunningState(isWallRunning);
    }
    #endregion

    //limits the players speed to a max of 50  to prevent breaking levels
    void LimitVelocity(float maxVelocity)
    {
        // Get the current velocity
        Vector3 currentVelocity = rb.linearVelocity;

        // If the magnitude of the velocity is greater than the max allowed velocity, clamp it
        if (currentVelocity.magnitude > maxVelocity)
        {
            // Set the velocity to the maximum allowed value while keeping the direction
            rb.linearVelocity = currentVelocity.normalized * maxVelocity;
        }
    }


    void poleVault()
    {
        if (Input.GetKeyDown(KeyCode.F) && isGrounded && !isSliding && isOnPoleVaultPad)
        {
            GetComponent<PlayerAudio>()?.PlayPoleVault(); //audio hook

            // Mark player as airborne
            isGrounded = false;

            isVaulting = true; // For Pole Vault Animation - Colton

            //// Apply immediate forward impulse
            //rb.AddForce(transform.forward * forwardForce, ForceMode.Impulse); -------- Moved to EndVaultAfterDelay - Colton

            // Start upward impulse over short time
            //StartCoroutine(SmoothPoleVaultImpulse()); ------- Moved to EndVaultAfterDelay - Colton

            StartCoroutine(EndVaultAfterDelay(0.5f)); // End vaulting state after 1.2 seconds - Colton
        }
    }
    // Ends vaulting state after a delay - Colton
    private IEnumerator EndVaultAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isVaulting = false;
        // Apply immediate forward impulse
        rb.AddForce(transform.forward * forwardForce, ForceMode.Impulse);

        StartCoroutine(SmoothPoleVaultImpulse());
    }

    IEnumerator SmoothPoleVaultImpulse()
    {
        float duration = 0.3f;
        float timer = 0f;

        // Keep gravity on for natural arc
        rb.useGravity = true;

        while (timer < duration)
        {
            float t = timer / duration;

            // Apply small upward impulses each frame, decaying over time
            rb.AddForce(Vector3.up * upForce * (1 - t) * Time.deltaTime, ForceMode.VelocityChange);

            timer += Time.deltaTime;
            yield return null;
        }
    }

    void DashForward()
    {
        if (!isDashing && !isDashOnCooldown)
        {
            GetComponent<PlayerAudio>()?.PlayDash(); //audio hook
            StartCoroutine(DashRoutine());
        }
    }

    private IEnumerator DashRoutine()
    {
        isDashing = true;
        isDashOnCooldown = true;

        float elapsed = 0f;
        Vector3 direction = playerCamera.transform.forward;

        // Reset velocity to prevent any existing movement during dash
        rb.linearVelocity = Vector3.zero;

        while (elapsed < dashDuration)
        {
            Vector3 dashVelocity = direction * dashSpeed;
            rb.linearVelocity = new Vector3(dashVelocity.x, rb.linearVelocity.y, dashVelocity.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        isDashing = false;

        // Start cooldown timer
        yield return new WaitForSeconds(dashCooldown);
        isDashOnCooldown = false;
    }


    private HashSet<EnemyDamageComponent> damagedThisDash = new HashSet<EnemyDamageComponent>();

    private void CheckEnemyInCrosshair()
    {
        // Start dash
        if (Input.GetMouseButtonDown(0) && !isMoving)
        {
            // Find best target in cone
            Collider[] hits = Physics.OverlapSphere(transform.position, coneRange, enemyLayer);

            Transform bestTarget = null;
            float bestDistance = Mathf.Infinity;

            foreach (var hit in hits)
            {
                if (hit.CompareTag("enemy"))
                {
                    Vector3 toTarget = hit.transform.position - transform.position;
                    Vector3 toTargetXZ = new Vector3(toTarget.x, 0f, toTarget.z).normalized;

                    float horizontalAngle = Vector3.Angle(transform.forward, toTargetXZ);
                    float verticalDifference = Mathf.Abs(toTarget.y);
                    float verticalTolerance = coneRange * 0.75f;

                    if (horizontalAngle <= coneAngle * 0.5f && verticalDifference <= verticalTolerance)
                    {
                        float distance = toTarget.magnitude;
                        if (distance < bestDistance)
                        {
                            bestDistance = distance;
                            bestTarget = hit.transform;
                        }
                    }
                }
            }

            if (bestTarget != null)
            {
                // Set dash state
                isMoving = true;
                isDashing = true;
                dashTimer = 0f;
                damagedThisDash.Clear(); // Reset for this dash

                // Compute target
                Vector3 dashDirection = (bestTarget.position - transform.position).normalized;
                targetPosition = bestTarget.position + dashDirection * dashOvershoot;

                // Reduce friction temporarily
                if (playerCollider.material != null)
                {
                    PhysicsMaterial dashMat = new PhysicsMaterial();
                    dashMat.dynamicFriction = 0f;
                    dashMat.staticFriction = 0f;
                    dashMat.frictionCombine = PhysicsMaterialCombine.Minimum;
                    playerCollider.material = dashMat;
                }

                // Reset velocity and apply impulse
                rb.linearVelocity = Vector3.zero;
                rb.AddForce(dashDirection * dashSpeed, ForceMode.VelocityChange);
            }
        }

        // Update dash
        if (isMoving)
        {
            dashTimer += Time.deltaTime;

            // Apply continuous force towards target (physics-based steering)
            Vector3 toTarget = (targetPosition - transform.position);
            Vector3 moveDirection = toTarget.normalized;

            // Calculate desired velocity
            Vector3 desiredVelocity = moveDirection * dashSpeed;

            // Calculate steering force to maintain dash speed and direction
            Vector3 steeringForce = (desiredVelocity - rb.linearVelocity) * 10f; // Adjust multiplier as needed

            // Apply the steering force
            rb.AddForce(steeringForce, ForceMode.Force);

            // --- Manual enemy overlap check ---
            Collider[] enemiesHit = Physics.OverlapSphere(transform.position, 1f, enemyLayer);
            foreach (var enemy in enemiesHit)
            {
                ApplyDashDamage(enemy);
            }

            // End dash after duration
            if (dashTimer >= dashDuration)
            {
                isMoving = false;
                isDashing = false;
                dashTimer = 0f;
                damagedThisDash.Clear(); // Ready for next dash

                // Restore friction
                if (playerCollider.material != null)
                    playerCollider.material = originalMaterial;

                // Keep momentum forward
                rb.linearVelocity = moveDirection * rb.linearVelocity.magnitude * 1.2f;
            }
        }
    }

    // Helper method to apply damage only once per dash
    private void ApplyDashDamage(Collider other)
    {
        EnemyDamageComponent dmg = other.GetComponentInParent<EnemyDamageComponent>();
        if (dmg != null && dashDamageProfile != null && !damagedThisDash.Contains(dmg))
        {
            float scaledDamage = dashDamageProfile.damageAmount * rb.linearVelocity.magnitude;

            DamageData dashDamage = new DamageData
            {
                source = gameObject,
                profile = dashDamageProfile,
                customDamage = scaledDamage
            };

            dmg.TakeDamage2(dashDamage);
            damagedThisDash.Add(dmg); // Prevent multiple hits
            Debug.Log("Dash hit enemy: " + other.name);
        }
    }








    private void CheckForLedgeVault()
    {
        if (!isGrounded || isSliding || isWallRunning) return;

        Vector3 origin = transform.position + Vector3.up * ledgeCheckHeight;
        Vector3 forward = transform.forward;

        // Step 1: Detect wall in front
        if (Physics.Raycast(origin, forward, out RaycastHit wallHit, ledgeDetectDistance, ledgeLayerMask))
        {
            // Step 2: Check if there's a surface above it we can stand on
            Vector3 ledgeCheckOrigin = wallHit.point + Vector3.up * 0.5f + forward * 0.1f;

            if (Physics.Raycast(ledgeCheckOrigin, Vector3.down, out RaycastHit ledgeHit, ledgeCheckDown, ledgeLayerMask))
            {
                // Vault: Reset Y velocity, and apply upward & forward force
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
                rb.AddForce(Vector3.up * vaultUpForce, ForceMode.Impulse);
                rb.AddForce(forward * vaultForwardForce, ForceMode.Impulse);

                // Optional: trigger animation or sound here
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Vector3 origin = transform.position;
        Vector3 forward = transform.forward;

        // Draw the cone boundary lines
        Quaternion leftRotation = Quaternion.Euler(0, -coneAngle / 2f, 0);
        Quaternion rightRotation = Quaternion.Euler(0, coneAngle / 2f, 0);

        Vector3 leftDir = leftRotation * forward;
        Vector3 rightDir = rightRotation * forward;

        Gizmos.DrawLine(origin, origin + leftDir * coneRange);
        Gizmos.DrawLine(origin, origin + rightDir * coneRange);

        // Draw arc to represent cone
        int segments = 20;
        for (int i = 0; i < segments; i++)
        {
            float angle1 = -coneAngle / 2f + (coneAngle / segments) * i;
            float angle2 = -coneAngle / 2f + (coneAngle / segments) * (i + 1);

            Vector3 dir1 = Quaternion.Euler(0, angle1, 0) * forward;
            Vector3 dir2 = Quaternion.Euler(0, angle2, 0) * forward;

            Gizmos.DrawLine(origin + dir1 * coneRange, origin + dir2 * coneRange);
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("CanPoleVault"))
        {
            isOnPoleVaultPad = true;
                
        }
        else
        {
            isOnPoleVaultPad = false;
        }


    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Triggered with: " + other.name);

        if (isMoving && other.CompareTag("enemy"))
        {
            Debug.Log("Hit enemy trigger: " + other.name);
        }
        if (isMoving && other.CompareTag("enemy"))
        {
            // Look for damage component on this object or its parent
            EnemyDamageComponent dmg = other.GetComponentInParent<EnemyDamageComponent>();
            if (dmg != null && dashDamageProfile != null)
            {
                float speed = rb.linearVelocity.magnitude;
                float scaledDamage = dashDamageProfile.damageAmount * speed;

                DamageData dashDamage = new DamageData
                {
                    source = gameObject,
                    profile = dashDamageProfile,
                    customDamage = scaledDamage
                };

                dmg.TakeDamage2(dashDamage);
                Debug.Log("Dash hit enemy and applied " + scaledDamage + " damage to: " + other.name);
            }
            else
            {
                Debug.LogWarning("EnemyDamageComponent missing on " + other.name);
            }
        }
    }

}


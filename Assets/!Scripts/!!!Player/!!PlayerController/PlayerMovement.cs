using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Unity.VisualScripting;

public class CharacterController : MonoBehaviour
{

    //Jaxson Vignal 

    //clips through the wall during dash movement assuming its because speed jumps to 500 during dash and colliders cant keep up 
    //player can reattach to same wall need to change that will check for prev wall normal 
    //change slide so that it cant activate in air 

    public static CharacterController instance; // it's me -_-

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

    bool isSliding = false;
    bool slideInitiated = false;

    [Header("Wall Running")]
    public bool fallWhileWallRunning; // Slowly fall character while wall running
    public float keepWallRunningSpeedThreshold = 3f; // If speed drops below this, stop wall running
    public Transform playerCameraZRotator; // To be able to rotate the camera on the Z axis without affecting other rotations
    float wallRunStartingSpeed; // The speed that you begin wall running with, will be maintained while you keep wall running
    public float wallrunJumpSpeedBoost = 1.2f;
    public float wallrunJumpforce;
    bool jumpPressedThisFrame = false;

    bool isWallRunning = false;
    bool onRightWall = false;

    [Header("Pole Vault")]
    public float upForce;
    public float forwardForce;

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


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;
        wallrunJumpforce = jumpForce * 1.25f;
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
        // Update grounded state first for accurate jump/coyote checks
        SetIsGrounded(bottomCollider.IsColliding);

        // Decrease jump buffer timer
        if (jumpBufferTimer > 0)
            jumpBufferTimer -= Time.fixedDeltaTime;

        // Reset coyote timer if grounded, else decrease it
        if (isGrounded)
            coyoteTimer = coyoteTime;
        else
            coyoteTimer -= Time.fixedDeltaTime;

        // Call jump logic (consumes jumpBuffer and coyote timers)
        Jump();

        // Call wall run logic (checks jumpInitiated)
        WallRun();

        // Movement and sliding
        Move();
        Slide();

        // Update displacement and last position for speed tracking
        displacement = (transform.position - lastPosition) * 50;
        lastPosition = transform.position;

        // Clamp velocity to max speed
        LimitVelocity(maxSpeed);
    }

    void Move()
    {
        // Move with WASD (the direction)
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        Vector3 movementDirection = transform.right * x + transform.forward * z;

        // If wall running & sliding, don't take input
        if (isSliding) return; // TODO: If moving in the opposite direction "cancel sliding", ...maybe
        if (isWallRunning) return;

        // Otherwise, if no input, stop fast if grounded
        if (movementDirection == Vector3.zero)
        {
            // Dampen speed fast
            if (isGrounded) rb.linearVelocity = rb.linearVelocity * 0.95f;
            return;
        }

        //* Applying the Movement

        // Only consider horizontal velocity for movement (on ground OR in air)
        //? On the ground horizontal vel. is 0, in air if you add it then you can keep speeding up infinitely
        float horizontalSpeed = new Vector3(displacement.x, 0, displacement.z).magnitude; //? This used to be rb.velocity...
        float speedToApply = Mathf.Max(baseSpeed, horizontalSpeed); // If the player is going FASTER than the limit, cap it there

        // If in the air, keep the LAST speed before takeoff
        //? Otherwise, the player can keep speeding up infinitely with gravity
        if (!isGrounded) speedToApply = lastSpeedBeforeTakeoff;

        // If the player is going over the limit, dampen it a bit
        if (speedToApply > baseSpeed) speedToApply *= isGrounded ? 0.985f : 0.99f; // Dampens harder while on the ground

        // The new velocity to apply
        Vector3 newVelocity = movementDirection.normalized * speedToApply;
        newVelocity.y = rb.linearVelocity.y; // Keep the current vertical speed

        //? If in the air we add force instead of modifying the velocity so that the gravity can do its thing
        if (isGrounded) rb.linearVelocity = newVelocity;
        else rb.AddForce(movementDirection.normalized * speedToApply, ForceMode.Force);

        // If player is going too fast HORIZONTALLY in AIR => dampen HORIZONTAL speed
        //? Otherwise the speed applied above goes out of control
        if (!isGrounded && horizontalSpeed > baseSpeed)
        {
            Vector3 newHorizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            newHorizontalVelocity *= 0.98f;
            rb.linearVelocity = new Vector3(newHorizontalVelocity.x, rb.linearVelocity.y, newHorizontalVelocity.z);
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
            isFalling = false;
            currentDownwardForce = 0f;

            lastSpeedBeforeTakeoff = displacement.magnitude;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // custom gravity code unchanged
        if (rb.linearVelocity.y <= 0)
        {
            if (!isFalling)
                isFalling = true;

            currentDownwardForce = Mathf.Lerp(currentDownwardForce, downwardGravityForce, Time.deltaTime * downwardForceLerpSpeed);
            rb.AddForce(Vector3.down * currentDownwardForce, ForceMode.Acceleration);
        }
        else
        {
            isFalling = false;
        }
    }



    void RotateBodyHorizontally()
    {
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

        // Move camera down 1 unit
        //cameraHolder.DOLocalMoveY(cameraHolder.localPosition.y - 0.4f, 0.2f);

        //change this so that it squish the collider and pushes into the ground 
        cameraController.UpdateCameraState(CinemachineCameraController.PlayerState.Default);
        playerVisual.transform.DOLocalRotate(new Vector3(-45, 0, 0), 0.2f);
        playerVisual.transform.DOLocalMoveY(-0.2f, 0.2f);

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
        playerVisual.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.2f);
        playerVisual.transform.DOLocalMoveY(0f, 0.2f);

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
        if (jumpInitiated && !isGrounded)
        {
            if (isWallRunning)
            {
                int directionCount = 1; // Can be up to 3 directions, magnitude can be 1, 1.25, 1.5 depending

                // Jump off the wall
                Vector3 jumpDirection = Vector3.up;

                // If holding forward, add a force forward
                if (Input.GetAxisRaw("Vertical") > 0)
                {
                    jumpDirection += transform.forward;
                    directionCount++;
                }

                // If holding the horizontal direction AWAY from the wall, add that horizontal direction as well
                if (Input.GetAxisRaw("Horizontal") < 0 && onRightWall)
                {
                    jumpDirection += rightCollider.outHit.normal;
                    directionCount++;
                }
                else if (Input.GetAxisRaw("Horizontal") > 0 && !onRightWall)
                {
                    jumpDirection += leftCollider.outHit.normal;
                    directionCount++;
                }

                // Normalize to prevent artificial speed boost
                float magnitude = 1 + (directionCount - 1) * 0.25f;
                jumpDirection = jumpDirection.normalized * magnitude;

                rb.AddForce(jumpDirection * wallrunJumpforce, ForceMode.Impulse);
                rb.linearVelocity = rb.linearVelocity * wallrunJumpSpeedBoost;

                StopWallRunning();

                jumpInitiated = false; // Consume the jump input here
            }
            else
            {
                // Try to start wallrunning if near a wall
                if (leftCollider.IsColliding) StartWallRunning(false);
                else if (rightCollider.IsColliding) StartWallRunning(true);

                jumpInitiated = false; // Consume the jump input here as well
            }
        }

        if (isWallRunning)
        {
            // Stop wallrun if grounded
            if (isGrounded)
            {
                StopWallRunning();
                return;
            }

            // Stop wallrun if no walls are detected
            if (!leftCollider.IsColliding && !rightCollider.IsColliding)
            {
                StopWallRunning();
                return;
            }

            // Determine which wall we are running on
            onRightWall = rightCollider.IsColliding;
            var col = onRightWall ? rightCollider : leftCollider;
            Vector3 wallNormal = col.outHit.normal;

            // Calculate wall forward direction
            Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

            // Align wallForward with player forward direction
            if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude)
            {
                wallForward = -wallForward;
            }

            // Keep vertical speed component for smooth movement
            float ySpeed = rb.linearVelocity.y;

            // Set velocity along the wall
            rb.linearVelocity = wallForward * wallRunStartingSpeed;

            // Stop wallrun if speed too low
            if (rb.linearVelocity.magnitude < keepWallRunningSpeedThreshold)
            {
                StopWallRunning();
                return;
            }

            // Apply some vertical velocity while wallrunning (simulate slight falling)
            if (fallWhileWallRunning && ySpeed < 0)
                rb.linearVelocity += new Vector3(0, ySpeed * 0.75f, 0);

            // Push player towards the wall
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
    }


    //initiate our wallrun movement 
    void StartWallRunning(bool rightWall)
    {
        
        SetIsWallRunning(true);

        //disable players gravity (if set that way in inspector)
        if (!fallWhileWallRunning) rb.useGravity = false;

        // Rotate camera Z 20 degrees away from wall
        playerCameraZRotator.DOLocalRotate(new Vector3(0, 0, rightWall ? 20 : -20), 0.2f);
        playerVisual.transform.DOLocalRotate(new Vector3(0, 0, rightWall ? 20 : -20), 0.2f);

        wallRunStartingSpeed = rb.linearVelocity.magnitude + 5f;

        // Debug.Log($"WALL RUN [START] (spd: {wallRunStartingSpeed})");
        //cameraController.SetFollowTarget(rightWall ? cameraController.wallFollowRight : cameraController.wallFollowLeft); // AIDEN CAMERA ADJUSTMENT
       
                cameraController.UpdateCameraState(rightWall
            ? CinemachineCameraController.PlayerState.WallRunningRight
            : CinemachineCameraController.PlayerState.WallRunningLeft);
    }

    //stop our wallrun movement 
    void StopWallRunning()
    {
        SetIsWallRunning(false);

        //reactivate players gravity 
        if (!fallWhileWallRunning) rb.useGravity = true;

        // Rotate camera and player Z to 0
        playerCameraZRotator.DOLocalRotate(new Vector3(0, 0, 0), 0.2f); ;
        playerVisual.transform.DOLocalRotate(new Vector3(0, 0, 0), 0.2f);

        //cameraController.SetFollowTarget(cameraController.defaultFollowTarget); // AIDEN CAMERA ADJUSTMENT  
        cameraController.UpdateCameraState(CinemachineCameraController.PlayerState.Default);

    }

    //manages switching to and from wallrun state
    void SetIsWallRunning(bool state)
    {
        isWallRunning = state;
        if (PlayerStatesManager.instance) PlayerStatesManager.instance.SetWallRunningState(isWallRunning);
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
        if (Input.GetKeyDown(KeyCode.F) && isGrounded && !isSliding)
        {
            // Apply initial vault forces
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z); // Preserve some horizontal momentum
            rb.AddForce(transform.up * upForce, ForceMode.Impulse);
            rb.AddForce(transform.forward * forwardForce, ForceMode.Impulse);

            // Allow air control after vaulting
            isGrounded = false; // Mark the player as airborne
        }
    }

    void DashForward()
    {
        if (!isDashing && !isDashOnCooldown)
        {
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


    void CheckEnemyInCrosshair()
    {
        //saves rb curent speed
        Vector3 lastspeed = rb.linearVelocity;

        //check for mouse down
        if (Input.GetMouseButtonDown(0)) // Left click
        {

            //fire raycast from camera position
            Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
            RaycastHit hit;

            //check if raycast hits object with enemy tag
            if (Physics.Raycast(ray, out hit, maxDistance))
            {
                if (hit.collider.CompareTag("enemy"))
                {
                    Debug.Log("Moving to enemy: " + hit.collider.name);
                    
                    //find position for enemy 
                    targetPosition = hit.collider.transform.position;
                    isMoving = true;
                }
            }
        }

        if (isMoving)
        {
            //move player to enemy position 
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, dashSpeed * Time.deltaTime);
            
            //stop movement when player is within 0.5 units of the enemy
            if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
            {
                isMoving = false;

                //reapply the rb velocity with a multiple of 1.2
                rb.linearVelocity = lastspeed * 1.2f;
            }
        }
    
    }
}


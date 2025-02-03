using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class controller : MonoBehaviour
{
    CharacterController cc;

    public Transform cameraTransform; // Reference to the camera's transform
    public Transform groundCheckRayCast;

    Vector3 move;
    Vector3 input;
    Vector3 yVelocity;
    Vector3 forwardDirection;
    float gravity;
    bool isGrounded;
    float speed;
    public float runSpeed;
    public float normalGravity;
    public LayerMask whatIsGround;

    [Header("Jump settings")]
    public float jumpHeight;
    int jumpCharges;

    [Header("Air Settings")]
    public float AirSpeed;

    [Header("Wallrun")]
    public LayerMask whatIsWall;
    bool isWallRunning;
    public float wallrunGravity;
    public float wallSpeedIncrease;
    public float wallSpeedDecrease;
    public RaycastHit leftWallHit;
    public RaycastHit rightWallHit;
    Vector3 wallNormal;
    bool onLeftWall;
    bool onRightWall;
    bool hasWallRan = false;
    Vector3 lastWallNormal;

    [Header("Camera settings")]
    public Camera playerCam;
    float normalFOV;
    public float wallrunFOV;
    public float cameraChangeTimer;
    public float wallrunCameraTilt;
    public float tilt;

    public float groundCheckDistance = 0.3f; // Distance for raycast to check if grounded

    // Variables for tracking apex and gravity increase
    private float apexTime = -1f; // Time when the player reaches the apex
    private bool wasAscending = false; // Flag to check if the player was ascending

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        normalFOV = playerCam.fieldOfView;
    }

    void InputHandler()
    {
        input = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        input = transform.TransformDirection(input);
        input = Vector3.ClampMagnitude(input, 1f);

        if (Input.GetKeyUp(KeyCode.Space) && jumpCharges > 0)
        {
            jump();
        }
    }

    // Update is called once per frame
    void Update()
    {
        checkWallRun();
        InputHandler();


        if (isGrounded)
        {
            GroundMovement();

        }
        else if (!isGrounded && !isWallRunning)
        {
            AirMovement();
        }

        else if (isWallRunning)
        {
            wallRunMovement();
            decreaseSpeed(wallSpeedDecrease);
        }

        // Check for apex (transition from ascending to descending)
        CheckApex();

        // Rotate the character to always face away from the camera
        RotateCharacter();

        groundChecker();
        cc.Move(move * Time.deltaTime);
        ApplyGravity();
        cameraEffects();
    }

    void cameraEffects()
    {
        float fov = isWallRunning ? wallrunFOV : normalFOV;
        playerCam.fieldOfView = Mathf.Lerp(playerCam.fieldOfView, fov, cameraChangeTimer * Time.deltaTime);

        if (isWallRunning)
        {
            if (onRightWall)
            {
                tilt = Mathf.Lerp(tilt, wallrunCameraTilt, cameraChangeTimer * Time.deltaTime);
            }

            if (onLeftWall)
            {
                tilt = Mathf.Lerp(tilt, -wallrunCameraTilt, cameraChangeTimer * Time.deltaTime);
            }
        }

        if (!isWallRunning)
        {
            tilt = Mathf.Lerp(tilt, 0f, cameraChangeTimer * Time.deltaTime);
        }
    }

    void GroundMovement()
    {
        speed = runSpeed;

        move = Vector3.zero; // Reset movement

        if (input.x != 0)
        {
            move.x += input.x * speed;
        }

        if (input.z != 0)
        {
            move.z += input.z * speed;
        }

        move = Vector3.ClampMagnitude(move, speed);
    }

    // Raycast to check if the character is grounded
    void groundChecker()
    {
        // Cast a ray downward from the character's position
        RaycastHit hit;
        isGrounded = Physics.Raycast(groundCheckRayCast.position, Vector3.down, out hit, groundCheckDistance, whatIsGround);

        if (isGrounded)
        {
            jumpCharges = 1; // Reset jump charges when grounded

        }
    }

    void ApplyGravity()
    {
        gravity = isWallRunning ? wallrunGravity : normalGravity;
        // Check if the player is falling (velocity is negative in Y and not grounded)
        if (!isGrounded && yVelocity.y < 0)
        {
            // Increase gravity once the player starts falling after reaching the apex
            gravity = Mathf.Lerp(normalGravity, normalGravity * 3f, Time.time - apexTime);
        }
        else
        {
            // Reset gravity to normal when grounded or ascending
            gravity = normalGravity;
        }

        // Apply gravity to the Y velocity
        yVelocity.y += gravity * Time.deltaTime;

        // Move the character with the updated gravity
        cc.Move(yVelocity * Time.deltaTime);
    }

    void jump()
    {
        if (!isGrounded && !isWallRunning)
        {
            jumpCharges -= 1;
        }
        else if (isWallRunning)
        {
            exitWallRun();
            increaseSpeed(wallSpeedIncrease);
        }
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2f * normalGravity);


    }

    void AirMovement()
    {
        move.x += input.x * AirSpeed;
        move.z += input.z * AirSpeed;

        move = Vector3.ClampMagnitude(move, AirSpeed);
    }

    // Function to rotate the character to face away from the camera
    void RotateCharacter()
    {
        // Get the direction from the camera to the character
        Vector3 directionToCamera = (cameraTransform.position - transform.position).normalized;

        // Remove the y-axis component to ensure the character rotates only in the horizontal plane
        directionToCamera.y = 0;

        // Calculate the opposite direction (away from the camera)
        Vector3 oppositeDirection = -directionToCamera;

        // Rotate the character to face the opposite direction of the camera
        if (move.magnitude > 0) // Only rotate when the character is moving
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(oppositeDirection), Time.deltaTime * 10f);
        }
    }

    // Track when the player reaches the apex (when they stop ascending and start falling)
    void CheckApex()
    {
        if (yVelocity.y > 0 && !wasAscending) // The player was moving up and has now stopped ascending
        {
            apexTime = Time.time; // Store the time when the apex is reached
            wasAscending = true; // Set flag to indicate the player has passed the apex
        }

        if (yVelocity.y < 0)
        {
            wasAscending = false; // The player is falling
        }
        else if (yVelocity.y > 0)
        {
            wasAscending = true; // The player is ascending
        }
    }

    void increaseSpeed(float speedIncrease)
    {
        speed += speedIncrease;
    }

    void decreaseSpeed(float speedDecrease)
    {
        speed -= speedDecrease;
    }


    void checkWallRun()
    {
        onLeftWall = Physics.Raycast(transform.position, -transform.right, out leftWallHit, 2f, whatIsWall);
        onRightWall = Physics.Raycast(transform.position, transform.right, out rightWallHit, 2f, whatIsWall);

        if ((onLeftWall || onRightWall) && !isWallRunning)
        {
            wallrun();
        }
        if ((onLeftWall || onRightWall) && isWallRunning)
        {
            exitWallRun();
        }
    }

    void wallrun()
    {
        isWallRunning = true;
        jumpCharges = 1;
        increaseSpeed(wallSpeedIncrease);
        yVelocity = new Vector3(0f, 0f, 0f);

        wallNormal = onLeftWall ? leftWallHit.normal : rightWallHit.normal;

        forwardDirection = Vector3.Cross(wallNormal, Vector3.up);

        if (Vector3.Dot(forwardDirection, transform.forward) < 0f)
        {
            forwardDirection = -forwardDirection;
        }
    }

    void exitWallRun()
    {
        isWallRunning = false;
        lastWallNormal = wallNormal;
    }

    void wallRunMovement()
    {
        if (input.z > (forwardDirection.z - 10f) && input.z < (forwardDirection.z + 10f))
        {
            move += forwardDirection;
        }
        else if (input.z < (forwardDirection.z - 10f) && input.z > (forwardDirection.z + 10f))
        {
            move.x = 0f;
            move.y = 0f;
            exitWallRun();
        }

        move.x += input.x * AirSpeed;
        move = Vector3.ClampMagnitude(move, speed);

    }

    void testWallRun()
    {


        if (hasWallRan)
        {

            float wallAngle = Vector3.Angle(wallNormal, lastWallNormal);

            if (wallAngle > 15)
            {
                wallrun();
            }
        }
        else
        {
            wallrun();
            hasWallRan = true;

        }
    }
}

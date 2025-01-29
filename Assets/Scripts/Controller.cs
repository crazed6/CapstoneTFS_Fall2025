using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class controller : MonoBehaviour
{
    CharacterController cc;

    public Transform cameraTransform; // Reference to the camera's transform
    public Transform groundCheckRayCast;

    Vector3 move;
    Vector3 input;
    Vector3 yVelocity;
    float gravity;
    bool isGrounded;
    float speed;
    public float runSpeed;
    public float normalGravity;
    public LayerMask whatIsGround;
    public float increase;
    public float decrease;
    float originalHeight;
    float slideHeight;

    [Header("Jump settings")]
    public float jumpHeight;
    int jumpCharges;

    [Header("Air Settings")]
    public float AirSpeed;

    public float groundCheckDistance = 0.3f; // Distance for raycast to check if grounded

    // Variables for tracking apex and gravity increase
    private float apexTime = -1f; // Time when the player reaches the apex
    private bool wasAscending = false; // Flag to check if the player was ascending

    Vector3 forwardDirection;
    bool isSliding;
    float slideTime;
    public float maxSlideTime;
    public float slideSpeedIncrease;
    public float slideSpeedDecrease;

    [Header("Interaction Settings")]
    public float interactionDistance = 2.0f; // Distance within which objects can be interacted with
    private Transform currentInteractable; // Reference to the closest interactable object

    // Start is called before the first frame update
    void Start()
    {
        cc = GetComponent<CharacterController>();
        originalHeight = cc.height;
        slideHeight = originalHeight * .5f;
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

        if (Input.GetKeyDown(KeyCode.LeftControl) && (move.x > 0 || move.z > 0))
        {
            Slide();
        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            ExitSlide();
        }

        // Interaction logic
        if (Input.GetKeyDown(KeyCode.E) && currentInteractable != null)
        {
            
        }
    }

    // Update is called once per frame
    void Update()
    {
        InputHandler();

        if (isGrounded && !isSliding)
        {
            GroundMovement();
        }
        else if (!isGrounded)
        {
            AirMovement();
        }

        else if (isSliding)
        {
            SlideMovement();
            SpeedDecrease(decrease);
            slideTime -= 1f * Time.deltaTime;
        }

        if (slideTime <= 0f)
        {
            isSliding = false;
        }

        // Check for apex (transition from ascending to descending)
        CheckApex();

        // Rotate the character to always face away from the camera
        RotateCharacter();

        groundChecker();
        cc.Move(move * Time.deltaTime);
        ApplyGravity();

        // Update the current interactable object
        UpdateCurrentInteractable();
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
        if (!isGrounded && yVelocity.y < 0)
        {
            gravity = Mathf.Lerp(normalGravity, normalGravity * 3f, Time.time - apexTime);
        }
        else
        {
            gravity = normalGravity;
        }

        yVelocity.y += gravity * Time.deltaTime;
        cc.Move(yVelocity * Time.deltaTime);
    }

    void jump()
    {
        yVelocity.y = Mathf.Sqrt(jumpHeight * -2f * normalGravity);
    }

    void AirMovement()
    {
        move.x += input.x * AirSpeed;
        move.z += input.z * AirSpeed;

        move = Vector3.ClampMagnitude(move, AirSpeed);
    }

    void RotateCharacter()
    {
        Vector3 directionToCamera = (cameraTransform.position - transform.position).normalized;
        directionToCamera.y = 0;
        Vector3 oppositeDirection = -directionToCamera;

        if (move.magnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(oppositeDirection), Time.deltaTime * 10f);
        }
    }

    void CheckApex()
    {
        if (yVelocity.y > 0 && !wasAscending)
        {
            apexTime = Time.time;
            wasAscending = true;
        }

        if (yVelocity.y < 0)
        {
            wasAscending = false;
        }
        else if (yVelocity.y > 0)
        {
            wasAscending = true;
        }
    }

    void Slide()
    {
        transform.localScale = new Vector3(transform.localScale.x, slideHeight, transform.localScale.z);

        if (isGrounded)
        {
            isSliding = true;
            forwardDirection = transform.forward;
            slideTime = maxSlideTime;
            SpeedIncrease(increase);
        }
    }

    void SpeedIncrease(float increase)
    {
        speed += increase;
    }

    void SpeedDecrease(float decrease)
    {
        speed -= decrease * Time.deltaTime;
    }

    void SlideMovement()
    {
        move += forwardDirection;
        move = Vector3.ClampMagnitude(move, speed);
    }

    void ExitSlide()
    {
        isSliding = false;
        transform.localScale = new Vector3(transform.localScale.x, originalHeight, transform.localScale.z);
    }

    void UpdateCurrentInteractable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, interactionDistance);
        currentInteractable = null;

        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Interactable"))
            {
                currentInteractable = collider.transform;
                break;
            }
        }
    }

    
}

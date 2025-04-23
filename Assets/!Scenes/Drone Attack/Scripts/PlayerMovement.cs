using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 9f; // Movement speed
    private Rigidbody rb;
    private Camera mainCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main; // Get the main camera reference

        // Ensure Rigidbody settings are correctly set for physics interactions
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.mass = 1f;
        rb.linearDamping = 1f;  // Adds slight friction to smooth movement
        rb.angularDamping = 1f;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        // Set collision detection mode to continuous to avoid missing fast-moving objects
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Get camera forward and right vectors to move relative to the camera's direction
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        // Ignore vertical movement (Y-axis)
        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement direction based on camera orientation
        Vector3 move = (cameraRight * moveX + cameraForward * moveZ).normalized * speed;

        // Apply velocity to Rigidbody
        rb.linearVelocity = new Vector3(move.x, rb.linearVelocity.y, move.z);
    }

    void OnCollisionEnter(Collision collision)
    {
        // If the player collides with a drone, destroy the drone immediately
        if (collision.gameObject.CompareTag("Drone"))
        {
            Debug.Log("Player hit by Drone! Destroying drone.");
            Destroy(collision.gameObject); // Destroy the drone on collision
        }
    }
}

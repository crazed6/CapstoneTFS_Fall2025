using UnityEngine;

public class JavelinController : MonoBehaviour
{
    [Header("Arc Settings")]
    public float speed = 50f;                // Initial launch speed of the javelin -_-
    public float gravityStrength = 30f;      // Custom gravity strength for creating arc -_-
    public float lifetime = 5f;              // Time after which the javelin auto-destroys -_-

    [Header("Rotation")]
    public float rotationSpeed = 720f;       // Z-axis spin speed for visual flair -_-

    private Vector3 velocity;                // Current velocity of the javelin in world space -_-
    private bool isFlying = false;           // Whether the javelin is currently in flight -_-

    // Called when the javelin is thrown to initialize its direction and start movement -_-
    public void SetDirection(Vector3 direction)
    {
        velocity = direction.normalized * speed;                    // Set the initial velocity in the throw direction -_-
        transform.rotation = Quaternion.LookRotation(velocity);     // Rotate javelin to face the initial velocity direction -_-
        isFlying = true;                                            // Enable movement logic -_-
        Destroy(gameObject, lifetime);                              // Auto-destroy after a certain time -_-
    }

    // Update is called once per frame -_-
    void Update()
    {
        if (!isFlying) return;                                      // If not flying, skip movement logic -_-

        velocity += Vector3.down * gravityStrength * Time.deltaTime; // Apply custom gravity to simulate arc -_-

        transform.position += velocity * Time.deltaTime;            // Move javelin according to current velocity -_-

        if (velocity != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(velocity.normalized); // Align javelin rotation with new trajectory -_-

        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self); // Optional Z-axis spin effect -_-
    }

    // Handle collision with any object -_-
    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject); // Destroy javelin on collision -_-
    }
}


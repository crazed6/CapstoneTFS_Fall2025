using UnityEngine;

public class JavelinController : MonoBehaviour
{
    [Header("Flight Settings")]
    public float speed = 50f;
    public float lifetime = 5f;

    [Header("Arc Settings")]
    public float gravity = -9.81f; // affects arc -_-
    private float verticalVelocity = 0f;

    [Header("Rotation Settings")]
    public float rotationSpeed = 720f; // spin around Z -_-

    private Vector3 direction;
    private bool isThrown = false;

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
        isThrown = true;
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        if (!isThrown) return;

        // Simulate vertical arc -_-
        verticalVelocity += gravity * Time.deltaTime;

        Vector3 move = direction * speed * Time.deltaTime;
        move += Vector3.up * verticalVelocity * Time.deltaTime;

        transform.position += move;

        // Rotate javelin to match new velocity direction (forward + arc) -_-
        Vector3 newForward = direction * speed + Vector3.up * verticalVelocity;
        transform.rotation = Quaternion.LookRotation(newForward.normalized);

        // corkscrew spin for flair -_-
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}

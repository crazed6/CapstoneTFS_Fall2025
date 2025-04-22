using UnityEngine;

public class JavelinController : MonoBehaviour
{
    public float speed = 50f;
    public float lifetime = 5f; // Destroy after 5 seconds if no hit -_-
    public float rotationSpeed = 720f;

    private Vector3 direction;
    private bool isThrown = false;

    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
        transform.rotation = Quaternion.LookRotation(direction);
        isThrown = true;
        Destroy(gameObject, lifetime); // Destroy after timeout -_-
    }

    void Update()
    {
        if (isThrown)
        {
            // Direct movement -_-
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            // Rotate around Z-axis -_-
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Destroy javelin immediately on impact -_-
        Destroy(gameObject);
    }
}

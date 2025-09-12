using UnityEngine;
public class LedgeSnapDown360 : MonoBehaviour
{
    public Rigidbody rb;
    public float downwardForce = 20f; // Strong downward push
    public float groundNormalMaxAngle = 45f; // Max angle from up to be considered "ground"

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision fired");
        bool foundValidContact = false;

        foreach (ContactPoint contact in collision.contacts)
        {
            // Check if the surface is "ground-like" within a reasonable angle
            float angle = Vector3.Angle(contact.normal, Vector3.up);

            // Also check if contact point is below character center (hitting ledge top, not wall)
            bool contactBelowCenter = contact.point.y < transform.position.y;

            Debug.Log($"Contact: normal={contact.normal}, angle={angle:F1}°, below center={contactBelowCenter}, point Y={contact.point.y:F2}, player Y={transform.position.y:F2}");

            if (angle <= groundNormalMaxAngle && contactBelowCenter)
            {
                Debug.Log("SNAPPING!");
                SnapDown();
                foundValidContact = true;
                break;
            }
        }

        if (!foundValidContact)
        {
            Debug.Log("NO SNAP - no valid contact found");
        }
    }

    private void SnapDown()
    {
        // Cancel upward velocity
        if (rb.linearVelocity.y > 0f)
        {
            Vector3 vel = rb.linearVelocity;
            vel.y = 0f;
            rb.linearVelocity = vel;
        }
        // Apply strong downward force
        rb.AddForce(Vector3.down * downwardForce, ForceMode.VelocityChange);
    }
}
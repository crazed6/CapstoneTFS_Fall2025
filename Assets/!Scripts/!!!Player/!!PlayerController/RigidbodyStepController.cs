using UnityEngine;

public class LedgeSnapDown360 : MonoBehaviour
{
    public Rigidbody rb;
    public float downwardForce = 20f; // Strong downward push
    public float groundNormalMaxAngle = 45f; // Max angle from up to be considered "ground"

    private void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            // Check if the surface is "ground-like" within a reasonable angle
            float angle = Vector3.Angle(contact.normal, Vector3.up);
            if (angle <= groundNormalMaxAngle)
            {
                SnapDown();
                break;
            }
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

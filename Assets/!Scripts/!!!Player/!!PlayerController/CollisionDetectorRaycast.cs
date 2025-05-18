using UnityEngine;

public class CollisionDetectorRaycast : MonoBehaviour
{
    public LayerMask detectionLayers; // Select layers from the inspector
    public bool IsColliding { get; private set; }
    public Vector3 rayDirection = Vector3.forward; // Default direction
    Vector3 finalRayDirection;
    public float rayLength = 1.0f; // Default length
    public bool rotateWithTransform = false; // If true, the ray will rotate with the transform

    // Not used... but works :)
    public delegate void CollisionStateChangedAction(bool state);
    public event CollisionStateChangedAction OnCollisionStateChanged;

    [HideInInspector] public RaycastHit outHit;

    private void Update()
    {
        finalRayDirection = rotateWithTransform ? transform.TransformDirection(rayDirection) : rayDirection;

        RaycastHit hit;
        bool hitDetected = Physics.Raycast(transform.position, finalRayDirection.normalized * rayLength, out hit, rayLength, detectionLayers);
        outHit = hit;

        if (hitDetected && !IsColliding)
        {
            SetCollisionState(true);
        }
        else if (!hitDetected && IsColliding)
        {
            SetCollisionState(false);
        }
    }

    void SetCollisionState(bool state)
    {
        if (IsColliding != state)
        {
            IsColliding = state;
            OnCollisionStateChanged?.Invoke(IsColliding);
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the ray in the scene view for visualization
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, finalRayDirection.normalized * rayLength);
    }
}

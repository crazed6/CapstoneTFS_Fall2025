using UnityEngine;

public class SpeedLinesFollower : MonoBehaviour
{
    public Transform targetCamera; // Camera to follow
    public Vector3 offset = new Vector3(0, 0, 0.5f); // Local offset from camera
    public bool followRotation = true;

    void Start()
    {
        if (targetCamera == null)
        {
            if (Camera.main != null)
                targetCamera = Camera.main.transform;
            else
                Debug.LogError("SpeedLinesFollower: No camera assigned and no main camera found.");
        }
    }

    void LateUpdate()
    {
        if (targetCamera == null) return;

        // Apply offset in camera's local space
        transform.position = targetCamera.position +
                             targetCamera.right * offset.x +
                             targetCamera.up * offset.y +
                             targetCamera.forward * offset.z;

        if (followRotation)
            transform.rotation = targetCamera.rotation;
    }
}

// Asad with Help of Joshua and Ritwik
using UnityEngine;

public class ExplodingPetalAnimator : MonoBehaviour
{
    [Header("Petal References")]
    public Transform[] petals; // Assign your 3 petal transforms here

    [Header("Spin Settings")]
    public float baseSpinSpeed = 180f;   // Spin speed when timer starts
    public float maxSpinSpeed = 720f;    // Max spin speed near explosion

    [Header("Movement Settings")]
    public float startDistanceFromCenter = 1f;
    public float minDistanceFromCenter = 0.05f;

    [Header("Spin Reference Point")]
    public Transform spinCenter; // The point petals spin around

    [Header("Explosion Link")]
    public ExplodingEnemy explodingEnemy; // Reference to ExplodingEnemy script

    private float explosionDuration;

    void Start()
    {
        if (explodingEnemy == null)
        {
            explodingEnemy = GetComponent<ExplodingEnemy>();
            if (explodingEnemy == null)
            {
                Debug.LogError("ExplodingEnemy reference missing on " + gameObject.name);
                enabled = false;
                return;
            }
        }

        if (spinCenter == null)
        {
            spinCenter = transform; // fallback to this object's transform
        }

        explosionDuration = explodingEnemy.outerRadiusTimerStart;
    }

    void Update()
    {
        if (explodingEnemy == null || petals.Length == 0) return;
        if (!explodingEnemy.TimerStarted) return;

        float timerPercent = Mathf.Clamp01(explodingEnemy.explosionTimer / explosionDuration);

        // Spin speed ramps up as timer counts down
        float currentSpinSpeed = Mathf.Lerp(maxSpinSpeed, baseSpinSpeed, timerPercent);

        // Petals move towards center as timer counts down
        float currentDistance = Mathf.Lerp(minDistanceFromCenter, startDistanceFromCenter, timerPercent);

        for (int i = 0; i < petals.Length; i++)
        {
            if (petals[i] == null) continue;

            // Spin clockwise around the spinCenter on X axis (negative angle for clockwise)
            petals[i].RotateAround(spinCenter.position, Vector3.right, -currentSpinSpeed * Time.deltaTime);

            // Direction from spin center to petal
            Vector3 dirFromCenter = (petals[i].position - spinCenter.position).normalized;

            // Move petal closer to center along that direction
            petals[i].position = Vector3.Lerp(
                spinCenter.position + dirFromCenter * startDistanceFromCenter,
                spinCenter.position + dirFromCenter * minDistanceFromCenter,
                1 - timerPercent
            );
        }
    }
}

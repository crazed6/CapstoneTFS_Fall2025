using System.Collections;
using UnityEngine;

//Diego's Script
public class ChaserDroneRework : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;                  // Reference to the player
    public float detectionRange = 30f;        // How far the drone can detect the player
    public float chargeUpTime = 1.5f;         // How long it charges up before attacking
    public float kamikazeSpeed = 30f;         // Speed of the kamikaze attack

    [Header("Line Renderer")]
    public LineRenderer lineRenderer;         // Line Renderer component used for aiming visuals
    public Material lineMaterial;             // Material for the line (should support color changes)
    public float lineWidth = 0.05f;           // Width of the line

    [Header("Explosion Settings")]
    public GameObject explosionEffect;        // Particle effect prefab for explosion
    public float explosionRadius = 5f;        // AOE radius to check for player hit on explosion

    private bool isCharging = false;          // Whether the drone is currently charging up
    private bool hasFired = false;            // Whether the drone has already fired
    private Vector3 attackDirection;          // Final locked direction for the kamikaze
    private Rigidbody rb;                     // Rigidbody reference for physics-based movement

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Find the player by tag if not assigned
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null)
                player = foundPlayer.transform;
        }

        // Setup the line renderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = false;
    }

    void Update()
    {
        // If the drone already attacked or player is not assigned, do nothing
        if (hasFired || player == null)
            return;

        // Check distance to player
        float distance = Vector3.Distance(transform.position, player.position);

        // If within detection range, begin charging and attack
        if (distance <= detectionRange)
        {
            StartCoroutine(ChargeAndKamikaze());
        }
    }

    IEnumerator ChargeAndKamikaze()
    {
        isCharging = true;
        hasFired = true;

        lineRenderer.enabled = true;

        float elapsed = 0f;

        // Charge-up visual: flashing line that turns solid red in the last 0.5s
        while (elapsed < chargeUpTime)
        {
            elapsed += Time.deltaTime;

            Color flashColor;

            // Final warning: solid red in the last 0.5s
            if (elapsed >= chargeUpTime - 0.5f)
            {
                flashColor = Color.red;
            }
            else
            {
                // Alternate between red and white every 0.1s
                bool flashWhite = Mathf.FloorToInt(elapsed * 10f) % 2 == 0;
                flashColor = flashWhite ? Color.white : Color.red;
            }

            lineRenderer.startColor = flashColor;
            lineRenderer.endColor = flashColor;

            // Continuously track the player position
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, player.position);

            yield return null;
        }

        // Lock in the final direction toward the player
        Vector3 lockedTargetPos = player.position;
        attackDirection = (lockedTargetPos - transform.position).normalized;

        isCharging = false;
        lineRenderer.enabled = false;

        // Launch the drone at high speed
        rb.isKinematic = false;
        rb.linearVelocity = attackDirection * kamikazeSpeed;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Log who we hit directly
        Debug.Log("Drone collided with: " + collision.collider.name);

        // Check for direct hit on the player
        if (collision.collider.CompareTag("Player"))
        {
            Debug.Log("Player directly hit by Chaser Drone!");
        }

        // Check for AOE explosion hit using Physics.OverlapSphere
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        bool aoeHit = false;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Player hit by drone explosion AOE!");
                aoeHit = true;
            }
        }

        if (!aoeHit)
        {
            Debug.Log("Player NOT hit by AOE.");
        }

        // Spawn explosion effect
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        // Draw explosion radius in scene view (for 1 second)
        StartCoroutine(DebugDrawExplosionRadius());

        // Delay destruction slightly to allow logs and effects to appear
        Destroy(gameObject, 0.1f);
    }

    // Draw a debug wire sphere in the Scene view to visualize the explosion radius
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1, 0, 0, 0.3f); // semi-transparent red
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    // Debug draw the explosion radius as lines during gameplay
    IEnumerator DebugDrawExplosionRadius()
    {
        float debugTime = 1f;
        float elapsed = 0f;

        while (elapsed < debugTime)
        {
            DrawDebugSphere(transform.position, explosionRadius, Color.red);
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    // Helper to draw a ring on XZ plane to visualize radius
    void DrawDebugSphere(Vector3 center, float radius, Color color)
    {
        int segments = 24;
        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle1 = i * angleStep * Mathf.Deg2Rad;
            float angle2 = (i + 1) * angleStep * Mathf.Deg2Rad;

            Vector3 point1 = center + new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * radius;
            Vector3 point2 = center + new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * radius;

            Debug.DrawLine(point1, point2, color); // draw ring on XZ plane
        }
    }
}

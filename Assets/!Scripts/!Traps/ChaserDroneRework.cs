using System.Collections;
using UnityEngine;

// Diego + Josh + Kaylani + Updates
public class ChaserDroneRework : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform player;             // Reference to the player
    public float detectionRange = 30f;   // How far the drone can detect the player
    public float chargeUpTime = 1.5f;    // How long it charges up before attacking
    public float kamikazeSpeed = 30f;    // Speed of the kamikaze attack

    [Header("Collision Mode")]
    public bool useTriggerCollision = false; // Toggle between physical collision and trigger collision for explosion

    [Header("Rotation Settings")]
    public float rotationSpeed = 8f;     // How quickly the drone rotates to face target

    [Header("Line Renderer")]
    public LineRenderer lineRenderer;    // Line Renderer component used for aiming visuals
    public Material lineMaterial;        // Material for the line (should support color changes)
    public float lineWidth = 0.05f;      // Width of the line

    [Header("Explosion Settings")]
    public GameObject explosionEffect;   // Particle effect prefab for explosion
    public float explosionRadius = 5f;   // AOE radius to check for player hit on explosion

    private bool isCharging = false;     // Whether the drone is currently charging up
    private bool hasFired = false;       // Whether the drone has already fired
    private bool hasLaunched = false;    // Whether the drone has launched (new flag)
    private Vector3 attackDirection;     // Final locked direction for the kamikaze
    private Rigidbody rb;                // Rigidbody reference for physics-based movement

    // Josh Addition for Damage Profile
    public DamageProfile ChaserDroneDirect; // Reference to the DamageProfile ScriptableObject
    public DamageProfile ChaserDroneAoE;    // Reference to the DamageProfile ScriptableObject

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Find the player by tag if not assigned
        if (player == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");
            if (foundPlayer != null) player = foundPlayer.transform;
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
        if (hasFired || player == null) return;

        // Check distance to player
        float distance = Vector3.Distance(transform.position, player.position);

        // If within detection range, begin charging and attack
        if (distance <= detectionRange)
        {
            StartCoroutine(ChargeAndKamikaze());
        }
    }

    void FixedUpdate()
    {
        // Handle rotation in FixedUpdate for better physics integration
        if (player == null) return;

        if (isCharging && !hasLaunched)
        {
            // Rotate toward player during charging
            Vector3 dir = (player.position - transform.position).normalized;
            if (dir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                rb.rotation = Quaternion.Slerp(rb.rotation, targetRot, Time.fixedDeltaTime * rotationSpeed);
            }
        }
        else if (hasLaunched && attackDirection.sqrMagnitude > 0.001f)
        {
            // Rotate toward locked attack direction after launch
            Quaternion targetRot = Quaternion.LookRotation(attackDirection);
            rb.rotation = Quaternion.Slerp(rb.rotation, targetRot, Time.fixedDeltaTime * rotationSpeed * 2f); // Faster rotation during flight
        }
    }

    IEnumerator ChargeAndKamikaze()
    {
        isCharging = true;
        hasFired = true;
        lineRenderer.enabled = true;

        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        MaterialPropertyBlock[] blocks = new MaterialPropertyBlock[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            blocks[i] = new MaterialPropertyBlock();

        float elapsed = 0f;
        float baseIntensity = 2f;
        float pulseMin = 5f;
        float pulseMax = 8f;
        float finalIntensity = 10f;

        while (elapsed < chargeUpTime)
        {
            elapsed += Time.deltaTime;
            float intensity;

            // Timing cutoffs
            float pulseStart = chargeUpTime - 0.5f; // last 0.5s = pulse
            float finalStart = chargeUpTime - 0.1f; // last 0.1s = max

            if (elapsed < pulseStart)
            {
                intensity = baseIntensity; // steady glow
            }
            else if (elapsed < finalStart)
            {
                // Rapid pulsing
                float t = Mathf.PingPong((elapsed - pulseStart) * 12f, 1f);
                intensity = Mathf.Lerp(pulseMin, pulseMax, t);
            }
            else
            {
                intensity = finalIntensity; // locked max
            }

            // Apply emission via MPB
            for (int i = 0; i < renderers.Length; i++)
            {
                if (renderers[i] == null) continue;

                Color emissionColor = Color.white * intensity;
                blocks[i].SetColor("_EmissionColor", emissionColor);
                renderers[i].SetPropertyBlock(blocks[i]);
            }

            // Keep line aimed at player
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, player.position);

            yield return null;
        }

        // Lock in final direction and launch
        Vector3 lockedTargetPos = player.position;
        attackDirection = (lockedTargetPos - transform.position).normalized;
        isCharging = false;
        hasLaunched = true;
        lineRenderer.enabled = false;

        rb.isKinematic = false;
        rb.linearVelocity = attackDirection * kamikazeSpeed;
    }

    // PHYSICAL COLLISION METHOD (Original)
    void OnCollisionEnter(Collision collision)
    {
        if (useTriggerCollision) return; // Only work if physical collision mode is enabled

        Debug.Log("Drone collided with: " + collision.collider.name);
        HandleExplosion(collision.collider);
    }

    // TRIGGER COLLISION METHOD (New)
    void OnTriggerEnter(Collider other)
    {
        if (!useTriggerCollision) return; // Only work if trigger mode is enabled

        Debug.Log("Drone triggered with: " + other.name);
        HandleExplosion(other);
    }

    // Shared explosion logic for both collision methods
    void HandleExplosion(Collider hitCollider)
    {
        if (hitCollider.CompareTag("Player"))
        {
            Debug.Log("Player directly hit by Chaser Drone!");

            Health playerHealth = hitCollider.GetComponent<Health>();
            if (playerHealth != null && ChaserDroneDirect != null)
            {
                DamageData damageData = new DamageData(gameObject, ChaserDroneDirect);
                playerHealth.PlayerTakeDamage(damageData);
            }
        }

        // Check AOE
        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        bool aoeHit = false;
        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Debug.Log("Player hit by drone explosion AOE!");
                aoeHit = true;

                Health playerHealth = hit.GetComponent<Health>();
                if (playerHealth != null && ChaserDroneAoE != null)
                {
                    DamageData damageData = new DamageData(gameObject, ChaserDroneAoE);
                    playerHealth.PlayerTakeDamage(damageData);
                }
            }
        }

        if (!aoeHit)
            Debug.Log("Player NOT hit by AOE.");

        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);

        StartCoroutine(DebugDrawExplosionRadius());

        Destroy(gameObject, 0.1f);
    }

    void OnDrawGizmosSelected()
    {
        // Draw explosion radius
        Gizmos.color = new Color(1, 0, 0, 0.3f);
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        // Draw detection range
        Gizmos.color = new Color(0, 1, 0, 0.2f);
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }

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
            Debug.DrawLine(point1, point2, color);
        }
    }
}
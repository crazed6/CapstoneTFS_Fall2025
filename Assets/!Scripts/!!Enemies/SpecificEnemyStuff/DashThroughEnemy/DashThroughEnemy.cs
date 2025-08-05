//Ritwik -_-
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DashThroughEnemy : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Assign player transform. If left null, it auto-finds first object with tag Player on Start.")]
    public Transform player;

    [Header("Hover")]
    public float hoverAmplitude = 0.25f;      // vertical bob amount
    public float hoverFrequency = 1.5f;       // speed of bob
    public float hoverPhaseOffset = 0f;       // randomize per instance if desired

    [Header("Detection & Facing")]
    public float detectionRange = 25f;        // radial detection distance
    public bool useLineOfSight = true;        // optional ray test
    public LayerMask obstructionMask;         // walls/geometry
    public float rotationSpeed = 6f;          // how fast to face the player (smoother = lower)

    [Header("Gizmos")]
    public bool showGizmos = true;
    public Color rangeColor = new Color(1f, 0.2f, 0.2f, 0.2f);

    // --- internals ---
    private Vector3 _basePos;
    private Quaternion _baseRot;

    private void Awake()
    {
        // Ensure collider is trigger so player can pass through freely.
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void Start()
    {
        _basePos = transform.position;
        _baseRot = transform.rotation;

        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }
    }

    private void Update()
    {
        Hover();

        if (player != null && PlayerInSight())
        {
            FacePlayer();
        }
        else
        {
            // optional: drift back to original rotation if you want
            // transform.rotation = Quaternion.Slerp(transform.rotation, _baseRot, Time.deltaTime * rotationSpeed * 0.5f);
        }
    }

    private void Hover()
    {
        float y = Mathf.Sin((Time.time + hoverPhaseOffset) * hoverFrequency) * hoverAmplitude;
        // Keep XZ fixed, only move Y relative to base
        Vector3 target = new Vector3(_basePos.x, _basePos.y + y, _basePos.z);
        transform.position = target;
    }

    private bool PlayerInSight()
    {
        Vector3 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;
        if (dist > detectionRange) return false;

        if (!useLineOfSight) return true;

        // Simple LoS ray at chest height
        Vector3 origin = transform.position + Vector3.up * 1.2f;
        Vector3 target = player.position + Vector3.up * 1.2f;

        if (Physics.Linecast(origin, target, obstructionMask))
            return false;

        return true;
    }

    private void FacePlayer()
    {
        Vector3 dir = (player.position - transform.position);
        dir.y = 0f; // keep upright
        if (dir.sqrMagnitude < 0.0001f) return;

        Quaternion look = Quaternion.LookRotation(dir.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, look, Time.deltaTime * rotationSpeed);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        Gizmos.color = rangeColor;
        Gizmos.DrawWireSphere(Application.isPlaying ? _basePos : transform.position, detectionRange);
    }
#endif
}

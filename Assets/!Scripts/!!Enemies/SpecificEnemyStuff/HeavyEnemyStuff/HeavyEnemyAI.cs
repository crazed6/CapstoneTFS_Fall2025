using UnityEngine;
using Cysharp.Threading.Tasks;
using System;
using System.Threading;

public class HeavyEnemyAI : MonoBehaviour
{
    [Header("Player Detection")]
    public Transform player;

    [Header("Detection Zones")]
    public float detectionRange = 40f;
    public float firingZoneRange = 50f;

    [Header("Enemy Rotation Settings")]
    public float rotationSpeed = 5f;
    public Quaternion originalRotation;

    [Header("Line of Sight Obstruction")]
    public LayerMask obstructionLayer;

    [Header("Shooting Settings")]
    public Transform rockSpawnPoint;
    public GameObject rockPrefab;

    [Header("Shooting Timings")]
    public float trackDuration = 2f;
    public float lockDuration = 0.5f;
    public float postThrowCooldown = 1f;

    [Header("Rock Settings")]
    public float shootSpeed = 35.0f;

    [Header("Visual Trajectory")]
    public LineRenderer trajectoryLine;
    public Gradient normalColor;
    public Gradient lockedColor;

    [Header("Trajectory Settings")]
    public int trajectoryResolution = 30;
    public float trajectoryHeight = 3f;

    [Header("Slam Attack Settings")]
    public float slamRadius = 5f;
    public float slamKnockbackForce = 25f;
    public float slamCooldown = 3f;
    public float slamDamage = 25f;
    public Transform slamOrigin;

    [Header("Slam Timing")]
    public float slamHitDelay = 0.15f;

    [HideInInspector] public bool isSlamming = false;
    [HideInInspector] public bool slamOnCooldown = false;

    public HeavyEnemyStateMachine stateMachine;
    private bool isTracking = false;
    private Vector3 lockedTarget;
    private CancellationTokenSource trackingTokenSource;
    private GameObject lastDashedPlayer = null;
    private float dashImmunityDuration = 0.25f;
    private float dashImmunityTimer = 0f;
    public GameObject LastDashedPlayer => lastDashedPlayer;

    // === New Dash Cancel System ===
    [Header("Dash Cancel Detection")]
    public SphereCollider dashDetectionCollider; // Adjustable collider
    public LayerMask playerLayer;
    public ParticleSystem dashAttackEffect;

    [HideInInspector] public bool cantDashAttack = false;
    private ParticleSystem activeEffectInstance;
    // ===============================

    // Damage Profile Reference
    public DamageProfile GroundSlam;

    public Action OnSlam; //audio hook
    public event Action OnThrowRock; //audio hook
    public event Action OnLockOn; //audio hook
    public event Action OnStopTracking; //audio hook



    private void Start()
    {
        stateMachine = new HeavyEnemyStateMachine();
        stateMachine.ChangeState(new HeavyEnemyIdleState(this));
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        stateMachine.Execute();

        if (lastDashedPlayer != null)
        {
            dashImmunityTimer -= Time.deltaTime;
            if (dashImmunityTimer <= 0f)
            {
                lastDashedPlayer = null;
            }
        }

        // 🔴 Dash Cancel Sphere Check
        CheckDashAttack();
    }

    private void CheckDashAttack()
    {
        if (dashDetectionCollider == null) return;

        // Sphere position/scale
        Vector3 center = dashDetectionCollider.transform.TransformPoint(dashDetectionCollider.center);
        float radius = dashDetectionCollider.radius * Mathf.Max(
            dashDetectionCollider.transform.lossyScale.x,
            dashDetectionCollider.transform.lossyScale.y,
            dashDetectionCollider.transform.lossyScale.z
        );

        Collider[] hits = Physics.OverlapSphere(center, radius, playerLayer);

        bool playerInside = false;
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                playerInside = true;
                break;
            }
        }

        if (playerInside && !cantDashAttack)
        {
            cantDashAttack = true;

            if (dashAttackEffect != null && activeEffectInstance == null)
            {
                activeEffectInstance = Instantiate(dashAttackEffect, transform.position, Quaternion.identity, transform);
                activeEffectInstance.Play();
            }
        }
        else if (!playerInside && cantDashAttack)
        {
            cantDashAttack = false;

            if (activeEffectInstance != null)
            {
                activeEffectInstance.Stop();
                Destroy(activeEffectInstance.gameObject, 2f);
                activeEffectInstance = null;
            }
        }
    }

    public bool CanSeePlayer()
    {
        if (this == null || gameObject == null || !this || !gameObject.activeInHierarchy || player == null)
            return false;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance > firingZoneRange) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        if (angle > 90f) return false;

        Vector3 eyeLevel = transform.position + Vector3.up * 1.5f;
        Vector3[] points = {
            player.position + Vector3.up * 1.0f,
            player.position + Vector3.up * 1.8f,
            player.position + Vector3.up * 0.5f
        };

        foreach (var point in points)
        {
            if (!Physics.Linecast(eyeLevel, point, obstructionLayer))
            {
                Debug.DrawLine(eyeLevel, point, Color.green, 0.1f);
                return true;
            }
            else
            {
                Debug.DrawLine(eyeLevel, point, Color.red, 0.1f);
            }
        }

        return false;
    }

    public async void StartTracking()
    {
        if (this == null || gameObject == null || !gameObject.activeInHierarchy)
            return;

        if (isTracking) return;
        isTracking = true;

        OnLockOn?.Invoke(); //audio hook

        trackingTokenSource?.Cancel();
        trackingTokenSource = new CancellationTokenSource();
        CancellationToken token = trackingTokenSource.Token;

        float timer = 0f;

        try
        {
            while (timer < trackDuration)
            {
                if (token.IsCancellationRequested || this == null || gameObject == null || !gameObject.activeInHierarchy)
                    return;

                if (player != null && CanSeePlayer())
                {
                    lockedTarget = player.position;
                    DrawTrajectory(lockedTarget, false);
                }

                timer += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, token);

                if (this == null || gameObject == null || !gameObject.activeInHierarchy)
                    return;
            }

            if (token.IsCancellationRequested || this == null || gameObject == null || !gameObject.activeInHierarchy)
                return;

            if (trajectoryLine != null && trajectoryLine.gameObject.activeInHierarchy)
                trajectoryLine.colorGradient = lockedColor;

            await UniTask.Delay(TimeSpan.FromSeconds(lockDuration), cancellationToken: token);

            if (token.IsCancellationRequested || this == null || gameObject == null || !gameObject.activeInHierarchy)
                return;

            ThrowRockAtLockedPosition(lockedTarget);

            isTracking = false;

            await UniTask.Delay(TimeSpan.FromSeconds(postThrowCooldown), cancellationToken: token);

            if (token.IsCancellationRequested || this == null || gameObject == null || !gameObject.activeInHierarchy)
                return;

            if (Vector3.Distance(transform.position, player.position) <= firingZoneRange && CanSeePlayer())
            {
                stateMachine.ChangeState(new HeavyEnemyShootingState(this));
            }
            else
            {
                StopTracking();
                stateMachine.ChangeState(new HeavyEnemyIdleState(this));
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("StartTracking() was safely cancelled.");
        }
    }

    public void StopTracking()
    {
        trackingTokenSource?.Cancel();
        trackingTokenSource = null;

        trajectoryLine.enabled = false;
        isTracking = false;

        // AUDIO: notify listeners to stop lock-on sound
        OnStopTracking?.Invoke();
    }

    public void OnPlayerDashThrough(GameObject player)
    {
        lastDashedPlayer = player;
        dashImmunityTimer = dashImmunityDuration;
    }

    private void DrawTrajectory(Vector3 target, bool isLocked)
    {
        if (trajectoryLine == null || !trajectoryLine.gameObject.activeInHierarchy)
            return;

        trajectoryLine.enabled = true;
        trajectoryLine.positionCount = trajectoryResolution;

        Vector3 start = rockSpawnPoint.position;
        Vector3 mid = (start + target) / 2 + Vector3.up * trajectoryHeight;

        for (int i = 0; i < trajectoryResolution; i++)
        {
            float t = i / (float)(trajectoryResolution - 1);
            Vector3 point = QuadraticBezier(start, mid, target, t);
            trajectoryLine.SetPosition(i, point);
        }

        trajectoryLine.colorGradient = isLocked ? lockedColor : normalColor;
    }

    private Vector3 QuadraticBezier(Vector3 a, Vector3 b, Vector3 c, float t)
    {
        return (1 - t) * (1 - t) * a + 2 * (1 - t) * t * b + t * t * c;
    }

    private void ThrowRockAtLockedPosition(Vector3 target)
    {
        trajectoryLine.enabled = false;

        GameObject rock = Instantiate(rockPrefab, rockSpawnPoint.position, Quaternion.identity);
        HeavyEnemyRock rockScript = rock.GetComponent<HeavyEnemyRock>();

        rockScript.Launch(target, shootSpeed, trajectoryHeight);

        OnThrowRock?.Invoke();//audio hook
    }

    public async UniTaskVoid StartSlamCooldown()
    {
        slamOnCooldown = true;
        await UniTask.Delay(TimeSpan.FromSeconds(slamCooldown));
        slamOnCooldown = false;
    }

    // Debug Gizmo for Dash Cancel
    private void OnDrawGizmosSelected()
    {
        if (dashDetectionCollider != null)
        {
            Gizmos.color = cantDashAttack ? Color.red : Color.green;
            Vector3 center = dashDetectionCollider.transform.TransformPoint(dashDetectionCollider.center);
            float radius = dashDetectionCollider.radius * Mathf.Max(
                dashDetectionCollider.transform.lossyScale.x,
                dashDetectionCollider.transform.lossyScale.y,
                dashDetectionCollider.transform.lossyScale.z
            );
            Gizmos.DrawWireSphere(center, radius);
        }
    }
}

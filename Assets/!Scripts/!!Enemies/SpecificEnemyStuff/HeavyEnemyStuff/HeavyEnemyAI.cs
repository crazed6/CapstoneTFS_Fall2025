using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using System.Collections;

public class HeavyEnemyAI : MonoBehaviour
{
    [Header("Player Detection")]
    public Transform player;

    [Header("Detection Zones")]
    public float detectionRange = 40f;   // Enemy sees player if inside this range -_-
    public float firingZoneRange = 50f;  // Enemy stops throwing Rocks for sins if player moves beyond this -_-

    [Header("Enemy Rotation Settings")]
    public float rotationSpeed = 5f;  // Speed of rotation -_-
    public Quaternion originalRotation;  // Store original rotation -_-


    [Header("Line of Sight Obstruction")]
    public LayerMask obstructionLayer;  // Objects that block enemy vision -_-

    [Header("Shooting Settings")]
    public Transform rockSpawnPoint;
    public GameObject rockPrefab;

    [Header("Shooting Timings")]
    public float trackDuration = 2f;  // Time to track player before locking Rock throw -_-
    public float lockDuration = 0.5f; // Time locked before throwing -_-
    public float postThrowCooldown = 1f; // Wait time before repeating -_-

    [Header("Rock Settings")]
    public float shootSpeed = 35.0f;

    [Header("Visual Trajectory")]
    public LineRenderer trajectoryLine;
    public Gradient normalColor;
    public Gradient lockedColor;

    [Header("Trajectory Settings")]
    public int trajectoryResolution = 30; // How many points in the curve -_-
    public float trajectoryHeight = 3f;  // Peak height of the arc -_-


    public HeavyEnemyStateMachine stateMachine;
    private bool isTracking = false;
    private Vector3 lockedTarget;

    //JK - removing healthbar stuff to avoid conflict with damage component

    [Header("Temp Damage DeBug")] // DELETE WHEN OTHER STUFF ADDED
    public float damageAmount = 10f;

    private void Start()
    {
        stateMachine = new HeavyEnemyStateMachine();
        stateMachine.ChangeState(new HeavyEnemyIdleState(this));  // Start in idle -_-
        originalRotation = transform.rotation;
    }

    private void Update()
    {
        stateMachine.Execute();

        //if (Input.GetMouseButtonDown(0)) // 0 means left mouse button  DELETE WHEN DAMAGE SYSTEM IMPLEMENTED
        //JK - THIS TO BE DELETED, ALMOST HAVE DAMAGE SYSTEM
        //{
        //    // Raycast from the camera to where the mouse is pointing
        //    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit hit;

        //    if (Physics.Raycast(ray, out hit))
        //    {
        //        // Check if the raycast hits this enemy
        //        if (hit.collider.CompareTag("Enemy"))
        //        {
        //            // Call TakeDamage() on the enemy that was clicked
        //            HeavyEnemyAI enemy = hit.collider.GetComponent<HeavyEnemyAI>();
        //            if (enemy != null)
        //            {
        //                enemy.TakeDamage((int)damageAmount);
        //            }
        //        }
        //    }
        //}
    }

    public bool CanSeePlayer()
    {
        if (player == null) return false;

        float distance = Vector3.Distance(transform.position, player.position);

        //  Player must be inside detection range -_-
        if (distance > firingZoneRange) return false;

        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        //  Player must be inside 180° vision cone -_-
        if (angle > 90f) return false;

        //  Performing multiple raycasts to ensure vision is blocked properly -_-
        Vector3 enemyEyeLevel = transform.position + Vector3.up * 1.5f;
        Vector3 playerCenter = player.position + Vector3.up * 1.0f;
        Vector3 playerHead = player.position + Vector3.up * 1.8f;
        Vector3 playerFeet = player.position + Vector3.up * 0.5f;

        // Performing multiple Linecasts (head, center, feet) -_-
        bool canSeeCenter = !Physics.Linecast(enemyEyeLevel, playerCenter, obstructionLayer);
        bool canSeeHead = !Physics.Linecast(enemyEyeLevel, playerHead, obstructionLayer);
        bool canSeeFeet = !Physics.Linecast(enemyEyeLevel, playerFeet, obstructionLayer);

        if (!canSeeCenter && !canSeeHead && !canSeeFeet)
        {
            Debug.DrawLine(enemyEyeLevel, playerCenter, Color.red, 0.1f);
            Debug.DrawLine(enemyEyeLevel, playerHead, Color.red, 0.1f);
            Debug.DrawLine(enemyEyeLevel, playerFeet, Color.red, 0.1f);
            return false; //  Only return false if ALL rays are blocked -_-
        }

        Debug.DrawLine(enemyEyeLevel, playerCenter, Color.green, 0.1f);
        Debug.DrawLine(enemyEyeLevel, playerHead, Color.green, 0.1f);
        Debug.DrawLine(enemyEyeLevel, playerFeet, Color.green, 0.1f);

        return true; //  At least one ray is clear, enemy still sees the player -_-
    }



    public void StartTracking()
    {
        if (!isTracking)
        {
            StartCoroutine(TrackAndThrowRock());
        }
    }

    private IEnumerator TrackAndThrowRock()
    {
        isTracking = true;
        float timer = 0f;

        while (timer < trackDuration)
        {
            if (player != null && CanSeePlayer())
            {
                //  Keep updating the locked target position until locked -_-
                lockedTarget = player.position;
                DrawTrajectory(lockedTarget, false);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        //  Stop updating target after locking
        trajectoryLine.colorGradient = lockedColor;
        yield return new WaitForSeconds(lockDuration);

        //  Throw at the locked position
        ThrowRockAtLockedPosition(lockedTarget);
        isTracking = false;

        yield return new WaitForSeconds(postThrowCooldown);

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


    public void StopTracking()
    {
        StopCoroutine(TrackAndThrowRock());  //  Stop the tracking coroutine -_-
        trajectoryLine.enabled = false;      //  Hide the trajectory -_-
        isTracking = false;                  //  Reset tracking flag -_-
    }



    private void DrawTrajectory(Vector3 target, bool isLocked)
    {
        trajectoryLine.enabled = true;
        trajectoryLine.positionCount = trajectoryResolution;

        //  Start from the spawn point
        Vector3 startPoint = rockSpawnPoint.position;

        //  Add height to the peak point for a better curve
        Vector3 midPoint = (startPoint + target) / 2 + Vector3.up * trajectoryHeight;

        //  End at the target point (aligned with player position)
        Vector3 endPoint = target;

        for (int i = 0; i < trajectoryResolution; i++)
        {
            float t = i / (float)(trajectoryResolution - 1);
            Vector3 point = QuadraticBezier(startPoint, midPoint, endPoint, t);
            trajectoryLine.SetPosition(i, point);
        }

        trajectoryLine.colorGradient = isLocked ? lockedColor : normalColor;
    }


    // Bezier curve formula for smooth arcs -_-
    private Vector3 QuadraticBezier(Vector3 start, Vector3 middle, Vector3 end, float t)
    {
        return (1 - t) * (1 - t) * start + 2 * (1 - t) * t * middle + t * t * end;
    }

    private void ThrowRockAtLockedPosition(Vector3 target)
    {
        trajectoryLine.enabled = false;  // Hide trajectory after throw -_-

        GameObject rock = Instantiate(rockPrefab, rockSpawnPoint.position, Quaternion.identity);
        HeavyEnemyRock rockScript = rock.GetComponent<HeavyEnemyRock>();

        rockScript.Launch(target, shootSpeed, trajectoryHeight);
    }

    //JK - removing health bar stuff to avoid conflict with damage component
    //public void TakeDamage(int damageAmount)
    //{
    //    currentHealth -= damageAmount;
    //    currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); //Update health bar -JK

    //    if (healthBar != null)
    //    {
    //        healthBar.SetHealth((float)currentHealth / maxHealth);
    //    }

    //    if (currentHealth <= 0)
    //    {
    //        Invoke(nameof(DestroyObject), 0.5f);
    //        Debug.Log(gameObject.name + " is dead!");
    //    }
    //}
}

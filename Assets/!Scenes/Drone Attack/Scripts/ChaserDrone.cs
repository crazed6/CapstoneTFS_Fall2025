using System.Collections;
using UnityEngine;

public class ChaserDrone : MonoBehaviour
{
    // Drone settings
    [SerializeField] private float activationRange = 15f;
    [SerializeField] private float baseSpeed = 5.5f; // Base speed for normal attack
    [SerializeField] private float maxSpeed = 14f; // Max speed when descending from high altitude
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float maxTurnSpeed = 2f;
    [SerializeField] private float escapeRange = 25f;
    [SerializeField] private float escapeTime = 15f;
    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionForce = 1000f;
    [SerializeField] private float selfDestructTime = 6f;
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private float riseSpeed = 3f;
    [SerializeField] private float hoverAmplitude = 1.2f;  // More pronounced shaking
    [SerializeField] private float hoverSpeed = 0.08f;      // Slower and wider ripple
    [SerializeField] private float hoverSmoothTime = 3.0f;  // Smoother transition

    [SerializeField] private float collisionAvoidanceForce = 10f;
    [SerializeField] private float bounceForce = 3f;
    private float ceilingHeight;
    private bool isRising = true;
    private float startY;
    private float hoverVelocity = 0f;
    private Vector3 velocity = Vector3.zero;
    private float lastSeenTime;
    private float playerStopTime = 0f;
    private bool isPlayerStopped = false;

    private Transform player;
    private Rigidbody rb;
    private bool isActivated = false;
    private bool isChasing = false;
    private float activationTime;
    private float currentSpeed;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        //Physics Settings**
        rb.isKinematic = false;  
        rb.useGravity = false;   
        rb.mass = 1f;           
        rb.linearDamping = 1.5f;          // Friction in air
        rb.angularDamping = 1.5f;   // Make the turns smooth
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

       
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic; // Best mode for fast objects
    }


    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer < 1.5f && !isChasing)
        {
            Explode();
        }

        DetectCeiling();




        if (isRising)
        {
            if (transform.position.y < ceilingHeight - 0.2f) // We added a small margin
            {
                transform.position += Vector3.up * riseSpeed * Time.deltaTime;
            }
            else
            {
                isRising = false; // Stop rising when reaching the ceiling
            }
        }


        if (!isChasing && !isRising)
        {
            float hoverOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverAmplitude;
            float targetHoverY = ceilingHeight - hoverOffset;
            float smoothHoverY = Mathf.SmoothDamp(transform.position.y, targetHoverY, ref hoverVelocity, hoverSmoothTime);

            transform.position = new Vector3(transform.position.x, smoothHoverY, transform.position.z);
        }




        
        float heightDifference = Mathf.Abs(transform.position.y - player.position.y);
        Vector3 playerDirection = player.position - transform.position;
        float angleToPlayer = Vector3.Dot(transform.forward, playerDirection.normalized);

        // If it is high, do not detect it (if it is more than 30 units high)
        if (heightDifference > 30f) return;

        //Detect only the player under the drone with SphereCast
        float adjustedDetectionRadius = Mathf.Clamp(heightDifference * 0.5f, 1.5f, 7f);
        RaycastHit hit;
        if (Physics.SphereCast(transform.position, adjustedDetectionRadius, Vector3.down, out hit, heightDifference * 1.2f)) 
        {
            if (hit.collider.CompareTag("Player"))
            {
                ActivateDrone();
            }
        }

        // Distance based detection according to height difference**
        float adjustedActivationRange = activationRange + (heightDifference * 1.5f);
        if (!isActivated && distanceToPlayer <= adjustedActivationRange && heightDifference < 15f)
        {
            ActivateDrone();
        }

        if (!isActivated && distanceToPlayer <= activationRange && angleToPlayer < 0)
        {
            ActivateDrone();
        }

        if (isChasing)
        {
            AdjustSpeed(distanceToPlayer);
            ChasePlayer();
            CheckPlayerMovement();
        }
    }



    void DetectCeiling()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.up, out hit, Mathf.Infinity))
        {
            ceilingHeight = hit.point.y - 0.5f;
        }
        else
        {
            ceilingHeight = transform.position.y + 5f;
        }
    }

    public void ActivateDrone()
    {
        if (!isActivated) // Prevent multiple activations
        {
            isActivated = true;
            isChasing = true;
            activationTime = Time.time;
            lastSeenTime = Time.time;
            StartCoroutine(SelfDestruct());
        }
    }

    void AdjustSpeed(float distanceToPlayer)
    {
        float heightDifference = transform.position.y - player.position.y;
        float speedFactor = Mathf.Clamp(heightDifference / 10f, 1f, maxSpeed / baseSpeed);
        currentSpeed = baseSpeed * speedFactor;
    }

    void ChasePlayer()
    {
        if (player != null)
        {
            Vector3 targetPosition = player.position;
            Vector3 direction = (targetPosition - transform.position).normalized;
            float step = acceleration * Time.deltaTime;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, maxTurnSpeed * Time.deltaTime);

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);
        }
    }

    void CheckPlayerMovement()
    {
        if (player != null)
        {
            float playerSpeed = player.GetComponent<Rigidbody>().linearVelocity.magnitude;
            if (playerSpeed < 0.1f)
            {
                if (!isPlayerStopped)
                {
                    isPlayerStopped = true;
                    playerStopTime = Time.time;
                }
                else if (Time.time - playerStopTime > 5.5f) // 1.5 seconds delay before explosion
                {
                    Explode();
                }
            }
            else
            {
                isPlayerStopped = false;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Drone"))
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 avoidanceDirection = transform.position - collision.transform.position;
                rb.AddForce(avoidanceDirection.normalized * collisionAvoidanceForce, ForceMode.Impulse);
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Explode(); 
        }

    }



    IEnumerator SelfDestruct()
    {
        yield return new WaitForSeconds(selfDestructTime);
        Explode();
    }

    void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

       
        rb.isKinematic = true;
        rb.detectCollisions = false;

        Destroy(gameObject, 0.1f); // destroy
    }


}
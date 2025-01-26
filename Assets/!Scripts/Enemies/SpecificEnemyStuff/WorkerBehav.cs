using UnityEngine;
using UnityEngine.AI;

public class WorkerBehav : MonoBehaviour
{

    //STATE VARIABLES
    public float moveSpeed = 3f;
    public int damage = 1;
    private bool isIdle = true;


    //REFERENCES
    public Transform player;
    private NavMeshAgent agent;
    public float detectionRange = 10f;
    public float attackRange = 2f;
    //private Animator animator; **once we have an Idle animation


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        //animator = GetComponent<Animator>();

        if (agent != null)
        {
            agent.speed = moveSpeed;
        }
    }


    private void Update()
    {
        if (player != null)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                isIdle = false;
            }
            else
            {
                isIdle = true;
            }

            if (!isIdle)
            {
                Chase();

                //Check if within attack range
                if (distanceToPlayer <= attackRange)
                {
                    Attack();
                }
            }
        }
        //if(animator != null)
        //{
        //    animator.SetBool("isIdle", isIdle);
        //    animator.SetBool("isChasing", !isIdle);
        //}
    }


    //Follow player transform
    private void Chase()
    {
        if (agent != null)
        {
            agent.SetDestination(player.position);
        }
    }

    //Attack on collision

    private void Attack()
    {
        Debug.Log($"{gameObject.name} is attacking the player!");

        //Implement attack logic here (need player health/health manager)
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isIdle && collision.gameObject.CompareTag("Player"))
        {
            Debug.Log($"{gameObject.name} has collided with the player!");
            // Add collision-based attack logic here if needed
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (player != null)
        {
            //Detection range (green)
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectionRange);

            //Attack range (red)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }
    }

}

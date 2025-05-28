using System;
using UnityEngine;

public class PlayerAttackComponent : MonoBehaviour
{

    [Header("References")]
    public Rigidbody playerRB;
    //public Animator anim; //Uncomment once we have animations for attack

    [Header("Attack Cooldown Settings")]
    public float dashCooldown = 1.0f;
    public float javelinCooldown = 1.5f;

    [Header("Attack Settings")]
    public float attackRange = 3.0f; //need to test this
    public LayerMask enemyLayer; //Layer mask to filter for enemies

    private float lastDashTime = -Mathf.Infinity;
    private float lastJavelinTime = -Mathf.Infinity;


    private void Start()
    {
        if (playerRB == null)
        {
            playerRB = GetComponent<Rigidbody>();
            if (playerRB == null)
            {
                Debug.LogError("Player Rigidbody not found. Please assign it in the inspector.");
            }
        }

        //if (anim == null)
        //{
        //    anim = GetComponent<Animator>();
        //    if (anim == null)
        //    {
        //        Debug.LogError("Player Animator not found. Please assign it in the inspector.");
        //    }

    }

    private void Update()
    {
        
        //Right click: Dash Attack
        if (Input.GetMouseButtonDown(1) && Time.deltaTime >= lastDashTime + dashCooldown)
        {
            EnemyDamageComponent enemy = RaycastForEnemy();
            
            if(enemy != null)
            { 
            float speed = playerRB != null ? playerRB.linearVelocity.magnitude : 0f;
            float dashDamage = DamageCalculator.CalculateDamage(speed);
            enemy.TakeDamage(dashDamage, gameObject);
            Debug.Log($"[ATTACK] Applied dash damage: {dashDamage}");

            //if (anim != null)
            //    anim.SetTrigger("DashAttack"); //Trigger dash attack animation

            lastDashTime = Time.deltaTime; //Reset cooldown timer
            }
        }

        //Left click: Javelin Throw
        if (Input.GetKeyDown(0) && Time.deltaTime >= lastJavelinTime + javelinCooldown)
        {
            EnemyDamageComponent enemy = RaycastForEnemy();

            if(enemy != null)
            { 
            float javelinDamage = DamageCalculator.GetJavelinDamage();
            enemy.TakeDamage(javelinDamage, gameObject);
            Debug.Log($"[ATTACK] Applied javelin damage: {javelinDamage}");

            //if (anim != null)
            //    anim.SetTrigger("JavelinThrow"); //Trigger javelin throw animation

            lastJavelinTime = Time.deltaTime; //Reset cooldown timer
            }
        }
    }

    private EnemyDamageComponent RaycastForEnemy()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 1f, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange, enemyLayer))
        {
            return hit.collider.GetComponent<EnemyDamageComponent>();
        }

        Debug.DrawRay(transform.position + Vector3.up, transform.forward * attackRange, Color.cyan);

        return null;
    }
}

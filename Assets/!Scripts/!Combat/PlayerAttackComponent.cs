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
    public float attackRange = 10.0f; //need to test this
    public LayerMask Enemy; //Layer mask to filter for enemies

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
        if (Input.GetMouseButtonDown(0) && Time.time >= lastDashTime + dashCooldown) //0 means left mouse button
        {       
            EnemyDamageComponent enemy = RaycastForEnemy();
            
            if(enemy != null)
            { 
            float speed = playerRB != null ? playerRB.linearVelocity.magnitude : 0f;
            float dashDamage = DamageCalculator.CalculateDamage(speed);
            enemy.TakeDamage(dashDamage, gameObject);
            Debug.LogError($"[ATTACK] Applied dash damage: {dashDamage}");

            //if (anim != null)
            //    anim.SetTrigger("DashAttack"); //Trigger dash attack animation

            lastDashTime = Time.time; //Reset cooldown timer
            }
        }

        //Left click: Javelin Throw
        if (Input.GetMouseButtonDown(1) && Time.time >= lastJavelinTime + javelinCooldown) //1 means right mouse button
        {
            EnemyDamageComponent enemy = RaycastForEnemy();

            if(enemy != null)
            { 
            float javelinDamage = DamageCalculator.GetJavelinDamage();
            enemy.TakeDamage(javelinDamage, gameObject);
            Debug.LogError($"[ATTACK] Applied javelin damage: {javelinDamage}");

            //if (anim != null)
            //    anim.SetTrigger("JavelinThrow"); //Trigger javelin throw animation

            lastJavelinTime = Time.time; //Reset cooldown timer
            }
        }
    }

    private EnemyDamageComponent RaycastForEnemy()
    {
        Debug.DrawRay(transform.position + Vector3.up, transform.forward * attackRange, Color.cyan, 2.0f);

        Ray ray = new Ray(transform.position + Vector3.up * 1f, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackRange, Enemy))
        {
            
            return hit.collider.GetComponentInParent<EnemyDamageComponent>();
        }

        return null;
    }
}

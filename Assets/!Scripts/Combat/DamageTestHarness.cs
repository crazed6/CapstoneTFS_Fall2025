using UnityEngine;

public class DamageTestHarness : MonoBehaviour
{

    [Tooltip("Enemy to test damage on (assign in Inspector or will auto-find)")]
    public EnemyDamageComponent targetEnemy;

    [Tooltip("Assign Rigidbody to calculate speed-based damage")]
    public Rigidbody playerRB;

    private void Start()
    {
        //Auto-find enemy if not assigned
        if (targetEnemy == null)
        {
            targetEnemy = FindFirstObjectByType<EnemyDamageComponent>();
            if (targetEnemy == null)
                Debug.LogWarning("No EnemyDamageComponent found in the scene");
        }

        //Auto-find Rigidbody if not assigned
        if (playerRB == null)
        {
            playerRB = FindFirstObjectByType<Rigidbody>();
            if (playerRB == null)
                Debug.LogWarning("No Rigidbody found in the scene");
        }
    }

    private void Update()
    {
        if (targetEnemy == null) return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            float speed = playerRB != null ? playerRB.linearVelocity.magnitude : 0f;
            float dashDamage = DamageCalculator.CalculateDamage(speed);
            targetEnemy.TakeDamage(dashDamage, gameObject);
            Debug.Log($"[TEST] Applied dash damage: {dashDamage}");
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            float javelinDamage = DamageCalculator.GetJavelinDamage();
            targetEnemy.TakeDamage(javelinDamage, gameObject);
            Debug.Log($"[TEST] Applied javelin damage: {javelinDamage}");
        }
    }



}

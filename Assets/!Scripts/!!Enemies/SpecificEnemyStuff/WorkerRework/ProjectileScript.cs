using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float speed = 25f;         // How fast the projectile moves
    public float lifeTime = 5f;       // How long before the projectile disappears automatically
    public int damage = 1;            // Amount of damage this projectile does (not applied yet)

    private Vector3 direction;        // The direction the projectile will move in

    //Declared Variable
    //Josh testing
    public DamageProfile WorkerProjectile; // Reference to the damage profile for explosion damage
    //Josh testing end

    // This method is called when the projectile is first spawned
    public void Initialize(Vector3 shootDirection)
    {
        direction = shootDirection.normalized;  // Set and normalize the direction to ensure consistent speed
        Destroy(gameObject, lifeTime);          // Destroy the projectile after 'lifeTime' seconds
    }

    void Update()
    {
        // Move the projectile forward each frame based on speed and time
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        // If the projectile hits the player
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player hit!");
            // Here you can add logic to apply damage to the player (e.g., other.GetComponent<PlayerHealth>().TakeDamage(damage);)

            //Josh script, ensure to attach Damage Profile in inspector, on Projectile script
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null && WorkerProjectile != null)
            {
                DamageData damageData = new DamageData(gameObject, WorkerProjectile);
                playerHealth.TakeDamage(damageData);
            }
            //Josh script end

            Destroy(gameObject); // Remove the projectile after hitting
        }
        // If the projectile hits anything else solid (like a wall or ground), and it's not a trigger
        else if (!other.isTrigger)
        {
            Destroy(gameObject); // Destroy the projectile on impact
        }
    }
}

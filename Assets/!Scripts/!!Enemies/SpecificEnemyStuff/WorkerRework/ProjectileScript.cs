using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float speed = 25f;
    public float lifeTime = 5f;
    public int damage = 1;
    public static event System.Action<ProjectileScript, Collider> OnAnyProjectileHit;

    private Vector3 direction;

    public DamageProfile WorkerProjectile;

    public WorkerAudio ownerAudio;

    public void Initialize(Vector3 shootDirection)
    {
        direction = shootDirection.normalized;
        transform.forward = direction;

        // ✅ Sideways tilt (adjust angle if your model faces wrong way)
        transform.Rotate(90f, 0f, 0f);

        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Damage logic stays the same
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null && WorkerProjectile != null)
            {
                DamageData damageData = new DamageData(gameObject, WorkerProjectile);
                playerHealth.PlayerTakeDamage(damageData);
            }

            // Play the hit sound on the Worker that fired this projectile
            ownerAudio?.PlayImpact();

            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }

}

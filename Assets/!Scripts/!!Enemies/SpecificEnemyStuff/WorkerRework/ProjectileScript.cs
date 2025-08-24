using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public float speed = 25f;
    public float lifeTime = 5f;
    public int damage = 1;

    private Vector3 direction;

    public DamageProfile WorkerProjectile;

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
            Debug.Log("Player hit!");
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null && WorkerProjectile != null)
            {
                DamageData damageData = new DamageData(gameObject, WorkerProjectile);
                playerHealth.PlayerTakeDamage(damageData);
            }
            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}

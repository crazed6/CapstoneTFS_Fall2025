using UnityEngine;

public class DamageReceive : MonoBehaviour
{
    //private Health health;
    //public float rockDamage = 50f; // Example damage value for a rock
    //public float heavyMeleeDamage = 70f; // Example damage value for a heavy melee attack
    //public float minorExplosionDamage = 20f;
    //public float mediumExplosionDamage = 50f;
    //public float majorExplosionDamage = 100f;


    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    //private void Awake()
    //{
    //    {
    //        health = GetComponent<Health>();
    //        if (health == null)
    //            Debug.LogError("No Health component found on DamageReceiver's GameObject.");
    //    }
    //}

    //public void ReceiveDamage(DamageData data)
    //{
    //    if (health != null)
    //    {
    //        Debug.Log($"Received damage: {data.amount} from {data.source.name} of type {data.type}");
    //        health.TakeDamage(data);
    //    }
    //}

    //void OnCollisionEnter(Collision collision)
    //{
    //    GameObject other = collision.gameObject;

    //    if (other.CompareTag("Rock(EnemyProjectile)"))
    //    {
    //        Debug.LogWarning("Rock hit the player!");
    //        ReceiveDamage(new DamageData(other.gameObject, DamageType.Melee, (int)rockDamage));
    //    }
    //    if (other.CompareTag("HeavyMelee"))
    //    {
    //        Debug.LogWarning("Heavy melee attack hit the player!");
    //        ReceiveDamage(new DamageData(other.gameObject, DamageType.Melee, (int)heavyMeleeDamage));
    //    }
    //    if (other.CompareTag("MinorExplosion"))
    //    {
    //        Debug.LogWarning("Minor explosion hit the player!");
    //        ReceiveDamage(new DamageData(other.gameObject, DamageType.Explosion, (int)minorExplosionDamage));
    //    }
    //    if (other.CompareTag("MediumExplosion"))
    //    {
    //        Debug.LogWarning("Medium explosion hit the player!");
    //        ReceiveDamage(new DamageData(other.gameObject, DamageType.Explosion, (int)mediumExplosionDamage));
    //    }
    //    if (other.CompareTag("MajorExplosion"))
    //    {
    //        Debug.LogWarning("Major explosion hit the player!");
    //        ReceiveDamage(new DamageData(other.gameObject, DamageType.Explosion, (int)majorExplosionDamage));
    //    }
    //}
}

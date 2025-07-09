using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    //Josh testing
    public DamageProfile Trap; // Reference to the damage profile for explosion damage
    //Josh testing end

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has died! (Spike Trap Triggered)");

            //Spike Trap Josh health logic
            //Josh script, ensure to attach Damage Profile in inspector, on Trap script
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null && Trap != null)
            {
                DamageData damageData = new DamageData(gameObject, Trap);
                playerHealth.TakeDamage(damageData);
            }
            //Josh script end

        }
    }
}

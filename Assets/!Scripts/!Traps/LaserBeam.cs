using UnityEngine;
using UnityEngine.UIElements;

public class LaserBeam : MonoBehaviour
{
    //Josh testing
    public DamageProfile LaserTrap; // Reference to the damage profile for explosion damage
    //Josh testing end

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has died! (Laser Grid Triggered)");

            //Laser Trap Josh health logic
            //Josh script, ensure to attach Damage Profile in inspector, on Laser Trap script
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null && LaserTrap != null)
            {
                DamageData damageData = new DamageData(gameObject, LaserTrap);
                playerHealth.TakeDamage(damageData);
            }
            //Josh script end
        }
    }
}

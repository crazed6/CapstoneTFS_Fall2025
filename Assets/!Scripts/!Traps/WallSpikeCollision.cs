using UnityEngine;

public class WallSpikeCollision : MonoBehaviour
{
    //Josh attachment
    //Josh testing
    public DamageProfile SpikeTrap; // Reference to the damage profile for explosion damage
    //Josh testing end
    //Josh ends here
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Josh script
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null && SpikeTrap != null)
            {
                DamageData damageData = new DamageData(gameObject, SpikeTrap);
                playerHealth.PlayerTakeDamage(damageData);
            }
            //Josh ends here

            Debug.Log("Hit the Player!");
        }
    }
}

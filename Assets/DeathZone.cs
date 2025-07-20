using UnityEngine;

public class DeathZone : MonoBehaviour
{

    //Josh attachment
    //Josh testing
    public DamageProfile DeathZoneProfile; // Reference to the damage profile for explosion damage
    //Josh testing end
    //Josh ends here

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Player"))
        {
            //Josh script
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null && DeathZoneProfile != null)
            {
                DamageData damageData = new DamageData(gameObject, DeathZoneProfile);
                playerHealth.PlayerTakeDamage(damageData);
            }
            //Josh ends here

            Debug.Log("Hit the Player!");
        }
    }
}

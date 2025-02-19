using UnityEngine;
using System.Collections; // Required for Coroutines

public class SpikeWall : MonoBehaviour
{
    [SerializeField] private int damageAmount = 10; // Set damage in Inspector
    private bool canDamage = true; // Controls damage cooldown

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && canDamage) // Ensure it's the player & can take damage
        {
            Health playerHealth = other.GetComponent<Health>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
                Debug.Log("Player took " + damageAmount + " damage from SpikeWall!");
                //StartCoroutine(DamageCooldown()); // Start cooldown
            }
        }
    }

    ////private ienumerator damagecooldown()
    ////{
    ////    candamage = false; // disable damage
    ////    yield return new waitforseconds(3f); // wait for 5 seconds
    ////    candamage = true; // re-enable damage
    ////}
}

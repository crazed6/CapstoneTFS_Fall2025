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
                StartCoroutine(DamageCooldown()); // Start cooldown
            }
        }
    }

    private IEnumerator DamageCooldown()
    {
        canDamage = false; // Disable damage
        yield return new WaitForSeconds(3f); // Wait for 5 seconds
        canDamage = true; // Re-enable damage
    }
}

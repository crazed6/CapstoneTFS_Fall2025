using UnityEngine;
using UnityEngine.UI;

public class NotPlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public Slider healthSlider;

    void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    void Update()
    {
        // Check for reactivation input
        if (Input.GetKeyDown(KeyCode.V))
        {
            ReactivateCharacter();
        }

        // Check for damage input
        if (Input.GetKeyDown(KeyCode.B))
        {
            TakeDamage(10f); // Deal 10 damage when B is pressed
            Debug.Log("B pressed: Damaging character for 10 health.");
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthSlider.value = currentHealth;

        // Check if health is 0 or below and deactivate the character
        if (currentHealth <= 0)
        {
            Debug.Log("Character has died. Deactivating.");
            gameObject.SetActive(false); // Deactivate the character
        }
    }

    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthSlider.value = currentHealth;
    }

    private void ReactivateCharacter()
    {
        // Reactivate the character and restore health
        gameObject.SetActive(true);
        Heal(10f); // Restore 10 health
        Debug.Log("Character reactivated and healed for 10 health.");
    }

    // Added method to get current health
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}

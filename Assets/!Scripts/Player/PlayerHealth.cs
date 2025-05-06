using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{

    public HealthBar healthBar;
    public float maxHealth = 100;
    public float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    //For debugging purposes to Manually take damage or heal with keypress
    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Z))
        //{
        //    TakeDamage(25);
        //}

        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    Heal(25f);
        //}

        //Listen for C, V, B keypress
        if (Input.GetKeyDown(KeyCode.C))
        {
            TakeDamage(10f);
            Debug.Log("C press deals 10 damage");
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            Heal(10f);
            Debug.Log("V press regenerates 10 health");
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            TakeDamage(5f);
            Debug.Log("B press deals 5 damage");

        }
    }

    //Function to deal damage, trigger death logic, update UI, includes debug log
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log($"Player took {damage} damage. Current health: {currentHealth}");
        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //RIP
    private void Die()
    {
        Debug.Log("Player has died.");
        // Add player death logic here for respawn or game-over
    }

    //Increases health and updates UI
    public void Heal(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        Debug.Log($"Player healed {amount} health. Current health: {currentHealth}");
    }

    //Update UI according to damage and heal
    private void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            float normalizedHealth = currentHealth / maxHealth;
            healthBar.SetHealth(normalizedHealth);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    private float currentHealth = 1f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) // Decrease health
        {
            TakeDamage(0.25f);
        }

        if (Input.GetKeyDown(KeyCode.X)) // Increase health
        {
            Heal(0.25f);
        }
    }

    // Function for taking damage
    public void TakeDamage(float damage)
    {
        ModifyHealth(-damage);
    }

    // Function for healing
    public void Heal(float healing)
    {
        ModifyHealth(healing);
    }

    // General function to modify health
    private void ModifyHealth(float amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, 1f); // Ensure health stays between 0 and 1
        healthSlider.value = currentHealth; // Update the slider value
    }
}

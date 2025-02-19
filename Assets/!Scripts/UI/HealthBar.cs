using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Image fillImage;
    [Tooltip("Speed at which the health bar interpolates toward the target value.")]
    public float smoothingSpeed = 5f;
    private float targetHealth = 1f;
    public Gradient healthColorGradient;

   
    private void Start()
    {
        if (healthSlider == null)
        {
            Debug.LogError("HealthBar: healthSlider is not assigned!");
        }
        if (fillImage == null)
        {
            Debug.LogError("HealthBar: fillImage is not assigned!");
        }
        targetHealth = healthSlider != null ? healthSlider.value : 1f;

    }

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
        
        transform.LookAt(Camera.main.transform);
        
        healthSlider.value = Mathf.Lerp(healthSlider.value, targetHealth, smoothingSpeed * Time.deltaTime);
        
        //Update fill image color based on the slider value using the gradient
        if (fillImage != null && healthColorGradient != null)
        {
            fillImage.color = healthColorGradient.Evaluate(healthSlider.value);
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
        targetHealth += amount;
        targetHealth = Mathf.Clamp(targetHealth, 0f, 1f); // Ensure health stays between 0 and 1
        healthSlider.value = targetHealth; // Update the slider value
    }

    //Set the slide value based on a normalized value (0 to 1)
    public void SetHealth(float normalizedHealth)
    {
        targetHealth = Mathf.Clamp01(normalizedHealth);
    }
}

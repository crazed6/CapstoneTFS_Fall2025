using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Slider healthSlider; // Reference to the UI Slider component
    public Health playerHealth; // Reference to the player's Health component
    public Image fillImage; // Reference to the fill image of the slider
    public Gradient healthGradient; // Gradient for the fill color based on health

    private Coroutine warningCoroutine; // Coroutine for warning about low health
    private bool isWarningActive = false; // Flag to check if warning is active

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerHealth != null && healthSlider != null)
        {
            healthSlider.maxValue = playerHealth.MaxHealth; // Set the maximum value of the slider
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (playerHealth == null || healthSlider == null)
        {
            Debug.LogWarning("Player Health or Health Slider is not assigned.");
            return;
        }

        SetHealth(playerHealth.health, playerHealth.MaxHealth);
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        //Updating the slider value
        healthSlider.value = currentHealth;

        float healthPercentage = (float)currentHealth / maxHealth;

        //Update color based on health percentage
        if (fillImage != null && healthGradient != null)
        {
            fillImage.color = healthGradient.Evaluate(healthPercentage);
        }

        //Low health warning, under 20% of max health
        if (healthPercentage < 0.2f && !isWarningActive)
        {
            isWarningActive = true;
            if (warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);
            }
            warningCoroutine = StartCoroutine(BlinkRedEffect());
        }
        else if (healthPercentage >= 0.2f && isWarningActive)
        {
            isWarningActive = false;
            if (warningCoroutine != null)
            {
                StopCoroutine(warningCoroutine);
                warningCoroutine = null;
            }
        }
    }

    private IEnumerator BlinkRedEffect()
    {
        while (isWarningActive)
        {
            if (fillImage != null)
            {
                fillImage.color = Color.red; // Set to red
            }
            yield return new WaitForSeconds(0.3f);
            if (fillImage != null)
            {
                fillImage.color = healthGradient.Evaluate((float)playerHealth.health / playerHealth.MaxHealth); // Reset to gradient color
            }
            yield return new WaitForSeconds(0.3f);
        }
    }
}

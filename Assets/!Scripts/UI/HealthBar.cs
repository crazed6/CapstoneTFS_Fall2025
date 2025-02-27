using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Image fillImage;
    public Gradient healthColorGradient;

   //Directly sets health value without interpolation
   public void SetHealth (float normalizedHealth)
    {
        normalizedHealth = Mathf.Clamp01(normalizedHealth);

        if (healthSlider != null)
        {
            healthSlider.value = normalizedHealth;
        }

        if (fillImage != null && healthColorGradient != null)
        {
            fillImage.color = healthColorGradient.Evaluate(normalizedHealth);
        }
    }

    private void Update()
    {
        if (Camera.main != null)
        {
            transform.LookAt(Camera.main.transform);
        }
    }

}

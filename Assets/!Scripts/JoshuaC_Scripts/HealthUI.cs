using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Joshuas and Diego's Script

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    public RectTransform maskTransform; // The transform of the mask
    public Image fillImage;             // The fill image (can be used to set color)
    public Health playerHealth;

    [Header("Visuals")]
    public Gradient healthGradient;

    private float originalWidth;
    private Coroutine warningCoroutine;
    private bool isWarningActive = false;

    void Start()
    {
        if (maskTransform != null)
        {
            originalWidth = maskTransform.sizeDelta.x;
        }

        if (playerHealth != null)
        {
            SetHealth(playerHealth.health, playerHealth.MaxHealth);
        }
    }

    void Update()
    {
        if (playerHealth != null)
        {
            SetHealth(playerHealth.health, playerHealth.MaxHealth);
        }
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        // Clamp health percentage between 0 and 1
        float healthPercent = Mathf.Clamp01((float)currentHealth / maxHealth);

        // Resize the mask width to reflect current health (left-to-right)
        if (maskTransform != null)
        {
            // Multiply original full width by health percentage
            maskTransform.sizeDelta = new Vector2(originalWidth * healthPercent, maskTransform.sizeDelta.y);
        }

        // Update the fill image color if no warning is active
        if (fillImage != null && !isWarningActive)
        {
            fillImage.color = healthGradient.Evaluate(healthPercent);
        }

        // Low health warning logic (below 20%)
        if (healthPercent < 0.2f && !isWarningActive)
        {
            isWarningActive = true;
            if (warningCoroutine != null)
                StopCoroutine(warningCoroutine);
            warningCoroutine = StartCoroutine(BlinkRedEffect());
        }
        else if (healthPercent >= 0.2f && isWarningActive)
        {
            isWarningActive = false;
            if (warningCoroutine != null)
                StopCoroutine(warningCoroutine);
            warningCoroutine = null;

            // Restore fill color immediately after warning stops
            if (fillImage != null)
                fillImage.color = healthGradient.Evaluate(healthPercent);
        }
    }


    private IEnumerator BlinkRedEffect()
    {
        while (isWarningActive)
        {
            if (fillImage != null)
                fillImage.color = Color.red;

            yield return new WaitForSeconds(0.3f);

            if (fillImage != null)
                fillImage.color = healthGradient.Evaluate((float)playerHealth.health / playerHealth.MaxHealth);

            yield return new WaitForSeconds(0.3f);
        }
    }
}

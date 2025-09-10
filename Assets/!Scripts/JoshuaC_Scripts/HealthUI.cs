using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

public class HealthUI : MonoBehaviour
{
    [Header("References")]
    public RectTransform maskTransform; // The moving mask
    public Image[] slots;               // 10 slot images inside the mask
    public Health playerHealth;

    [Header("Visuals")]
    public Color maxColor = Color.green;
    public Color minColor = Color.red;
    public Color flashColorA = Color.red;
    public Color flashColorB = Color.white;

    private float originalWidth;
    private Coroutine flashCoroutine;
    private bool isFlashing = false;

    void Start()
    {
        if (maskTransform != null)
            originalWidth = maskTransform.sizeDelta.x;

        if (playerHealth != null)
            SetHealth(playerHealth.health, playerHealth.MaxHealth);

        // Initialize slot colors
        foreach (var slot in slots)
        {
            if (slot != null)
                slot.color = maxColor;
        }
    }

    void Update()
    {
        if (playerHealth != null)
            SetHealth(playerHealth.health, playerHealth.MaxHealth);
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        float healthPercent = Mathf.Clamp01((float)currentHealth / maxHealth);

        // Resize mask based on health %
        if (maskTransform != null)
        {
            float targetWidth = originalWidth * healthPercent;
            maskTransform.sizeDelta = new Vector2(targetWidth, maskTransform.sizeDelta.y);
        }

        // Base gradient color
        Color targetColor = Color.Lerp(minColor, maxColor, healthPercent);

        // Update slots if not flashing
        if (!isFlashing)
        {
            foreach (var slot in slots)
            {
                if (slot != null)
                    slot.DOColor(targetColor, 0.15f);
            }
        }

        // Handle flashing
        if (healthPercent <= 0.25f && !isFlashing)
        {
            isFlashing = true;
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRedWhite());
        }
        else if (healthPercent > 0.25f && isFlashing)
        {
            isFlashing = false;
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = null;

            // Reset to correct color
            foreach (var slot in slots)
            {
                if (slot != null)
                    slot.color = targetColor;
            }
        }
    }

    private IEnumerator FlashRedWhite()
    {
        while (isFlashing)
        {
            foreach (var slot in slots)
                if (slot != null)
                    slot.color = flashColorA; // red

            yield return new WaitForSeconds(0.25f);

            foreach (var slot in slots)
                if (slot != null)
                    slot.color = flashColorB; // white

            yield return new WaitForSeconds(0.25f);
        }
    }
}

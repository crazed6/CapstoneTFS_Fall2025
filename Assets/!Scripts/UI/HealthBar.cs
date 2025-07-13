
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Image fillImage; //Assign this to the Fill Area in the inspector
    public Gradient healthColorGradient;
    private Coroutine warningCoroutine;
    private bool isWarning = false; //flag to prevent redundant coroutine calls



    private void Start()
    {
        SetHealth(100f);
    }
    private void Update()
    {
        FaceCamera();
    }

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

       //Current threshold 20% of total health
        if (normalizedHealth < 0.35f)
        {
            if (!isWarning)
            {
                isWarning = true;
                warningCoroutine = StartCoroutine(BlinkRedEffect());
            }
        }
        else
        {
            if (isWarning)
            {
                isWarning = false;
                StopCoroutine(warningCoroutine);
                warningCoroutine = null;
                fillImage.color = healthColorGradient.Evaluate(normalizedHealth);
            }
        }
    }

    // Makes the health bar face the camera
    private void FaceCamera()
    {
        if (Camera.main == null) return;

        Vector3 direction = Camera.main.transform.position - transform.position;
        direction.y = 0f; //Lock vertical rotation
        transform.rotation = Quaternion.LookRotation(-direction);
    }

    //Health bar blinks red when low health
    private IEnumerator BlinkRedEffect()
    {
        while (true)
        {
            fillImage.color = Color.red;
            yield return new WaitForSeconds(0.3f);
            fillImage.color = Color.grey;
            yield return new WaitForSeconds(0.3f);
        }
    }

}

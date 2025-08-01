using UnityEngine;
using UnityEngine.UI; // Importing the UI namespace to use Image component
using System.Collections; // Importing System.Collections for IEnumerator

public class FadetoBlack : MonoBehaviour
{
    public Image fadeImage; // Reference to the Image component used for fading
    public float fadeDuration = 1.5f; // Duration of the fade effect in seconds

    private bool isFading = false; // Flag to check if a fade is already in progress

    public void TriggerFadeToBlack(System.Action onFadeComplete = null)
    {
        if (!isFading && fadeImage != null)
        {
            StartCoroutine(FadeToBlackCoroutine(onFadeComplete)); // Start the coroutine to handle the fade effect
        }
    }

    private IEnumerator FadeToBlackCoroutine(System.Action onFadeComplete)
    {
        isFading = true; // Set the flag to indicate that a fade is in progress

        float elapsed = 0f; // Initialize a timer to track the fade duration
        Color color = fadeImage.color; // Get the current color of the fade image
        color.a = 0f;
        fadeImage.color = color;
        fadeImage.gameObject.SetActive(true); // Make sure the image is enabled

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaled time for fade
            color.a = Mathf.Clamp01(elapsed / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        color.a = 1f;
        fadeImage.color = color;

        Debug.Log("Fade completed.");
        onFadeComplete?.Invoke();

        isFading = false;
    }
}

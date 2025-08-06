using UnityEngine;
using UnityEngine.UI; // Importing the UI namespace to use Image component
using System.Collections; // Importing System.Collections for IEnumerator

public class FadetoBlack : MonoBehaviour
{
    public Image fadeImage; // Reference to the Image component used for fading
    public float fadeDuration = 1.5f; // Duration of the fade effect in seconds

    private bool isFading = false; // Flag to check if a fade is already in progress

    public GameObject gameOverPanel; // Reference to the CheckpointSystem


    private void Awake()
    {
        if (fadeImage != null)
        {
            Color color = fadeImage.color;
            color.a = 0f; // Start fully transparent
            fadeImage.color = color;
            //fadeImage.gameObject.SetActive(false); // Ensure the image is disabled at start
        }
    }

    public void StartFadeToBlack()
    {
        if (!isFading && fadeImage != null)
        {
            StartCoroutine(FadeToBlackCoroutine());
        }
    }

    private IEnumerator FadeToBlackCoroutine()
    {
        isFading = true;
        // Ensure the fade image starts transparent
        Color fadeColor = fadeImage.color;
        fadeColor.a = 0f;
        fadeImage.color = fadeColor;
        fadeImage.gameObject.SetActive(true);

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // This works even when timeScale = 0
            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);

            fadeColor.a = alpha;
            fadeImage.color = fadeColor;

            yield return null;
        }

        // Ensure fully black
        fadeColor.a = 1f;
        fadeImage.color = fadeColor;

        // Show game over panel
        gameOverPanel.SetActive(true);
        //checkpointSystem.ShowGameOverPanel();
    }
}

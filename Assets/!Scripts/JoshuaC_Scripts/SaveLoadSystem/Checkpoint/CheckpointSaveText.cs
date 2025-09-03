using UnityEngine;
using TMPro;
using System.Collections;

public class CheckpointSaveText : MonoBehaviour
{
    public TextMeshProUGUI checkpointText;  // assign in inspector
    public float displayTime = 2f;          // how long text stays visible
    public float fadeDuration = 1f;         // how long to fade out

    private Coroutine currentRoutine;

    public void ShowCheckpointSaved()
    {
        // stop any existing fade if already showing
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }
        currentRoutine = StartCoroutine(ShowAndFade());
    }

    private IEnumerator ShowAndFade()
    {
        // Set text visible
        checkpointText.text = "Checkpoint Saved!";
        checkpointText.alpha = 1f;

        // Wait while fully visible
        yield return new WaitForSeconds(displayTime);

        // Fade out
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / fadeDuration);
            checkpointText.alpha = alpha;
            yield return null;
        }

        checkpointText.text = "";
        currentRoutine = null;
    }
}

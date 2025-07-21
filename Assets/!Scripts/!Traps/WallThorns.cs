using System.Collections;
using UnityEngine;

public class WallThorns : MonoBehaviour
{
    [Header("Spike Settings")]
    public GameObject spikePrefab;
    public int numberOfSpikes = 5;
    public float verticalSpacing = 0.5f;
    public Vector3 spikeScale = new Vector3(0.2f, 2f, 0.2f);

    [Header("Timing")]
    public float activeDuration = 2.5f;
    public float inactiveDuration = 2.5f;
    public float animationDuration = 0.5f;

    private GameObject[] spikes;
    private Vector3 _protrudedScale;
    private Vector3 _retractedScale;

    void Start()
    {
        spikes = new GameObject[numberOfSpikes];

        _protrudedScale = spikeScale;
        _retractedScale = new Vector3(spikeScale.x, 0f, spikeScale.z);

        for (int i = 0; i < numberOfSpikes; i++)
        {
            // The offset starts at zero and increases for each spike,
            // placing them all on one side of the pivot point.
            Vector3 offset = new Vector3(0, i * verticalSpacing, 0);
            Vector3 spawnPos = transform.position + offset;

            GameObject spike = Instantiate(spikePrefab, spawnPos, Quaternion.identity, transform);
            spike.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
            spike.transform.localScale = _retractedScale;

            if (spike.GetComponent<WallSpikeCollision>() == null)
                spike.AddComponent<WallSpikeCollision>();

            spikes[i] = spike;
        }

        StartCoroutine(ThornCycle());
    }

    // --- The rest of the script (ThornCycle and AnimateSpikes) is unchanged ---

    IEnumerator ThornCycle()
    {
        while (true)
        {
            yield return StartCoroutine(AnimateSpikes(_retractedScale, _protrudedScale, animationDuration));
            yield return new WaitForSeconds(activeDuration);
            yield return StartCoroutine(AnimateSpikes(_protrudedScale, _retractedScale, animationDuration));
            yield return new WaitForSeconds(inactiveDuration);
        }
    }

    IEnumerator AnimateSpikes(Vector3 startScale, Vector3 endScale, float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            float progress = timer / duration;
            Vector3 currentScale = Vector3.Lerp(startScale, endScale, progress);
            foreach (var spike in spikes)
            {
                spike.transform.localScale = currentScale;
            }
            timer += Time.deltaTime;
            yield return null;
        }
        foreach (var spike in spikes)
        {
            spike.transform.localScale = endScale;
        }
    }
}
//------------------------------------------------------

/*This code is a Unity script that manages a wall of spikes that protrude and retract in a cycle. The spikes are instantiated at the start, and their behavior is controlled by a coroutine that animates their scale over time.
 * 
using System.Collections;
using UnityEngine;

public class WallThorns : MonoBehaviour
{
    [Header("Spike Settings")]
    public GameObject spikePrefab;
    public int numberOfSpikes = 5;
    public float verticalSpacing = 0.5f;
    // This now represents the fully PROTRUDED scale of the spikes
    public Vector3 spikeScale = new Vector3(0.2f, 2f, 0.2f);

    [Header("Timing")]
    public float activeDuration = 2.5f;   // How long spikes stay fully protruded
    public float inactiveDuration = 2.5f; // How long spikes stay fully retracted
    public float animationDuration = 0.5f; // How long the retract/protrude animation takes

    private GameObject[] spikes;
    private Vector3 _protrudedScale;
    private Vector3 _retractedScale;

    void Start()
    {
        spikes = new GameObject[numberOfSpikes];

        // --- Define the protruded and retracted states ---
        _protrudedScale = spikeScale;
        // Retracted scale is the same, but with zero height (Y-axis)
        _retractedScale = new Vector3(spikeScale.x, 0f, spikeScale.z);

        for (int i = 0; i < numberOfSpikes; i++)
        {
            Vector3 offset = new Vector3(0, i * verticalSpacing, 0);
            Vector3 spawnPos = transform.position + offset;

            GameObject spike = Instantiate(spikePrefab, spawnPos, Quaternion.identity, transform);
            spike.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            // --- Start spikes in their retracted state ---
            spike.transform.localScale = _retractedScale;

            if (spike.GetComponent<WallSpikeCollision>() == null)
                spike.AddComponent<WallSpikeCollision>();

            spikes[i] = spike;
        }

        StartCoroutine(ThornCycle());
    }

    /// <summary>
    /// The main loop that controls the timing of the spike animations.
    /// </summary>
    IEnumerator ThornCycle()
    {
        while (true)
        {
            // --- Protrude Animation ---
            // Wait for the protrude animation to finish before proceeding
            yield return StartCoroutine(AnimateSpikes(_retractedScale, _protrudedScale, animationDuration));

            // --- Active Duration ---
            // Wait for a few seconds while the spikes are out
            yield return new WaitForSeconds(activeDuration);

            // --- Retract Animation ---
            // Wait for the retract animation to finish before proceeding
            yield return StartCoroutine(AnimateSpikes(_protrudedScale, _retractedScale, animationDuration));

            // --- Inactive Duration ---
            // Wait for a few seconds while the spikes are hidden
            yield return new WaitForSeconds(inactiveDuration);
        }
    }

    /// <summary>
    /// Animates all spikes from a starting scale to an ending scale over a set duration.
    /// </summary>
    /// <param name="startScale">The scale to begin the animation from.</param>
    /// <param name="endScale">The scale to animate towards.</param>
    /// <param name="duration">The time in seconds the animation should take.</param>
    IEnumerator AnimateSpikes(Vector3 startScale, Vector3 endScale, float duration)
    {
        float timer = 0f;

        // The animation loop continues as long as the timer is less than the duration
        while (timer < duration)
        {
            // Calculate progress from 0.0 to 1.0
            float progress = timer / duration;
            Vector3 currentScale = Vector3.Lerp(startScale, endScale, progress);

            // Apply the calculated scale to all spikes
            foreach (var spike in spikes)
            {
                spike.transform.localScale = currentScale;
            }

            // Increment timer and wait for the next frame
            timer += Time.deltaTime;
            yield return null;
        }

        // After the loop, force the scale to the final value to ensure precision
        foreach (var spike in spikes)
        {
            spike.transform.localScale = endScale;
        }
    }
}
*/

/// The following commented-out code is an alternative approach to animating the spikes.

//IEnumerator AnimateSpikes(Vector3 startScale, Vector3 endScale, float duration)
//{
//    float timer = 0f;
//    while (timer < duration)
//    {
//        float progress = timer / duration;
//        Vector3 currentScale = Vector3.Lerp(startScale, endScale, progress);

//        for (int i = 0; i < spikes.Length; i++)
//        {
//            GameObject spike = spikes[i];
//            spike.transform.localScale = currentScale;

//            // Adjust the position so it appears to grow outward from the base
//            float currentHeight = currentScale.y;
//            float baseOffset = i * verticalSpacing;
//            spike.transform.localPosition = new Vector3(0, baseOffset + currentHeight / 2f, 0);
//        }

//        timer += Time.deltaTime;
//        yield return null;
//    }

//    // Final state to ensure exactness
//    for (int i = 0; i < spikes.Length; i++)
//    {
//        GameObject spike = spikes[i];
//        spike.transform.localScale = endScale;
//        float baseOffset = i * verticalSpacing;
//        spike.transform.localPosition = new Vector3(0, baseOffset + endScale.y / 2f, 0);
//    }
//}
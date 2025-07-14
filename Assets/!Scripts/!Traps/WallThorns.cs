using System.Collections;
using UnityEngine;

public class WallThorns : MonoBehaviour
{
    [Header("Spike Settings")]
    public GameObject spikePrefab;              // Assign your spike prefab here
    public int numberOfSpikes = 5;              // Number of spikes to generate
    public float verticalSpacing = 0.5f;        // Distance between each spike
    public Vector3 spikeScale = new Vector3(0.2f, 2f, 0.2f); // Length and size

    [Header("Timing")]
    public float activeDuration = 3f;           // How long spikes stay out
    public float inactiveDuration = 3f;         // How long spikes are hidden

    private GameObject[] spikes;                // Holds spawned spike references
    private bool isActive = false;

    void Start()
    {
        spikes = new GameObject[numberOfSpikes];

        for (int i = 0; i < numberOfSpikes; i++)
        {
            // Calculate vertical position for each spike
            Vector3 offset = new Vector3(0, i * verticalSpacing, 0);
            Vector3 spawnPos = transform.position + offset;

            // Create the spike and parent it to this object
            GameObject spike = Instantiate(spikePrefab, spawnPos, Quaternion.identity, transform);

            // Rotate so it faces forward (+Z)
            spike.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            // Resize
            spike.transform.localScale = spikeScale;

            // Add collision detection script (optional)
            if (spike.GetComponent<WallSpikeCollision>() == null)
                spike.AddComponent<WallSpikeCollision>();

            spikes[i] = spike;
        }

        StartCoroutine(ThornCycle());
    }

    IEnumerator ThornCycle()
    {
        while (true)
        {
            isActive = !isActive;

            foreach (GameObject spike in spikes)
            {
                spike.SetActive(isActive); // Visually toggle the spike (disappear/appear)
            }

            yield return new WaitForSeconds(isActive ? activeDuration : inactiveDuration);
        }
    }
}

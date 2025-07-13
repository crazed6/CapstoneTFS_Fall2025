using System.Collections;
using UnityEngine;
//Diego's Script

public class WallThorns : MonoBehaviour
{
    [Header("Spike Settings")]
    public GameObject spikePrefab;              // Prefab to instantiate for each spike
    public int numberOfSpikes = 5;              // Total number of spikes to generate
    public float verticalSpacing = 0.5f;        // Vertical distance between each spike
    public Vector3 spikeScale = new Vector3(0.2f, 2f, 0.2f); // Scale of each spike (Y = forward length)

    [Header("Timing")]
    public float activeDuration = 3f;           // How long spikes remain active
    public float inactiveDuration = 3f;         // How long spikes remain retracted

    private bool isActive = false;              // Internal flag to track current spike state
    private WallSpikeCollision[] spikeScripts;  // References to each spike's collision logic

    void Start()
    {
        // Initialize the array to store references to each spike script
        spikeScripts = new WallSpikeCollision[numberOfSpikes];

        // Loop to create and place each spike
        for (int i = 0; i < numberOfSpikes; i++)
        {
            // Position each spike vertically, stacked along Y-axis
            Vector3 offset = new Vector3(0, i * verticalSpacing, 0);
            Vector3 spawnPos = transform.position + offset;

            // Instantiate the spike prefab at the calculated position
            GameObject spike = Instantiate(spikePrefab, spawnPos, Quaternion.identity, transform);

            // Rotate the spike to face forward (Z+ direction)
            spike.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);

            // Scale the spike to match the specified length and thickness
            spike.transform.localScale = spikeScale;

            // Add the collision detection script and store a reference to it
            WallSpikeCollision sc = spike.AddComponent<WallSpikeCollision>();
            spikeScripts[i] = sc;
        }

        // Start the coroutine to handle spike activation/deactivation over time
        StartCoroutine(ThornCycle());
    }

    IEnumerator ThornCycle()
    {
        while (true)
        {
            // Toggle the spike state
            isActive = !isActive;

            // Update all spikes with the new active state
            foreach (WallSpikeCollision sc in spikeScripts)
            {
                sc.SetTrapActive(isActive);
            }

            // Wait depending on whether the spikes are active or inactive
            yield return new WaitForSeconds(isActive ? activeDuration : inactiveDuration);
        }
    }
}

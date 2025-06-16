using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ItemCollectionSimulator : MonoBehaviour
{

    public static ItemCollectionSimulator Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensures this instance persists across scenes
        }
        else
        {
            Destroy(gameObject); // Prevents duplicate instances
        }
    }

    public void SimulateItemCollection(GameObject player, string itemName)
    {
        GameObject existingItem = GameObject.Find(itemName);

        if (existingItem == null)
        {
            Debug.LogWarning("Item not found: " + itemName + ". Make sure the object name correctly matches!");
            return;
        }

        //Testing if disabling collider allows for single item collection
        Collider col = existingItem.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false; // Disable the collider to prevent interaction
        }

        // Instantiate the item at the player's position
        Vector3 targetPosition = player.transform.position + new Vector3(0, 1, 0); // Spawn above the player
        existingItem.transform.position = targetPosition;

        StartCoroutine(EnableColliderNextFrame(col)); // Re-enable the collider in the next frame
    }

    private IEnumerator EnableColliderNextFrame(Collider col) //IEnumerator to re-enable the collider after one frame
    {
        yield return null; // Wait for the next frame
        if (col != null)
        {
            col.enabled = true; // Re-enable the collider
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class PlayerSave : MonoBehaviour
{
    private static PlayerSave instance;
    private InventoryManager inventoryManager; // Reference to the InventoryManager
  

    private void Start()
    {
        // Find the InventoryManager in the scene and store a reference to it
        //inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    public void Save(ref PlayerSaveData data)
    {
        //data.Position = transform.position; //Replace with CharacterController.instance.transform.position if using CharacterController
        data.Position = CharacterController.instance.transform.position; //Ensures the position is saved from the CharacterController
        
        Health health = CharacterController.instance.GetComponent<Health>(); //Replace with GetComponent<Health>() if not using CharacterController
        //Saving Health as well, saved to same object

        //Health health = GetComponent<Health>(); //Replace with CharacterController.instance.GetComponent<Health>() if using CharacterController
        if (health != null)
        {
            data.Health = health.health; //Saves the current health
        }

        data.SceneName = SceneManager.GetActiveScene().name; //Saves the current scene name
        

        if (inventoryManager != null)
        {
           List<string> collectedItems = new List<string>();
            foreach (var slot in inventoryManager.itemSlot)
            {
                if (slot.isFull) // Check if the slot is occupied
                {
                    collectedItems.Add(slot.itemName); // Assuming itemName is a string representing the item's name
                }
            }
            data.collectedItems = collectedItems; // Save the list of collected items
        }
    }

    public void Load(PlayerSaveData data)
    {
        Debug.Log("Setting player position to:" + data.Position);
        GetComponent<CharacterController>().enabled = false;

        //transform.position = data.Position; //Used to convert the Data position into the current Transform. Change to CharacterController.instance.transform.position if using CharacterController
        CharacterController.instance.transform.position = data.Position; //Ensures the position is loaded from the CharacterController


        GetComponent<CharacterController>().enabled = true; //Re-enabling character controller so as to prevent the override of the player position on load

        //Health health = GetComponent<Health>();

        Health health = CharacterController.instance.GetComponent<Health>(); //Replace with GetComponent<Health>() if not using CharacterController
        if (health != null)
        {
            health.health = (int)data.Health; //Loading the health value from the file
        }

        if (data.collectedItems != null)
        {
            //if (ItemCollectionSimulator.Instance == null)
            //{
            //    Debug.LogError("ItemCollectionSimulator instance is null! Ensure it exists in the scene.");
            //    return;
            //}
            //else
            //{
            //    foreach (string itemName in data.collectedItems)
            //    {
            //        Debug.Log("Simulating item collection for: " + itemName);
            //        ItemCollectionSimulator.Instance.SimulateItemCollection(gameObject, itemName);
            //    }
            //}

            foreach (string itemName in data.collectedItems)
            {
               Item item = GameObject.Find(itemName)?.GetComponent<Item>(); 
                if (item != null)
                {
                    // Simulate the collection of the item
                    //item.CollectItem(); // Assuming CollectItem is a method that handles item collection
                    Destroy(item.gameObject); // Destroy the item GameObject to prevent duplication
                    Debug.Log("Collected item: " + itemName);
                }
                else
                {
                    Debug.LogWarning("Item not found: " + itemName);
                }
            }
        }

    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}


[System.Serializable]
public struct PlayerSaveData
{
    public Vector3 Position;
    public Vector3 lastCheckpoint;
    public int Health;
    public string SceneName; // This stores the last scene

    public List<string> collectedItems; // List to store inventory items

}

[System.Serializable]
public struct InventoryItemData
{
    public string itemName;
    public string itemDescription;
    public string spritePath;
}

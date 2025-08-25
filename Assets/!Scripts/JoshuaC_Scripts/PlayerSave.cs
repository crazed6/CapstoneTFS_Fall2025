using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;


public class PlayerSave : MonoBehaviour
{
    private static PlayerSave instance;
    private InventoryManager inventoryManager; // Reference to the InventoryManager
  

    private void Start()
    {

       
    }

    public void Save(ref PlayerSaveData data)
    {
        if (CharacterController.instance == null)
        {
            Debug.LogWarning("Save skipped: CharacterController.instance is null!");
            return;
        }

        // Position & Rotation
        data.Position = CharacterController.instance.transform.position;
        data.Rotation = CharacterController.instance.transform.rotation;

        // Health
        Health health = CharacterController.instance.GetComponent<Health>();
        if (health != null)
            data.Health = health.health;

        // Scene
        data.SceneName = SceneManager.GetActiveScene().name;

        // Inventory
        var inventoryObj = GameObject.Find("InventoryCanvas");
        if (inventoryObj != null)
        {
            inventoryManager = inventoryObj.GetComponent<InventoryManager>();
            if (inventoryManager != null)
            {
                List<string> collectedItems = new List<string>();
                foreach (var slot in inventoryManager.itemSlot)
                {
                    if (slot.isFull)
                        collectedItems.Add(slot.itemName);
                }
                data.collectedItems = collectedItems;
            }
        }
    }

    public void Load(PlayerSaveData data)
    {
        //use using (Character Controller.instance.GetComponent<CharacterController>()) Saves memory by not having to use GetComponent<CharacterController>() multiple times


        Debug.Log("Setting player position to:" + data.Position);
        CharacterController.instance.GetComponent<CharacterController>().enabled = false;

        //transform.position = data.Position; //Used to convert the Data position into the current Transform. Change to CharacterController.instance.transform.position if using CharacterController
        CharacterController.instance.transform.position = data.Position; //Ensures the position is loaded from the CharacterController
        CharacterController.instance.transform.rotation = data.Rotation; //Loads the rotation of the player as well

        CharacterController.instance.GetComponent<CharacterController>().enabled = true; //Re-enabling character controller so as to prevent the override of the player position on load

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

            if (data.collectedItems != null)
            {
                GameObject[] allItems = GameObject.FindGameObjectsWithTag("Journal");

                Debug.Log("Found " + allItems.Length + " collectible items in the scene.");

                //New Test
                InventoryManager inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
                //New Test End

                foreach (GameObject itemObj in allItems)
                {
                    Item item = itemObj.GetComponent<Item>();
                    if (item != null && data.collectedItems.Contains(item.ItemName))
                    {
                        //New Test
                        inventoryManager.AddItem(item.ItemName, item.ItemSprite, item.ItemDescription); // Assuming AddItem is a method that adds the item to the inventory
                        //New Test End

                        Destroy(item.gameObject);
                        Debug.Log("Destroyed collected item: " + item.ItemName);
                    }
                }
            }

            //foreach (string itemName in data.collectedItems)
            //{
            //   Item item = GameObject.Find(itemName)?.GetComponent<Item>(); 
            //    if (item != null)
            //    {
            //        // Simulate the collection of the item
            //        //item.CollectItem(); // Assuming CollectItem is a method that handles item collection
            //        Destroy(item.gameObject); // Destroy the item GameObject to prevent duplication
            //        Debug.Log("Collected item: " + itemName);
            //    }
            //    else
            //    {
            //        Debug.LogWarning("Item not found: " + itemName);
            //    }
            //}
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
    public Quaternion Rotation;
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

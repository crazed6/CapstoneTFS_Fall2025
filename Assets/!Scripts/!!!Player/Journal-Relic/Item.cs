using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private string itemName;

    [SerializeField]
    private Sprite sprite;

    [TextArea]
    [SerializeField]
    private string itemDescription;

    // Find the InventoryManager in the scene and store a reference to it
    private InventoryManager inventoryManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }


    //Picking up the Item
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Add this item to the player's inventory
            inventoryManager.AddItem(itemName, sprite, itemDescription);
            Destroy(gameObject); // Destroy this item GameObject so it disappears after being collected

        }
    }
}

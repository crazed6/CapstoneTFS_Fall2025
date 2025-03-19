using UnityEngine;
using UnityEngine.Rendering;

public class InventoryManager : MonoBehaviour
{

    public GameObject InventoryMenu;
    private bool menuActivated;
    public ItemSlot[] itemSlot; // Array of item slots where items will be stored

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //Activating the Menu Canvas
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (menuActivated)
            {
                Time.timeScale = 1;
                InventoryMenu.SetActive(false);
                menuActivated = false;

                // Hide the cursor and lock it
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Time.timeScale = 0;
                InventoryMenu.SetActive(true);
                menuActivated = true;

                // Show the cursor and unlock it
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }


    // Adds an item to the first available empty slot
    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        // Loop through all item slots to find an empty one
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull == false) // Check if the slot is empty
            {
                // Assign the item details to the empty slot
                itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription);
                return; // Stop loop
            }
        }  
    }

    // Deselects all item slots, removing selection highlights
    public void DeselectAllSlots()
    {
        // Loop through all item slots
        for (int i = 0; i < itemSlot.Length; i++)
        {
            // Disable selection highlight effect
            itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].thisItemSelected = false; // Mark slot as unselected
        }
    }
}

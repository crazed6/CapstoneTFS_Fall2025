using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public GameObject JournalMenu;
    private bool menuActivated;
    public ItemSlot[] itemSlot; // Array of item slots where items will be stored

    [Header("Optional Scrollbar Reset")]
    public ScrollRect descriptionScrollRect; // Assign your ItemDescription ScrollRect here

    void Update()
    {
        //Activating the Menu Canvas
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (menuActivated)
            {
                Time.timeScale = 1;
                JournalMenu.SetActive(false);
                menuActivated = false;

                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Time.timeScale = 0;
                JournalMenu.SetActive(true);
                menuActivated = true;

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                HideAllItemDescriptions();
            }
        }
    }

    public void AddItem(string itemName, Sprite itemSprite, string itemDescription)
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull == false)
            {
                itemSlot[i].AddItem(itemName, itemSprite, itemDescription);
                return;
            }
        }
    }

    public void HideAllItemDescriptions()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i] != null && itemSlot[i].itemDescrription != null)
            {
                itemSlot[i].itemDescrription.SetActive(false);
            }
        }
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i] != null && itemSlot[i].selectedShader != null)
            {
                itemSlot[i].selectedShader.SetActive(false);
                itemSlot[i].thisItemSelected = false;
            }
        }
    }

    // Call this from ItemSlot when a new item is shown
    public void ResetDescriptionScroll()
    {
        if (descriptionScrollRect != null)
        {
            descriptionScrollRect.verticalNormalizedPosition = 1f; // scrolls back to top
        }
    }
}



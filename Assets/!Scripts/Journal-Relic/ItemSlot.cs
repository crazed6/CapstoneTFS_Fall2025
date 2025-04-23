using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    //Item Data
    public string itemName;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;
    public GameObject itemDescrription;


    //Item Slot
    [SerializeField]
    private Image itemImage;

    //Item Description Slot
    public Image ItemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;

    //Selected Item Outline
    public GameObject selectedShader;
    public bool thisItemSelected;

    private InventoryManager inventoryManager;

    private void Start()
    {
        // Finds the InventoryManager in the scene and assigns it to inventoryManager
        inventoryManager = GameObject.Find("InventoryCanvas").GetComponent<InventoryManager>();
    }

    //Where everything of the item will be displayed
    public void AddItem(string itemName, Sprite itemSprite, string itemDescription)
    {
        // Assign item details to this slot
        this.itemName = itemName;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        isFull = true; // Mark this slot as occupied

        // Update UI elements
        itemImage.sprite = itemSprite; // Show the item's icon in the slot
    }

    //Allows Mouse input
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
    }

    public void OnLeftClick()
    {
        inventoryManager.DeselectAllSlots(); // Deselect all other slots

        selectedShader.SetActive(true);      // Highlight this slot regardless
        thisItemSelected = true;             // Mark this one as selected

        if (isFull)
        {
            // Show and update the item description panel
            itemDescrription.SetActive(true);
            ItemDescriptionNameText.text = itemName;
            ItemDescriptionText.text = itemDescription;
            ItemDescriptionImage.sprite = itemSprite != null ? itemSprite : emptySprite;
        }
        else
        {
            // Hide the item description panel if there's no item
            itemDescrription.SetActive(false);
        }
    }
}
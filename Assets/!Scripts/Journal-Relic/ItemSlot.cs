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
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;


    //Item Slot
    [SerializeField]
    private TMP_Text quantityText;

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
    public void AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription)
    {
        // Assign item details to this slot
        this.itemName = itemName;
        this.quantity = quantity;
        this.itemSprite = itemSprite;
        this.itemDescription = itemDescription;
        isFull = true; // Mark this slot as occupied

        // Update UI elements
        quantityText.text = quantity.ToString(); // Show item quantity
        quantityText.enabled = true; // Enable the quantity text
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
        inventoryManager.DeselectAllSlots();//Deselects all slots first
        selectedShader.SetActive(true);//Highlights this specific slot
        thisItemSelected = true;//Marks this slot as selected

        //Updates the item description panel
        ItemDescriptionNameText.text = itemName;
        ItemDescriptionText.text = itemDescription;
        ItemDescriptionImage.sprite = itemSprite;

        //If there are no sprites, use default empty sprite
        if (ItemDescriptionImage.sprite == null)
            ItemDescriptionImage.sprite = emptySprite;
        
    }

}

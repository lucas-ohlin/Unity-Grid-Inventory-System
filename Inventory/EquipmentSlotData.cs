using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlotData : MonoBehaviour, IPointerClickHandler {

    public EnumEquipmentType enumEquipmentType;
    public InventoryItem equippedItem;
    public InventoryController inventoryController;

    private RectTransform rectTransform;

    [SerializeField] private GameObject itemPrefab;
    [SerializeField] private GameObject bpPrefab;


    private void Start() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerClick(PointerEventData eventData) {
        Vector2 clickPosition = eventData.position;

        //Calculate the boundaries of the slot
        Vector2 slotPosition = RectTransformUtility.WorldToScreenPoint(null, rectTransform.position);
        Rect slotRect = new Rect(slotPosition.x, slotPosition.y, rectTransform.rect.width, rectTransform.rect.height);

        if (equippedItem == null) {
            if (inventoryController.selectedItem != null) {
                //Check if the click position is within the slot's boundaries
                if (slotRect.Contains(clickPosition)) {
                    PlaceItemInEquipmentSlot();
                }
            }
        } 
        else {
            //Try Insert the item back into the inventory
            if (equippedItem != null) {
                if (inventoryController.selectedItem == null) {
                    //Check if the click position is within the slot's boundaries
                    if (slotRect.Contains(clickPosition)) {
                        Debug.Log("Unequip...");
                        inventoryController.PickUpItemFromSlot(equippedItem);
                        //equippedItem.RemoveFromInventory();
                        equippedItem = null;
                    }
                }
            }
        }

    }

    private void PlaceItemInEquipmentSlot() {
        //Check if the slot & the item are the same sort of item type
        if (inventoryController.selectedItem.itemData.enumItemType == enumEquipmentType) {

            var prefabType = itemPrefab;
            var itemType = inventoryController.selectedItem.itemData.enumItemType;

            switch (itemType) {
                case EnumEquipmentType.Headgear:
                    break;
                case EnumEquipmentType.Facecover:
                    break;
                case EnumEquipmentType.Rig:
                    break;
                case EnumEquipmentType.Armor:
                    break;
                case EnumEquipmentType.Backpack:
                    prefabType = bpPrefab;
                    break;
                case EnumEquipmentType.Gun:
                    break;
                case EnumEquipmentType.Pistol:
                    break;
                case EnumEquipmentType.Melee:
                    break;
                case EnumEquipmentType.Item:
                    break;
                default:
                    Debug.LogError("Unhandled item type: " + itemType);
                    break;
            }

            Debug.Log("PREFAB TYPE : " + prefabType);

            InventoryItem selectedItem = inventoryController.selectedItem;
            inventoryController.selectedItem = null;
    
            InventoryItem inventoryItem = selectedItem;

            RectTransform itemRectTransform = inventoryItem.GetComponent<RectTransform>();

            if (inventoryItem.rotated == true)
                inventoryItem.Rotate();

            //Set the item onto the equipment slot visually
            itemRectTransform.SetParent(this.rectTransform);
            itemRectTransform.localPosition = Vector3.zero;
            itemRectTransform.SetAsLastSibling();

            //Give instatiated prefab correct information
            inventoryItem.Set(selectedItem.itemData);
            equippedItem = inventoryItem;

            //Scale down the item to fit the box
            if (itemRectTransform.rect.width > rectTransform.rect.width || itemRectTransform.rect.height > rectTransform.rect.height) {
                itemRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, itemRectTransform.rect.width * 110 / itemRectTransform.rect.height);
                itemRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 110);
            }

            //Center the item in the slot
            Vector2 itemPos = itemRectTransform.anchoredPosition;
            itemPos.x = 0;
            itemPos.y = 0;
            itemRectTransform.anchoredPosition = itemPos;

            inventoryItem.UseEquipment(inventoryController);
            //RemoveOldItemFromInventory(inventoryController.selectedItem);

        }
    }

    private void RemoveOldItemFromInventory(InventoryItem selectedItem) {
        selectedItem.RemoveFromInventory();
    }

}


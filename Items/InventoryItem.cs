using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour {

    public ItemData itemData;

    public int HEIGHT {
        get {
            if (rotated == false) {
                return itemData.height;
            }
            return itemData.width;
        }
    }

    public int WIDTH {
        get {
            if (rotated == false) {
                return itemData.width;
            }
            return itemData.height;
        }
    }

    public int onGridPosX;
    public int onGridPosY;
    public bool rotated = false;

    internal void Set(ItemData itemData) {
        this.itemData = itemData;
        GetComponent<Image>().sprite = itemData.itemIcon;

        Vector2 size = new Vector2();
        size.x = itemData.width * ItemGrid.tileSizeWidth;
        size.y = itemData.height * ItemGrid.tileSizeHeight;
        GetComponent<RectTransform>().sizeDelta = size;
    }

    public void Rotate() {
        rotated = !rotated;

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0, 0, rotated == true ? 90f : 0f);
    }

    public void UseEquipment(InventoryController inventoryController) {
        switch (itemData.enumItemType) {
            case EnumEquipmentType.Headgear:
                break;
            case EnumEquipmentType.Facecover:
                break;
            case EnumEquipmentType.Rig:
                break;
            case EnumEquipmentType.Armor:
                break;
            case EnumEquipmentType.Backpack:
                UseBackpack(inventoryController);
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
                Debug.LogError("Unhandled item type: " + itemData.enumItemType);
                break;
        }
    }

    private void UseBackpack(InventoryController inventoryController) {
        Debug.Log("Size x : " + itemData.sizeX + " | Size y : " + itemData.sizeY);
        var bpSlots = gameObject.transform.GetChild(0);

        inventoryController.itemGridList.Add(bpSlots.GetComponent<ItemGrid>());
        bpSlots.gameObject.SetActive(true);
    }

    //public void UnUseEquipment() {
    //    switch (itemData.enumItemType) {
    //        case EnumEquipmentType.Headgear:
    //            break;
    //        case EnumEquipmentType.Facecover:
    //            break;
    //        case EnumEquipmentType.Rig:
    //            break;
    //        case EnumEquipmentType.Armor:
    //            break;
    //        case EnumEquipmentType.Backpack:
    //            Debug.Log("Size x : " + itemData.sizeX + " | Size y : " + itemData.sizeY);
    //            gameObject.transform.GetChild(0).gameObject.SetActive(false);
    //            break;
    //        case EnumEquipmentType.Gun:
    //            break;
    //        case EnumEquipmentType.Pistol:
    //            break;
    //        case EnumEquipmentType.Melee:
    //            break;
    //        case EnumEquipmentType.Item:
    //            break;
    //        default:
    //            Debug.LogError("Unhandled item type: " + itemData.enumItemType);
    //            break;
    //    }
    //}

    public void Use() {
        RemoveFromInventory();
        Debug.Log("Use");
    }

    public void RemoveFromInventory() {
        Destroy(this.gameObject);
    }

}

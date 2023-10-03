using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumEquipmentType {
    Headgear,
    Facecover,
    Rig,
    Armor,
    Backpack,
    Gun,
    Pistol,
    Melee,
    Item
}

[CreateAssetMenu(menuName = "Inventory/Item")]
public class ItemData : ScriptableObject {

    public EnumEquipmentType enumItemType;

    //new public string name = "Name";
    public Sprite itemIcon;

    [Header("Size In Inventory")]
    public int width = 1;
    public int height = 1;

    [Header("Backpack/Rig Attributes")]
    public int sizeX;
    public int sizeY;

}

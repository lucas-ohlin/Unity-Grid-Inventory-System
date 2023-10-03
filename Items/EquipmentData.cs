using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Inventory/Equipment")]
public class EquipmentData : ItemData {

    [Header("Armor Attributes")]
    public int armorModifier;

    [Header("Weapon Attributes")]
    public int damageModifier;

    //[Header("Backpack/Rig Attributes")]
    //public int sizeX;
    //public int sizeY;

}
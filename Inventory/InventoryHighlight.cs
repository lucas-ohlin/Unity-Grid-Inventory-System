using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHighlight : MonoBehaviour {

    [SerializeField] RectTransform highLighter;
    [SerializeField] GameObject highLighterPrefab;

    [SerializeField] ItemGrid defaultGrid;

    public void Show(bool show) {
        if (highLighter != null) {
            highLighter.gameObject.SetActive(show);
        }
    }

    public void SetSize(InventoryItem targetItem) {
        Vector2 size = new Vector2();
        size.x = targetItem.WIDTH * ItemGrid.tileSizeWidth;
        size.y = targetItem.HEIGHT * ItemGrid.tileSizeHeight;
        highLighter.sizeDelta = size;
    }

    public void SetParent(ItemGrid targetGrid) {
        if (targetGrid == null) { return; }
        if (highLighter != null) {
            highLighter.SetParent(targetGrid.GetComponent<RectTransform>());
        } 
        else if (highLighter == null && highLighterPrefab != null) {
            //Instantiate a new highLighter GameObject from the prefab
            highLighter = Instantiate(highLighterPrefab).GetComponent<RectTransform>();
            highLighter.SetParent(targetGrid.GetComponent<RectTransform>());
        }
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem) {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, targetItem.onGridPosX, targetItem.onGridPosY);
        highLighter.localPosition = pos;
    } 
    
    //public void SetPosition(InventoryItem targetItem) {
    //    Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, targetItem.onGridPosX, targetItem.onGridPosY);
    //    highLighter.localPosition = pos;
    //}

    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem, int posX, int posY) {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);
        highLighter.localPosition = pos;
    }

}

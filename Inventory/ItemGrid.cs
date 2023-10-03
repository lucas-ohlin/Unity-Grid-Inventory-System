using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour {

    public const float tileSizeWidth = 32;
    public const float tileSizeHeight = 32;

    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();

    InventoryItem[,] inventoryItemSlot;
    public int gridSizeWidth = 10;
    public int gridSizeHeight = 20;

    //Starts at Top Left
    RectTransform rectTransform;

    private void Start() {

        rectTransform = GetComponent<RectTransform>();

        //Check if the gameobject's parent is an equipment
        InventoryItem gridParent = gameObject.transform.parent.GetComponent<InventoryItem>();
        if (gridParent != null) {
            Init(gridParent.itemData.sizeX, gridParent.itemData.sizeY);
        }
        //Initialize non-equipment itemgrids
        else {
            Init(gridSizeWidth, gridSizeHeight);
        }

    }

    public void SetGridSize(int x, int y) {
        Init(x, y);
    }

    private void Init(int width, int height) {
        //Width and height of the inventory by slots
        inventoryItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        rectTransform.sizeDelta = size;
    }

    bool PositionCheck(int posX, int posY) {
        if (posX < 0 || posY < 0) {
            return false;
        }
        if (posX >= gridSizeWidth || posY >= gridSizeHeight) {
            return false;
        }
        return true;
    }

    public bool BoundryCheck(int posX, int posY, int widht, int height) {
        if (PositionCheck(posX, posY) == false) { return false; }
        //1 is the minimum size of the object
        posX += widht - 1;
        posY += height - 1;

        if (PositionCheck(posX, posY) == false) { return false; }

        return true;
    }

    private bool OverlapCheck(int posX, int posY, int widht, int height, ref InventoryItem overlapItem) {
        for (int x = 0; x < widht; x++) {
            for (int y = 0; y < height; y++) {
                //If there's an item being overlapped
                if (inventoryItemSlot[posX + x, posY + y] != null) {
                    if (overlapItem == null) {
                        overlapItem = inventoryItemSlot[posX + x, posY + y];
                    } else {
                        if (overlapItem != inventoryItemSlot[posX + x, posY + y]) {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    private bool CheckAvailableInventorySpace(int posX, int posY, int width, int height) {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {

                if (inventoryItemSlot[posX + x, posY + y] != null) {
                    return false;
                }

            }
        }
        return true;
    }

    public Vector2Int? FindSpaceForItem(InventoryItem itemToInsert) {

        int height = gridSizeHeight - itemToInsert.HEIGHT + 1;
        int width = gridSizeWidth - itemToInsert.WIDTH + 1;

        // Go through available inventory slots
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (CheckAvailableInventorySpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT)) {
                    //Return slots that are available for the item
                    Vector2Int position = new Vector2Int(x, y);
                    Debug.Log("Available space found at: " + position);
                    return position;
                }
            }
        }

        //If item is 1 : 1, just return
        if (itemToInsert.WIDTH == 1 && itemToInsert.HEIGHT == 1)
            return null;

        //If there's no available space for the item, try flipping height and width
        itemToInsert.Rotate();
        height = gridSizeHeight - itemToInsert.HEIGHT + 1;
        width = gridSizeWidth - itemToInsert.WIDTH + 1;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (CheckAvailableInventorySpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT)) {
                    //Return slots that are available for the item after rotating
                    Vector2Int position = new Vector2Int(x, y);
                    Debug.Log("Available space found after rotation at: " + position);
                    return position;
                }
            }
        }

        //There's no available space for the item, even after rotating
        Debug.Log("No available space found.");
        return null;
    }

    public bool FindSpaceForItemBool(InventoryItem itemToInsert) {

        int height = gridSizeHeight - itemToInsert.HEIGHT + 1;
        int width = gridSizeWidth - itemToInsert.WIDTH + 1;

        // Go through available inventory slots
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (CheckAvailableInventorySpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT)) {
                    return true;
                }
            }
        }

        //If there's no available space for the item, try flipping height and width
        itemToInsert.Rotate();
        //Recheck for available space after rotating
        height = gridSizeHeight - itemToInsert.HEIGHT + 1;
        width = gridSizeWidth - itemToInsert.WIDTH + 1;

        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                if (CheckAvailableInventorySpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT)) {
                    return true;
                }
            }
        }

        Debug.Log("No available space found.");
        return false;
    }

    public InventoryItem GetItem(int x, int y) {
        return inventoryItemSlot[x, y];
    }

    public Vector2Int GetTileGridPosition(Vector2 mousePosition) {

        //Mouse position on the grid
        positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;

        //Tile position on the grid
        tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth);
        tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight);

        return tileGridPosition;
    }

    private void CheckItemInventorySpace(InventoryItem item) {
        for (int ix = 0; ix < item.WIDTH; ix++) {
            for (int iy = 0; iy < item.HEIGHT; iy++) {
                inventoryItemSlot[item.onGridPosX + ix, item.onGridPosY + iy] = null;
            }
        }
    }

    public InventoryItem PickUpItem(int x, int y) {
        InventoryItem toReturn = inventoryItemSlot[x, y];
        if (toReturn == null) { return null; }

        //Cycle trough width & height of the item,
        //inventory slots it takes up
        CheckItemInventorySpace(toReturn);

        return toReturn;
    }

    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem) {

        //Check if the item that's being placed is being placed outside of the inventory
        if (BoundryCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT) == false) {
            return false;
        }

        if (OverlapCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT, ref overlapItem) == false) {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null) {
            CheckItemInventorySpace(overlapItem);
        }

        PlaceItem(inventoryItem, posX, posY);

        return true;
    }

    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY) {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        //Cycle trough width & height of the item to get how many
        //inventory slots it should take up
        for (int x = 0; x < inventoryItem.WIDTH; x++) {
            for (int y = 0; y < inventoryItem.HEIGHT; y++) {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }

        //Assign what grid position the item has
        inventoryItem.onGridPosX = posX;
        inventoryItem.onGridPosY = posY;
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);

        rectTransform.localPosition = position;
    }

    public Vector2 CalculatePositionOnGrid(InventoryItem inventoryItem, int posX, int posY) {

        Vector2 position = new Vector2();
        position.x = posX * tileSizeWidth + tileSizeWidth * inventoryItem.WIDTH / 2;
        position.y = -(posY * tileSizeHeight + tileSizeHeight * inventoryItem.HEIGHT / 2);

        return position;
    }
}

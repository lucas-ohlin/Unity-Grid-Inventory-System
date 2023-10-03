using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour {

    [HideInInspector]
    private ItemGrid selectedItemGrid;
    public ItemGrid SelectedItemGrid {
        get => selectedItemGrid;
        set {
            selectedItemGrid = value;
            inventoryHighlight.SetParent(value);
        }
    }

    public List<ItemGrid> itemGridList = new List<ItemGrid>();

    public InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectTransform;

    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject bpPrefab;
    [SerializeField] Transform canvasTransform;

    InventoryHighlight inventoryHighlight;

    [Header("Inventory / Pickup variables")]
    [SerializeField] private float pickupDistance = 3;
    [SerializeField] private GameObject inventoryCanvas;
    private Canvas canvasComponent;

    [SerializeField] private EquipmentSlotData[] equipmentSlotDataArray;
    private EquipmentData[] equippedItems;

    public ItemGrid testGrid;

    private void Awake() {
        inventoryHighlight = GetComponent<InventoryHighlight>();
        canvasComponent = inventoryCanvas.GetComponent<Canvas>();
        equippedItems = new EquipmentData[equipmentSlotDataArray.Length];
    }

    private void Update() {
        if (canvasComponent.enabled == true) {
            //Visualize the drag of the item
            ItemIconDrag();

            if (Input.GetKeyDown(KeyCode.R)) {
                RotateItem();
            }

            if (selectedItemGrid == null) {
                inventoryHighlight.Show(false);
                return;
            }

            HandleHighlight();

            if (Input.GetMouseButtonDown(0)) {
                //Pick up / place down an item
                InteractWithInventoryItem();
            }

            if (Input.GetKeyDown(KeyCode.Q)) {
                //Pick up / place down an item
                testGrid.SetGridSize(5, 5);
            }
        }
    }

    //Helper method to create an InventoryItem from an EquipmentData
    private InventoryItem CreateInventoryItemFromEquipment(EquipmentData equipmentData) {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        inventoryItem.Set(equipmentData);

        return inventoryItem;
    }

    private void ItemIconDrag() {
        if (selectedItem != null) {
            Vector3 dragPosition = Input.mousePosition;
            //dragPosition.z = 1;
            rectTransform.SetAsLastSibling();
            rectTransform.position = dragPosition;
        }
    }

    private void RotateItem() {
        if (selectedItem == null) { return; }
        selectedItem.Rotate();
    }

    //Throw out item from inventory eventually
    InventoryItem itemToRemove;
    public void RemoveItem() {

        Vector2Int posOnGrid = GetTileGridPosition();

        if (selectedItem == null) {
            itemToRemove = selectedItemGrid.GetItem(posOnGrid.x, posOnGrid.y);
            if (itemToRemove != null) {
                itemToRemove.RemoveFromInventory();
            }
        }
    }

    public void RemoveItem(InventoryItem itemToRemove) {

        Vector2Int posOnGrid = GetTileGridPosition();

        if (selectedItem == null) {
            itemToRemove = selectedItemGrid.GetItem(posOnGrid.x, posOnGrid.y);
            if (itemToRemove != null) {
                itemToRemove.RemoveFromInventory();
            }
        }
    }

    public void PickUpItemFromSlot(InventoryItem itemInSlot) {
        if (itemInSlot != null) {

            InventoryItem inventoryItem = itemInSlot;

            if (itemInSlot.itemData.enumItemType == EnumEquipmentType.Backpack) {
                itemGridList.Remove(inventoryItem.gameObject.transform.GetChild(0).GetComponent<ItemGrid>());
                inventoryItem.gameObject.transform.GetChild(0).gameObject.SetActive(false);
            } 

            selectedItem = inventoryItem;

            rectTransform = inventoryItem.GetComponent<RectTransform>();
            rectTransform.SetParent(canvasTransform);
            rectTransform.SetAsLastSibling();

            inventoryItem.Set(itemInSlot.itemData);

        }
    }

    public void PickUpAndInsertItem(InventoryItem itemOnGround) {
        if (itemOnGround != null) {

            InventoryItem inventoryItem;

            if (itemOnGround.itemData.enumItemType == EnumEquipmentType.Backpack) {
                inventoryItem = Instantiate(bpPrefab).GetComponentInChildren<InventoryItem>();
            } else {
                inventoryItem = Instantiate(itemPrefab).GetComponentInChildren<InventoryItem>();
            }

            selectedItem = inventoryItem;

            rectTransform = inventoryItem.GetComponent<RectTransform>();
            rectTransform.SetParent(canvasTransform);
            rectTransform.SetAsLastSibling();

            inventoryItem.Set(itemOnGround.itemData);

            InsertItem(itemOnGround);
        }
    }

    private void InsertItem(InventoryItem itemToInsert) {

        itemToInsert = selectedItem;
        selectedItem = null;

        selectedItemGrid = FindGridWithSpace(itemToInsert);
        if (selectedItemGrid == null) {
            Destroy(itemToInsert.gameObject);
            return;
        }

        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForItem(itemToInsert);
        Debug.Log("ITEM : " + itemToInsert.GetInstanceID() + " | POS: " + posOnGrid);

        if (posOnGrid == null) {
            //If there's no available space, destroy the item and return
            Debug.Log("Inventory is full. Cannot insert item.");
            Destroy(itemToInsert.gameObject);
            return;
        }

        if (posOnGrid != null) {
            selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        }

        selectedItemGrid = null;
    }

    private ItemGrid FindGridWithSpace(InventoryItem itemToInsert) {
        foreach (var grid in itemGridList) {
            if (grid.FindSpaceForItemBool(itemToInsert)) {
                return grid;
            }
        }
        return null;
    }

    Vector2 oldPos;
    InventoryItem itemToHighlight;
    private void HandleHighlight() {

        Vector2Int posOnGrid = GetTileGridPosition();
        if (oldPos == posOnGrid) { return; }

        oldPos = posOnGrid;
        if (selectedItem == null) {
            Debug.Log("X : " + posOnGrid.x + " | Y : " + posOnGrid.y);

            if (posOnGrid.x < 0)
                posOnGrid.x = 0;
            if (posOnGrid.y < 0)
                posOnGrid.y = 0;

            itemToHighlight = selectedItemGrid.GetItem(posOnGrid.x, posOnGrid.y);

            if (itemToHighlight != null) {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighlight);
                inventoryHighlight.SetPosition(selectedItemGrid, itemToHighlight);
            }
            else {
                inventoryHighlight.Show(false);
            }
        }
        else {
            inventoryHighlight.Show(selectedItemGrid.BoundryCheck(
                posOnGrid.x, posOnGrid.y, selectedItem.WIDTH, selectedItem.HEIGHT
            ));
            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, posOnGrid.x, posOnGrid.y);
        }
    }

    private void InteractWithInventoryItem() {
        Vector2Int tileGridPos = GetTileGridPosition();
        Debug.Log("NUMBER: " + tileGridPos);
        if (selectedItem == null) {
            PickUp(tileGridPos);
        } else {
            Place(tileGridPos);
        }
    }

    private Vector2Int GetTileGridPosition() {
        Vector2 position = Input.mousePosition;

        if (selectedItem != null) {
            position.x -= (selectedItem.WIDTH - 1) * ItemGrid.tileSizeWidth / 2;
            position.y += (selectedItem.HEIGHT - 1) * ItemGrid.tileSizeHeight / 2;
        }

        //Vector2Int tileGridPos = selectedItemGrid.GetTileGridPosition(position);
        return selectedItemGrid.GetTileGridPosition(position);
    }

    private void Place(Vector2Int tileGridPos) {
        bool complete = selectedItemGrid.PlaceItem(selectedItem, tileGridPos.x, tileGridPos.y, ref overlapItem);

        if (complete) {
            selectedItem = null;
            if (overlapItem != null) {
                selectedItem = overlapItem;
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();

                Transform parentTransform = selectedItemGrid.transform.parent.parent;

                if (parentTransform != null) {
                    EquipmentSlotData equipmentSlot = parentTransform.GetComponent<EquipmentSlotData>();
                    if (equipmentSlot != null) {
                        parentTransform.SetAsLastSibling();
                    } else {
                        selectedItemGrid.transform.SetAsLastSibling();
                    }
                }
            }
        }
    }

    private void PickUp(Vector2Int tileGridPos) {

        selectedItem = selectedItemGrid.PickUpItem(tileGridPos.x, tileGridPos.y);
        Transform parentTransform = selectedItemGrid.transform.parent.parent;

        if (parentTransform != null) {
            EquipmentSlotData equipmentSlot = parentTransform.GetComponent<EquipmentSlotData>();
            if (equipmentSlot != null) {
                parentTransform.SetAsLastSibling();
            } else {
                selectedItemGrid.transform.SetAsLastSibling();
            }
        }

        if (selectedItem != null) {
            rectTransform = selectedItem.GetComponent<RectTransform>();
        }
    }

}

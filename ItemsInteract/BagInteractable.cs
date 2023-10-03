using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagInteractable : Interactable {

    public override void OnFocus() {
        print("Looking At : " + gameObject.name);
    }

    public override void OnInteract(GameObject player) {
        print("Interacted With : " + gameObject.name);
        player.GetComponentInChildren<InventoryController>().PickUpAndInsertItem(gameObject.GetComponent<InventoryItem>());
    }

    public override void OnLoseFocus() {
        print("Stopped Looking At : " + gameObject.name);
    }

}

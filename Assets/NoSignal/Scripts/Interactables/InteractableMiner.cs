using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableMiner : InteractableChest
{
    [SerializeField] 
    protected MinerBase minerScript;

    public override void Interaction()
    {
        minerScript.UpdateInventory();
        inventoryManager.GetUIElementsForChest(uiObjects.chestInventoryObject, uiObjects.chestInventoryCursor, uiObjects.chestPlayerSlotsHolder, uiObjects.chestSlotsHolder);
        inventoryManager.OpenChest(inventory, inventory.maxSize, gameObject, false);
    }
}

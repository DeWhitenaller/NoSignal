using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableCraftStation : InteractableChest
{
    [SerializeField] 
    private Material normalMat, ledMat;

    [SerializeField] 
    private MeshRenderer[] ledObjectMeshRenderers;





    public override void OnInteract()
    {
        Interaction();
    }

    public override void Interaction()
    {
        TurnOnLed();

        InventoryManager.OnCraftStationClose += CloseCraftStation;

        inventoryManager.GetUIElementsForCraftStation(uiObjects.craftStationInventoryObject, uiObjects.craftStationInventoryCursor, uiObjects.craftStationPlayerSlotsHolder, uiObjects.craftStationChestSlotsHolder, uiObjects.craftProgressBar, uiObjects.craftSlotsHolder, uiObjects.craftingQueueSlotsHolder, uiObjects.craftSlotSelector, uiObjects.requirementSlotsHolder, uiObjects.craftAmountText);
        inventoryManager.OpenChest(inventory, inventory.maxSize, gameObject, true);
    }




    private void CloseCraftStation(InventoryManager _inventoryManager)
    {
        TurnOffLed();
        InventoryManager.OnCraftStationClose -= CloseCraftStation;
    }




    private void TurnOnLed()
    {
        for (int i = 0; i < ledObjectMeshRenderers.Length; i++)
        {
            ledObjectMeshRenderers[i].material = ledMat;
        }
    }

    private void TurnOffLed()
    {
        for (int i = 0; i < ledObjectMeshRenderers.Length; i++)
        {
            ledObjectMeshRenderers[i].material = normalMat;
        }
    }
}

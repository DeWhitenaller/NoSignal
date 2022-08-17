using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableBeet : InteractableBase
{
    protected Inventory inventory;

    protected InventoryManager inventoryManager;

    protected UIObjectsHolder uiObjects;

    protected PlantGrow plantGrow;





    public override void Start()
    {
        plantGrow = GetComponent<PlantGrow>();
        inventory = GetComponent<Inventory>();
        inventoryManager = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryManager>();
        base.Start();
        uiObjects = GameObject.FindGameObjectWithTag("UIObjectsHolder").GetComponent<UIObjectsHolder>();
    }



    #region Interaction Methods

    public override void OnInteract()
    {
        Interaction();
    }

    public override void Interaction()
    {
        if (audioSource) PlayRandomAudio();

        InventoryManager.OnChestClose += CloseChest;
        InventoryManager.OnItemMoveEnd += OnItemMoveEnd;

        inventoryManager.GetUIElementsForChest(uiObjects.chestInventoryObject, uiObjects.chestInventoryCursor, uiObjects.chestPlayerSlotsHolder, uiObjects.chestSlotsHolder);
        inventoryManager.OpenChest(inventory, inventory.maxSize, gameObject, false);
    }

    #endregion Interaction Methods



    protected virtual void OnItemMoveEnd(ItemClass _item)
    {
        plantGrow.CheckForSeed();
    }

    protected virtual void CloseChest(InventoryManager _inventoryManager)
    {
        InventoryManager.OnChestClose -= CloseChest;
        InventoryManager.OnItemMoveEnd -= OnItemMoveEnd;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableChest : InteractableBase
{
    protected Inventory inventory;

    protected InventoryManager inventoryManager;

    protected UIObjectsHolder uiObjects;

    private Animator anim;





    public override void Start()
    {
        inventory = GetComponent<Inventory>();
        anim = GetComponent<Animator>();
        inventoryManager = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryManager>();
        base.Start();
        uiObjects = GameObject.FindGameObjectWithTag("UIObjectsHolder").GetComponent<UIObjectsHolder>();
    }

    public override void OnInteract()
    {
        Interaction();
    }

    public override void Interaction()
    {
        if (audioSource) PlayRandomAudio();

        InventoryManager.OnChestClose += CloseChest;

        anim.Play("OpenChest");
        inventoryManager.GetUIElementsForChest(uiObjects.chestInventoryObject, uiObjects.chestInventoryCursor, uiObjects.chestPlayerSlotsHolder, uiObjects.chestSlotsHolder);
        inventoryManager.OpenChest(inventory, inventory.maxSize, gameObject, false);
    }

    private void CloseChest(InventoryManager _inventoryManager)
    {
        anim.Play("CloseChest");
        InventoryManager.OnChestClose -= CloseChest;
    }
}

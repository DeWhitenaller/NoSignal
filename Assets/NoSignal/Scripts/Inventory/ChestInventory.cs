using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChestInventory : Inventory
{
    public ChestClass chestClass;



    private void Awake()
    {
        maxSize = chestClass.maxInventorySize;

        //creates an array which will contain the items
        items = new SlotClass[maxSize];

        for (int i = 0; i < items.Length; i++)
        {
            items[i] = new SlotClass();
        }
    }
}

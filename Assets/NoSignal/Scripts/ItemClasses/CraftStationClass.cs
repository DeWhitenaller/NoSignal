using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new CraftStation Class", menuName = "Item/CraftStation")]
public class CraftStationClass : StructureClass
{
    [SerializeField] 
    private CraftableItemClass[] craftableItems;

    public int maxInventorySize;



    public CraftableItemClass[] GetCraftableItems() { return craftableItems; }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new CraftableItem Class", menuName = "Craftable Item")]

public class CraftableItemClass : ScriptableObject
{
    [Header("Item")]
    public string itemName;

    public Sprite itemIcon;

    public ItemClass itemReference;



    [Header("Crafting")]
    [SerializeField] 
    private ItemClass[] itemsNeeded;
    
    [SerializeField] 
    private int[] amountsNeeded;

    [SerializeField] 
    private float craftingTime;

    [SerializeField] 
    private int amountPerCraft;




    public ItemClass GetItem() { return itemReference; }
    public ItemClass[] GetNeededItems() { return itemsNeeded; }
    public int[] GetNeededAmounts() { return amountsNeeded; }
    public float GetCraftingTime() { return craftingTime; }
    public int GetAmountPerCraft() { return amountPerCraft; }
}

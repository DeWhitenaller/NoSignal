using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeSlotClass : SlotClass
{
    /// <summary>
    /// this is a special slot that gets used by traders 
    /// you can click on these types of slots to see requirements for a trade
    /// </summary>
     


    [SerializeField] 
    private ItemClass itemNeeded;
    
    [SerializeField] 
    private int amountNeeded;




    public ItemClass GetNeededItem() { return itemNeeded; }

    public int GetNeededAmount() { return amountNeeded; }

    public void SetNeededItem(ItemClass _item) { itemNeeded = _item; }

    public void SetNeededAmount(int _amount) { amountNeeded = _amount; }
}

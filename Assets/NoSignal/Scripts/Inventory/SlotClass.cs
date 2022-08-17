using System.Collections;
using UnityEngine;

[System.Serializable]
public class SlotClass
{
    /// <summary>
    /// this class represents a slot of an inventory
    /// it provides easy access to the inventory slots and you can add an item to that slot, add quantity to the item in that slot and so on
    /// </summary>
    
    
    
    [SerializeField] 
    private ItemClass item;
    
    [SerializeField] 
    private int quantity;
    
    
    private bool isFromPlayer;




    public SlotClass()
    {
        item = null;
        quantity = 0;
    }

    public SlotClass(ItemClass _item, int _quantity)
    {
        item = _item;
        quantity = _quantity;
    }

    public SlotClass(SlotClass slot)
    {
        this.item = slot.GetItem();
        this.quantity = slot.GetQuantity();
    }

    public SlotClass(ItemClass _item, int _quantity, bool _isFromPlayer)
    {
        this.item = _item;
        this.quantity = _quantity;
        this.isFromPlayer = _isFromPlayer;
    }

    public void Clear()
    {
        this.item = null;
        this.quantity = 0;
    }

    
    public ItemClass GetItem() { return item; }

    public int GetQuantity() { return quantity; }

    public void AddQuantity(int _quantity) { quantity += _quantity; }

    public void SubQuantity(int _quantity) { quantity -= _quantity; }

    public void AddItem(ItemClass item, int quantity)
    {
        this.item = item;
        this.quantity = quantity;
    }

    public void SetisFromPlayer(bool _isFromPlayer) { this.isFromPlayer = _isFromPlayer; }
    public bool GetisFromPlayer() { return isFromPlayer; }
}

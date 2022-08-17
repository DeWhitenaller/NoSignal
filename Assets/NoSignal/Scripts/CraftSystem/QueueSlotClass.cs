using System.Collections;
using UnityEngine;

[System.Serializable]
public class QueueSlotClass
{
    [SerializeField] 
    private CraftableItemClass craftableItem;

    [SerializeField] 
    private int quantity;

    private bool isFromPlayer;




    public QueueSlotClass()
    {
        this.craftableItem = null;
        this.quantity = 0;
    }

    public QueueSlotClass(CraftableItemClass _item, int _quantity)
    {
        this.craftableItem = _item;
        this.quantity = _quantity;
    }

    public QueueSlotClass(QueueSlotClass slot)
    {
        this.craftableItem = slot.GetItem();
        this.quantity = slot.GetQuantity();
    }

    public QueueSlotClass(CraftableItemClass _item, int _quantity, bool _isFromPlayer)
    {
        this.craftableItem = _item;
        this.quantity = _quantity;
        this.isFromPlayer = _isFromPlayer;
    }

    public void Clear()
    {
        this.craftableItem = null;
        this.quantity = 0;
    }

    public CraftableItemClass GetItem() { return craftableItem; }

    public int GetQuantity() { return quantity; }

    public void AddQuantity(int _quantity) { this.quantity += _quantity; }

    public void SubQuantity(int _quantity) { this.quantity -= _quantity; }

    public void AddItem(CraftableItemClass _craftableItem, int _quantity)
    {
        this.craftableItem = _craftableItem;
        this.quantity = _quantity;
    }

    public void SetisFromPlayer(bool _isFromPlayer) { this.isFromPlayer = _isFromPlayer; }

    public bool GetisFromPlayer() { return isFromPlayer; }
}

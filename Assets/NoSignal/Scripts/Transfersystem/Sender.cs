using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sender : MonoBehaviour
{
    /// <summary>
    /// this class contains the logic of the Sender that gets accessed by the SenderConnector
    /// </summary>



    [SerializeField] 
    private List<Receiver> connectedReceivers;

    [SerializeField] 
    private List<Receiver> receiverList;
    
    private SenderAndReceiverList senderAndReceiver;

    public List<Amplifier> connectedAmplifiers;
    
    
    
    public int channel;




    #region Unity Methods

    private void Start()
    {
        senderAndReceiver = GameObject.FindGameObjectWithTag("SenderAndReceiverList").GetComponent<SenderAndReceiverList>();
        senderAndReceiver.senderList.Add(this);
        CheckForReceiversOnSameChannel();
    }

    private void OnEnable()
    {
        Receiver.OnChannelSet += CheckForReceiversOnSameChannel;
    }

    private void OnDisable()
    {
        Receiver.OnChannelSet -= CheckForReceiversOnSameChannel;
        senderAndReceiver.senderList.Remove(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchChannel(1);
            CheckForReceiversOnSameChannel();
        }
    }

    #endregion Unity Methods




    #region Channel Functionality

    private void SwitchChannel(int _channelToSwitchTo)
    {
        channel = _channelToSwitchTo;
    }

    public void CheckForReceiversOnSameChannel()
    {
        receiverList = senderAndReceiver.receiverList;
        connectedReceivers.Clear();

        for (int i = 0; i < receiverList.Count; i++)
        {
            if(receiverList[i].channel == channel)
            {
                connectedReceivers.Add(receiverList[i]);
            }
        }
    }

    #endregion Channel Functionality




    #region Access Inventory

    public bool CheckReceiverInventories(SlotClass _itemSlot, out SlotClass _updatedItemSlot)
    {
        SlotClass newSlot = new SlotClass();

        for (int x = 0; x < connectedReceivers.Count; x++)
        {
            for (int i = 0; i < connectedReceivers[x].connectedInventories.Count; i++)
            {
                TryToStoreItemsInReceiverInventory(connectedReceivers[x].connectedInventories[i], _itemSlot, out _itemSlot);

                if(_itemSlot.GetQuantity() == 0)
                {
                    newSlot = new SlotClass(_itemSlot.GetItem(), 0);
                    _updatedItemSlot = newSlot;

                    return true;
                }
            }
        }

        newSlot = new SlotClass(_itemSlot.GetItem(), _itemSlot.GetQuantity());
        _updatedItemSlot = newSlot;

        return false;
    }

    public void TryToStoreItemsInReceiverInventory(Inventory _inventory, SlotClass _itemSlot, out SlotClass _updatedItemSlot)
    {
        for (int i = 0; i < _inventory.items.Length; i++)
        {
            if (_inventory.items[i].GetItem() == _itemSlot.GetItem())
            {
                if(_inventory.items[i].GetItem() == null)
                {
                    Debug.Log("lol");
                }
            }
        }

        for (int i = 0; i < _inventory.items.Length; i++)
        {
            if (_inventory.items[i].GetItem() == _itemSlot.GetItem())
            {
                int amountLeftInSlot = _inventory.items[i].GetItem().maxStackSize - _inventory.items[i].GetQuantity();

                if (_itemSlot.GetQuantity() > amountLeftInSlot)
                {
                    //store as much as you can and continue with the next inventory slot
                    _inventory.items[i].AddQuantity(amountLeftInSlot);
                    _itemSlot.SubQuantity(amountLeftInSlot);
                }
                else
                {
                    //store all items and clear the slot
                    _inventory.items[i].AddQuantity(_itemSlot.GetQuantity());
                    _itemSlot.SubQuantity(_itemSlot.GetQuantity());
                }
            }
            else if (_inventory.items[i].GetItem() == null)
            {
                if (_itemSlot.GetQuantity() > _itemSlot.GetItem().maxStackSize)
                {
                    _inventory.items[i].AddItem(_itemSlot.GetItem(), _itemSlot.GetItem().maxStackSize);
                    _itemSlot.SubQuantity(_itemSlot.GetItem().maxStackSize);
                }
                else
                {
                    _inventory.items[i].AddItem(_itemSlot.GetItem(), _itemSlot.GetQuantity());
                    _itemSlot.SubQuantity(_itemSlot.GetQuantity());
                }
            }

            if (_itemSlot.GetQuantity() == 0)
            {
                break;
            }
        }

        if(_itemSlot.GetQuantity() != 0)
        {
            SlotClass newSlot = new SlotClass(_itemSlot.GetItem(), _itemSlot.GetQuantity());
            _updatedItemSlot = newSlot;
        }
        else
        {
            SlotClass newSlot = new SlotClass(_itemSlot.GetItem(), 0);
            _updatedItemSlot = newSlot;
        }
    }

    #endregion Access Inventory




    private void OnDestroy()
    {
        for (int i = 0; i < connectedAmplifiers.Count; i++)
        {
            connectedAmplifiers[i].Disconnect();
        }
    }
}

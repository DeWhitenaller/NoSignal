using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableReceiver : InteractableBase
{
    private InventoryManager inventoryManager;

    private UIObjectsHolder uiObjectsHolder;

    private Receiver receiver;




    public override void Start()
    {
        base.Start();
        inventoryManager = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryManager>();
        uiObjectsHolder = GameObject.FindGameObjectWithTag("UIObjectsHolder").GetComponent<UIObjectsHolder>();
        receiver = GetComponentInChildren<Receiver>();
    }


    public override void Interaction()
    {
        UIEvents.OnSetChannel += SetChannel;
        UIEvents.OnIncrease += Increase;
        UIEvents.OnDecrease += Decrease;

        InventoryManager.OnChestClose += CloseReceiver;

        inventoryManager.OpenSender();

        uiObjectsHolder.senderChannelText.text = receiver.channel.ToString();
    }


    public virtual void CloseReceiver(InventoryManager _inventoryManager)
    {
        UIEvents.OnSetChannel -= SetChannel;
        UIEvents.OnIncrease -= Increase;
        UIEvents.OnDecrease -= Decrease;

        InventoryManager.OnChestClose -= CloseReceiver;
    }


    public void Increase()
    {
        receiver.channel++;
        uiObjectsHolder.senderChannelText.text = receiver.channel.ToString();
    }


    public void Decrease()
    {
        if (receiver.channel == 1) return;

        receiver.channel--;
        uiObjectsHolder.senderChannelText.text = receiver.channel.ToString();
    }


    public void SetChannel()
    {
        receiver.SetChannel();
    }

}

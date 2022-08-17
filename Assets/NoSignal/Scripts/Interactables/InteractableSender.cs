using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableSender : InteractableBase
{
    private InventoryManager inventoryManager;

    private UIObjectsHolder uiObjectsHolder;

    private Sender sender;





    public override void Start()
    {
        base.Start();
        inventoryManager = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryManager>();
        uiObjectsHolder = GameObject.FindGameObjectWithTag("UIObjectsHolder").GetComponent<UIObjectsHolder>();
        sender = GetComponentInChildren<Sender>();
    }


    public override void Interaction()
    {
        UIEvents.OnSetChannel += SetChannel;
        UIEvents.OnIncrease += Increase;
        UIEvents.OnDecrease += Decrease;

        InventoryManager.OnChestClose += CloseSender;

        inventoryManager.OpenSender();

        uiObjectsHolder.senderChannelText.text = sender.channel.ToString();
    }


    public virtual void CloseSender(InventoryManager _inventoryManager)
    {
        UIEvents.OnSetChannel -= SetChannel;
        UIEvents.OnIncrease -= Increase;
        UIEvents.OnDecrease -= Decrease;

        InventoryManager.OnChestClose -= CloseSender;
    }


    public void Increase()
    {
        sender.channel++;
        uiObjectsHolder.senderChannelText.text = sender.channel.ToString();
    }


    public void Decrease()
    {
        if (sender.channel == 1) return;

        sender.channel--;
        uiObjectsHolder.senderChannelText.text = sender.channel.ToString();
    }


    public void SetChannel()
    {
        sender.CheckForReceiversOnSameChannel();
    }

}

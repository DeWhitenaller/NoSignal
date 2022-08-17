using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableNPC : InteractableBase
{
    [SerializeField] 
    protected ItemClass[] tradeItems;

    protected TradableItemList tradableItemList;

    protected InventoryManager inventoryManager;

    protected UIObjectsHolder uiObjects;

    protected TradeSlotClass[] tradeSlots;





    public override void Start()
    {
        base.Start();
        inventoryManager = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryManager>();
        uiObjects = GameObject.FindGameObjectWithTag("UIObjectsHolder").GetComponent<UIObjectsHolder>();
        tradableItemList = GameObject.FindGameObjectWithTag("TradableItemList").GetComponent<TradableItemList>();

        tradeItems = new ItemClass[4];
        GetRandomTradeItems();

        tradeSlots = new TradeSlotClass[4];

        for (int i = 0; i < tradeSlots.Length; i++)
        {
            tradeSlots[i] = new TradeSlotClass();
        }

        for (int i = 0; i < tradeItems.Length; i++)
        {
            tradeSlots[i].AddItem(tradeItems[i], 0);
        }

        GetRandomItemsNeededForTrade();
    }

    public virtual void GetRandomTradeItems()
    {
        for (int i = 0; i < 4; i++)
        {
            int random = Random.Range(0, tradableItemList.tradableItemList.Count);
            tradeItems[i] = tradableItemList.tradableItemList[random];
        }
    }

    public virtual void GetRandomItemsNeededForTrade()
    {
        for (int i = 0; i < 4; i++)
        {
            int random = Random.Range(0, tradableItemList.tradableItemList.Count);
            tradeSlots[i].SetNeededItem(tradableItemList.tradableItemList[random]);

            while (tradeSlots[i].GetNeededItem() == tradeSlots[i].GetItem())
            {
                random = Random.Range(0, tradableItemList.tradableItemList.Count);
                tradeSlots[i].SetNeededItem(tradableItemList.tradableItemList[random]);
            }

            CheckItemType(tradeSlots[i].GetNeededItem(), i);
        }
    }

    public virtual void CheckItemType(ItemClass _item, int _tradeSlotIndex)
    {
        if(tradeSlots[_tradeSlotIndex].GetItem() == _item)
        {
            tradeSlots[_tradeSlotIndex].SetNeededAmount(1);
        }
        else
        {
            switch (_item.type)
            {
                case ItemClass.State.Tool:
                    tradeSlots[_tradeSlotIndex].SetNeededAmount(1);
                    break;

                case ItemClass.State.Misc:
                    tradeSlots[_tradeSlotIndex].SetNeededAmount(Random.Range(2, 4));
                    break;

                case ItemClass.State.Consumable:
                    tradeSlots[_tradeSlotIndex].SetNeededAmount(Random.Range(2, 6));
                    break;

                case ItemClass.State.Structure:
                    tradeSlots[_tradeSlotIndex].SetNeededAmount(1);
                    break;

                case ItemClass.State.Resource:
                    tradeSlots[_tradeSlotIndex].SetNeededAmount(Random.Range(2, 6));
                    break;

                default: Debug.LogError("Unknown Item Type");
                    break;
            }
        }
    }

    public override void Interaction()
    {
        if (audioSource) PlayRandomAudio();

        inventoryManager.GetUIElementsForNPC(tradeSlots, uiObjects.npcInventoryObject, uiObjects.npcInventoryCursor, uiObjects.npcPlayerSlotsHolder, uiObjects.tradableSlotsHolder, uiObjects.tradableSlotSelector, uiObjects.npcRequirementSlotsHolder, uiObjects.tradeAmountText);
        inventoryManager.OpenNPCTrading(gameObject);
    }
}

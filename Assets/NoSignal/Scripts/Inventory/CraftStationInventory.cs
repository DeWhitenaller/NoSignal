using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftStationInventory : Inventory
{
    //events
    public delegate void Action(CraftStationInventory _craftStationInventory);

    public static Action OnItemCraftFinish;

    public static Action OnItemQueueFailed;

    public static Action OnCraftableItemsCheck;





    public CraftStationClass craftStationClass;

    [HideInInspector]
    public CraftableItemClass[] craftableItems;

    public QueueSlotClass[] craftingQueue;

    public ItemClass itemInProgress;





    [HideInInspector]
    public GameObject[] queueSlots;
    



    [HideInInspector]
    public float craftTimer;
    
    public Slider craftProgressBar;

    private AudioSource audioSource;




    public List<SlotClass> neededItems;

    public List<SlotClass> summarizedInventoryItems;

    public List<SlotClass> neededItemsCopy;

    public List<SlotClass> sumInventoryItemsCopy;




    [HideInInspector]
    public bool craftingStart;

    [Header("Only Shown for Debugging")]
    public bool craftStationOpened = false;
    public bool inventoryFull = false; 
    public bool ressourcesMissing = false;





    #region Unity Methods
    public virtual void Awake()
    {
        maxSize = craftStationClass.maxInventorySize;

        //creates an array which will contain the items
        items = new SlotClass[maxSize];

        for (int i = 0; i < items.Length; i++)
        {
            items[i] = new SlotClass();
        }

        //creates an array which will contain the queued items
        craftingQueue = new QueueSlotClass[3];

        for (int i = 0; i < craftingQueue.Length; i++)
        {
            craftingQueue[i] = new QueueSlotClass();
        }


        craftableItems = craftStationClass.GetCraftableItems();

        craftingStart = false;
    }

    public virtual void Start()
    {
        if (!audioSource)
        {
            if (!TryGetComponent<AudioSource>(out audioSource))
            {
                Debug.LogError("AudioSource missing!", gameObject);
            }
        }
    }

    private void Update()
    {
        if (craftingStart)
        {
            craftTimer += Time.deltaTime;

            if (craftStationOpened)
            {
                craftProgressBar.value = Mathf.InverseLerp(0f, craftingQueue[0].GetItem().GetCraftingTime(), craftTimer);
            }

            if(craftTimer >= craftingQueue[0].GetItem().GetCraftingTime())
            {
                craftingStart = false;
                craftTimer = 0f;

                CheckInventorySpace(craftingQueue[0].GetItem().GetAmountPerCraft());
            }
        }
    }

    #endregion Unity Methods




    #region Sounds
    private void PlaySound()
    {
        audioSource.Play();
    }

    private void StopSound()
    {
        audioSource.Stop();
    }

    #endregion Sounds




    #region Open/Close
    public void CloseCraftStation()
    {
        craftStationOpened = false;
    }

    public void OpenCraftStation(Slider _craftProgressBar)
    {
        craftStationOpened = true;
        craftProgressBar = _craftProgressBar;
        GetListOfLeftItems();
    }

    #endregion Open/Close




    #region Craft Process

    public void StartCrafting()
    {
        RemoveRequiredItems();
    }

    private void RemoveRequiredItems()
    {
        for (int i = 0; i < craftingQueue[0].GetItem().GetNeededItems().Length; i++)
        {
            ContainsList(craftingQueue[0].GetItem().GetNeededItems()[i], out List<SlotClass> _compatibleSlots);
            int quantityToRemove = craftingQueue[0].GetItem().GetNeededAmounts()[i];

            foreach (SlotClass slot in _compatibleSlots)
            {
                if (quantityToRemove >= slot.GetQuantity())
                {
                    quantityToRemove -= slot.GetQuantity();
                    slot.Clear();
                }
                else
                {
                    slot.SubQuantity(quantityToRemove);
                    quantityToRemove = 0;
                    break;
                }
            }

            if (quantityToRemove != 0)
            {
                ressourcesMissing = true;
                Debug.Log("Not enough ressources to craft this");
                CancelQueue(true);
                StopSound();
                return;
            }
        }

        ressourcesMissing = false;
        craftingStart = true;
        PlaySound();
        itemInProgress = craftingQueue[0].GetItem().GetItem();
    }

    private void CheckInventorySpace(int _amountPerCraft)
    {
        int amountPerCraft = _amountPerCraft;
        bool hasSpace = false;

        for (int i = 0; i < items.Length; i++)
        {
            if(items[i].GetItem() == itemInProgress)
            {
                if(items[i].GetQuantity() == 0)
                {
                    items[i].Clear();
                    continue;
                }

                int quantityLeft = items[i].GetItem().maxStackSize - items[i].GetQuantity();

                if(quantityLeft != 0)
                {
                    if(amountPerCraft <= quantityLeft)
                    {
                        items[i].AddQuantity(amountPerCraft);
                        hasSpace = true;
                        amountPerCraft = 0;
                        break;
                    }
                    else
                    {
                        Debug.Log("amount per craft is bigger than left size");
                        amountPerCraft -= quantityLeft;
                        items[i].AddQuantity(quantityLeft);
                    }
                }
            }
        }

        if(amountPerCraft != 0)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].GetItem() == null)
                {
                    items[i].AddItem(itemInProgress, amountPerCraft);
                    hasSpace = true;
                    _amountPerCraft = 0;
                    break;
                }
            }
        }

        if (!hasSpace)
        {
            Debug.Log("Not enough space in CraftStation to craft this item");
            inventoryFull = true;
            StopSound();
            return;
        }
        else
        {
            
            craftProgressBar.value = 0f;

            inventoryFull = false;
            craftingQueue[0].SubQuantity(1);
            ReorganizeQueue();
            itemInProgress = null;
            CraftNextItemInQueue();

            OnItemCraftFinish?.Invoke(this);

            if (craftStationOpened)
            {
                GetListOfLeftItems();
            }

            //craft process done!!!
        }
    }

    private void ReorganizeQueue()
    {
        if(craftingQueue[0].GetQuantity() == 0)
        {
            craftingQueue[0].Clear();

            QueueSlotClass temp = new QueueSlotClass();

            for (int x = 1; x < craftingQueue.Length; x++)
            {
                if(craftingQueue[x].GetItem() == null)
                {
                    break;
                }

                temp.AddItem(craftingQueue[x].GetItem(), craftingQueue[x].GetQuantity());

                craftingQueue[x].Clear();
                craftingQueue[x - 1].AddItem(temp.GetItem(), temp.GetQuantity());
            }
        }
    }

    private void CraftNextItemInQueue()
    {
        if(craftingQueue[0].GetItem() != null)
        {
            StartCrafting();
        }
        else
        {
            StopSound();
        }
    }

    public bool TryToPutItemInQueue(CraftableItemClass _item, int _craftAmount)
    {

        bool itemQueued = false;
        bool firstItemInQueue = false;

        if(craftingQueue[0].GetItem() == null)
        {
            firstItemInQueue = true;
        }

        for (int i = 0; i < queueSlots.Length; i++)
        {
            if (craftingQueue[i].GetItem() == _item)
            {
                craftingQueue[i].AddQuantity(_craftAmount);
                itemQueued = true;
                break;
            }
            else if (craftingQueue[i].GetItem() == null)
            {
                craftingQueue[i].AddItem(_item, _craftAmount);
                itemQueued = true;
                break;
            }
        }


        if (firstItemInQueue)
        {
            craftingStart = true;
            RemoveRequiredItems();
        }

        return itemQueued;
    }


    #endregion Craft Process




    #region Pre-Check if Item is craftable

    public void OnCraft(CraftableItemClass _item, int _craftAmount)
    {
        CheckIfItemIsCraftable(_item, _craftAmount);
    }

    private void CheckIfItemIsCraftable(CraftableItemClass _craftableItem, int _craftAmount)
    {
        SummarizeItemsInInventory();
        SummarizeNeededItemsForQueue();

        neededItemsCopy = neededItems;
        sumInventoryItemsCopy = summarizedInventoryItems;

        SubtractNeededItemsFromInventoryList(neededItems, out bool queueCanBeCrafted);

        if (!queueCanBeCrafted)
        {
            Debug.Log("Queue cannot be crafted");
            return;
        }

        //create a temporary list with the items that got requested from the player
        List<SlotClass> requestedItems = new List<SlotClass>();

        for (int i = 0; i < _craftableItem.GetNeededItems().Length; i++)
        {
            ItemClass currentNeededItem = _craftableItem.GetNeededItems()[i];
            int currentNeededItemAmount = _craftableItem.GetNeededAmounts()[i] * _craftAmount;
            AddNeededItem(currentNeededItem, currentNeededItemAmount, requestedItems);
        }

        //now subtract the requested items from the inventory list to find out if there are enough items left for crafting
        SubtractNeededItemsFromInventoryList(requestedItems, out bool craftRequestSucceeded);

        if (!craftRequestSucceeded)
        {
            Debug.Log("not enough ressources to craft " + _craftableItem.GetItem() + " x" + _craftAmount);
            return;
        }
        else
        {
            TryToPutItemInQueue(_craftableItem, _craftAmount);

            OnItemCraftFinish?.Invoke(this);
        }
    }

    private void SubtractNeededItemsFromInventoryList(List<SlotClass> _list, out bool _succeeded)
    {
        _succeeded = false;

        if (_list.Count == 0)
        {
            _succeeded = true;
            return;
        }


        for (int i = 0; i < _list.Count; i++)
        {
            bool itemFound = false;

            for (int x = 0; x < summarizedInventoryItems.Count; x++)
            {
                //Debug.Log("list reihe " + i + " und inventory reihe " + x);
                if (_list[i].GetItem() == sumInventoryItemsCopy[x].GetItem())
                {
                    //Debug.Log("found item");

                    //found the needed item in the inventory list
                    //now subtract the amount of that needed item from the inventory list
                    itemFound = true;
                    sumInventoryItemsCopy[x].SubQuantity(_list[i].GetQuantity());

                    if (sumInventoryItemsCopy[x].GetQuantity() < 0)
                    {
                        //Debug.Log("oh shit " + sumInventoryItemsCopy[x].GetQuantity());
                        _succeeded = false;
                        return;
                    }

                    break;
                }
            }

            if (!itemFound)
            {
                _succeeded = false;
                return;
            }
            else
            {
                _succeeded = true;
            }
        }
    }


    #endregion Pre-Check if Item is craftable




    #region Unused
    public void TryCraftAgain()
    {
        if (inventoryFull)
        {
            inventoryFull = false;
            CheckInventorySpace(craftingQueue[0].GetItem().GetAmountPerCraft());
        }
        else if (ressourcesMissing)
        {
            ressourcesMissing = false;
            StartCrafting();
        }
    }

    private void UpdateQueue()
    {
        Debug.Log("hi");
        if (craftingQueue[0].GetItem() == null) return;

        //go through every slot in the queue
        for (int currentSlot = 0; currentSlot < craftingQueue.Length; currentSlot++)
        {
            if (craftingQueue[currentSlot].GetItem() == null) return;

            bool canCraftQueueSlot = false;

            //check if all the items in that slot can be crafted
            if (CheckIfQueueSlotIsCraftable(craftingQueue[currentSlot].GetItem().GetItem(), currentSlot, neededItemsCopy, sumInventoryItemsCopy))
            {
                canCraftQueueSlot = true;
            }
            else
            {
                //destroy the rest of that slot and move to the next slot
                int amountToSub = craftingQueue[0].GetQuantity() - 1;
                craftingQueue[0].SubQuantity(amountToSub);

                for (int i = 1; i < craftingQueue.Length; i++)
                {
                    craftingQueue[i].Clear();
                }

                break;
            }
        }
    }

    private bool CheckIfItemInQueueIsCraftable(ItemClass _item, int queueSlotIndex, List<SlotClass> _neededItemsCopy, List<SlotClass> _sumInventoryItemsCopy)
    {
        //check through all needed items for crafting this item
        for (int x = 0; x < craftingQueue[queueSlotIndex].GetItem().GetNeededItems().Length; x++)
        {
            //go through the summarized inventory to find the needed item there
            for (int i = 0; i < _sumInventoryItemsCopy.Count; i++)
            {
                //if the items matched
                if(craftingQueue[queueSlotIndex].GetItem().GetNeededItems()[x] == _sumInventoryItemsCopy[i].GetItem())
                {
                    //find the needed item in the summarized needed item list as well, because we need to subtract from that
                    bool neededItemFound = false;
                    int neededItemIndex = 0;

                    //search that item in the summarized needed item list
                    for (int y = 0; y < _neededItemsCopy.Count; y++)
                    {
                        if(craftingQueue[queueSlotIndex].GetItem().GetNeededItems()[x] == _neededItemsCopy[y].GetItem())
                        {
                            neededItemFound = true;
                            neededItemIndex = y;
                            break;
                        }
                    }

                    //if we found that that item
                    if (neededItemFound)
                    {
                        _sumInventoryItemsCopy[i].SubQuantity(craftingQueue[queueSlotIndex].GetItem().GetNeededAmounts()[x]);

                        if (_sumInventoryItemsCopy[i].GetQuantity() < 0)
                        {
                            return false;
                        }
                    }
                }
            }
        }


        return true;
    }

    #endregion Unused




    #region SummarizeItems
    public void SummarizeNeededItemsForQueue()
    {
        //this gets all of the needed items for the complete queue

        neededItems = new List<SlotClass>();

        if (craftingQueue[0].GetItem() == null)
        {
            return;
        }
        else
        {
            //go through every slot in the queue
            for (int currentSlot = 0; currentSlot < craftingQueue.Length; currentSlot++)
            {
                if(craftingQueue[currentSlot].GetItem() == null)
                {
                    break;
                }
                else
                {
                    //go through every ingredient of the craft receipt in that queue slot
                    //add every ingredient to the needed items list and multiply it with the craft amount
                    for (int i = 0; i < craftingQueue[currentSlot].GetItem().GetNeededItems().Length; i++)
                    {
                        ItemClass currentNeededItem = craftingQueue[currentSlot].GetItem().GetNeededItems()[i];
                        int currentNeededItemAmount = craftingQueue[currentSlot].GetItem().GetNeededAmounts()[i] * craftingQueue[currentSlot].GetQuantity();
                        AddNeededItem(currentNeededItem, currentNeededItemAmount, neededItems);
                    }
                }
            }
        }
    }

    public void SummarizeNeededItems()
    {
        if (craftingQueue[0].GetItem() == null)
        {
            neededItems.Clear();
            return;
        }

        neededItems = new List<SlotClass>();

        for (int currentSlot = 0; currentSlot < craftingQueue.Length; currentSlot++)
        {
            if(craftingQueue[currentSlot].GetItem() == null)
            {
                break;
            }
            else
            {
                for (int i = 0; i < craftingQueue[currentSlot].GetItem().GetNeededItems().Length; i++)
                {
                    ItemClass currentNeededItem = craftingQueue[currentSlot].GetItem().GetNeededItems()[i];
                    int currentNeededItemAmount = craftingQueue[currentSlot].GetItem().GetNeededAmounts()[i] * craftingQueue[currentSlot].GetQuantity();
                    AddNeededItem(currentNeededItem, currentNeededItemAmount, neededItems);
                }
            }
        }
    }

    private void AddNeededItem(ItemClass _item, int _itemAmount, List<SlotClass> _list)
    {
        if(_list.Count != 0)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (_list[i].GetItem() == _item)
                {
                    _list[i].AddQuantity(_itemAmount);
                    break;
                }
                else
                {
                    SlotClass newSlot = new SlotClass();
                    newSlot.AddItem(_item, _itemAmount);
                    _list.Add(newSlot);
                    break;
                }
            }
        }
        else
        {
            SlotClass newSlot = new SlotClass();
            newSlot.AddItem(_item, _itemAmount);
            _list.Add(newSlot);
        }
    }

    public void SummarizeItemsInInventory()
    {
        summarizedInventoryItems = new List<SlotClass>();
        
        for (int i = 0; i < items.Length; i++)
        {
                
            if(items[i].GetItem() == null)
            {
                continue;
            }

            bool isInList = false;

            if(summarizedInventoryItems.Count == 0)
            {
                isInList = true;
                SlotClass newSlot = new SlotClass();
                newSlot.AddItem(items[i].GetItem(), items[i].GetQuantity());
                summarizedInventoryItems.Add(newSlot);
            }
            else
            {
                for (int x = 0; x < summarizedInventoryItems.Count; x++)
                {
                    if (items[i].GetItem() == summarizedInventoryItems[x].GetItem())
                    {
                        isInList = true;
                        summarizedInventoryItems[x].AddQuantity(items[i].GetQuantity());
                        break;
                    }
                }
            }

            if (!isInList)
            {
                SlotClass newSlot = new SlotClass();
                newSlot.AddItem(items[i].GetItem(), items[i].GetQuantity());
                summarizedInventoryItems.Add(newSlot);
            }
        }
    }

    public void GetListOfLeftItems()
    {
        SummarizeItemsInInventory();
        SummarizeNeededItemsForQueue();

        sumInventoryItemsCopy = summarizedInventoryItems;
        neededItemsCopy = neededItems;

        SubtractNeededItemsFromInventoryList(neededItems, out bool queueCanBeCrafted);
    }

    #endregion SummarizeItems




    public void CancelQueue(bool clearCurrentItem)
    {
        for (int i = 0; i < craftingQueue.Length; i++)
        {
            if(!clearCurrentItem && i == 0)
            {
                craftingQueue[i].SubQuantity(craftingQueue[i].GetQuantity() - 1);
            }
            else
            {
                craftingQueue[i].Clear();
            }
        }
    }


    private void ContainsList(ItemClass _compareItem, out List<SlotClass> _items)
    {
        _items = new List<SlotClass>();

        foreach (SlotClass slot in items)
        {
            if (slot.GetItem() == _compareItem)
            {
                _items.Add(slot);
            }
        }
    }


    public bool CheckIfQueueSlotIsCraftable(ItemClass _item, int queueSlotIndex, List<SlotClass> _neededItemsCopy, List<SlotClass> _sumInventoryItemsCopy)
    {
        //check through all needed items for crafting this item
        for (int x = 0; x < craftingQueue[queueSlotIndex].GetItem().GetNeededItems().Length; x++)
        {
            //go through the summarized inventory to find the needed item there
            for (int i = 0; i < _sumInventoryItemsCopy.Count; i++)
            {
                //if the items matched
                if(craftingQueue[queueSlotIndex].GetItem().GetNeededItems()[x] == _sumInventoryItemsCopy[i].GetItem())
                {
                    //find the needed item in the summarized needed item list as well, because we need to subtract from that
                    bool neededItemFound = false;
                    int neededItemIndex = 0;

                    //search that item in the summarized needed item list
                    for (int y = 0; y < _neededItemsCopy.Count; y++)
                    {
                        if(craftingQueue[queueSlotIndex].GetItem().GetNeededItems()[x] == _neededItemsCopy[y].GetItem())
                        {
                            neededItemFound = true;
                            neededItemIndex = y;
                            break;
                        }
                    }

                    //if we found that that item
                    if (neededItemFound)
                    {
                        _sumInventoryItemsCopy[i].SubQuantity(craftingQueue[queueSlotIndex].GetItem().GetNeededAmounts()[x] * craftingQueue[queueSlotIndex].GetQuantity());

                        if (_sumInventoryItemsCopy[i].GetQuantity() < 0)
                        {
                            CancelQueue(false);
                            return false;
                        }
                    }
                }
            }
        }


        return true;
    }


    public void GetMaxCraftAmountForRequestedItem(CraftableItemClass _craftableItem)
    {
        GetListOfLeftItems();

        List<SlotClass> requestedItems = new List<SlotClass>();



        int maxCraftAmount = 0;
        bool firstCheck = true;

        for (int i = 0; i < _craftableItem.GetNeededItems().Length; i++)
        {
            for (int x = 0; x < sumInventoryItemsCopy.Count; x++)
            {
                if(_craftableItem.GetNeededItems()[i] == sumInventoryItemsCopy[x].GetItem())
                {
                    //items matched
                    int tempMaxCraftAmount = sumInventoryItemsCopy[x].GetQuantity() / _craftableItem.GetNeededAmounts()[i];

                    if(tempMaxCraftAmount == 0)
                    {
                        return;
                    }

                    if (firstCheck)
                    {
                        firstCheck = false;
                        maxCraftAmount = tempMaxCraftAmount;
                    }
                    else
                    {
                        if(tempMaxCraftAmount <= maxCraftAmount)
                        {
                            maxCraftAmount = tempMaxCraftAmount;
                        }
                    }
                }
            }
        }


        for (int i = 0; i < _craftableItem.GetNeededItems().Length; i++)
        {
            ItemClass currentNeededItem = _craftableItem.GetNeededItems()[i];
            int currentNeededItemAmount = _craftableItem.GetNeededAmounts()[i] * maxCraftAmount;
            AddNeededItem(currentNeededItem, currentNeededItemAmount, requestedItems);
        }

        //now subtract the requested items from the inventory list to find out if there are enough items left for crafting
        SubtractNeededItemsFromInventoryList(requestedItems, out bool craftRequestSucceeded);

        if (!craftRequestSucceeded)
        {
            Debug.Log("not enough ressources to craft " + _craftableItem.GetItem() + " x" + maxCraftAmount);
            return;
        }
        else
        {
            TryToPutItemInQueue(_craftableItem, maxCraftAmount);

            OnItemCraftFinish?.Invoke(this);
        }
    }
}
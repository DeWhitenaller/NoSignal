using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("Connected Scripts")]
    [SerializeField] private PlayerCraftStation playerInventory;
    [SerializeField] private CraftStationManager craftStationManager;
    [SerializeField] private BuildSystem buildSystem;
    [SerializeField] private EquipItemHandler equipItemHandler;

    [Header("UI Inventory Screens")]
    [SerializeField] private GameObject playerInventoryUIObject;
    [SerializeField] private GameObject sharedInventoryUIObject;
    [SerializeField] private GameObject craftStationUIObject;
    [SerializeField] private GameObject activeInventoryUIObject;

    [Header("UI Inventory Cursors")]
    [SerializeField] private GameObject playerInventoryCursor;
    [SerializeField] private GameObject sharedInventoryCursor;
    [SerializeField] private GameObject craftStationCursor;
    private GameObject itemCursor;

    [Header("UI Playerslots Holders")]
    [SerializeField] private GameObject playerInventoryPlayerSlotHolder;
    [SerializeField] private GameObject sharedInventoryPlayerSlotHolder;
    [SerializeField] private GameObject craftStationPlayerSlotHolder;

    [Header("UI Chestslots Holders")]
    [SerializeField] private GameObject sharedInventoryChestSlotHolder;
    [SerializeField] private GameObject craftStationChestSlotHolder;
    private GameObject playerSlotHolder;
    private GameObject chestSlotHolder;
    
    [Header("UI Hotbar")]
    [SerializeField] private GameObject hotbarSlotHolder;
    [SerializeField] private GameObject hotbarSelector;

    [Header("UI CraftStation")]
    [SerializeField] private Slider craftProgressBar;
    [SerializeField] private Slider playerCraftProgressBar;

    [Header("CraftStationManager UI GameObjects")]
    [SerializeField] private GameObject craftSlotHolder;
    [SerializeField] private GameObject craftingQueueSlotsHolder;
    [SerializeField] private GameObject craftSlotSelector;
    [SerializeField] private GameObject requirementSlotsHolder;
    [SerializeField] private TextMeshProUGUI craftAmountText;

    [Header("CraftStationManager Player UI GameObjects")]
    [SerializeField] private GameObject playercraftSlotHolder;
    [SerializeField] private GameObject playercraftingQueueSlotsHolder;
    [SerializeField] private GameObject playercraftSlotSelector;
    [SerializeField] private GameObject playerrequirementSlotsHolder;
    [SerializeField] private TextMeshProUGUI playercraftAmountText;

    [Header("Trading UI")]
    [SerializeField] private GameObject[] tradeSlotObjects;
    [SerializeField] private TradeSlotClass[] tradeSlots;
    [SerializeField] private GameObject[] tradeRequirementSlots;
    [SerializeField] private GameObject tradeButtons;
    private TradeSlotClass currentTradeSlot;
    private int tradeAmount;

    [Header("SenderUI")]
    [SerializeField] private GameObject senderUI;

    [Header("Player Objects")]
    [SerializeField] GameObject playerItemHolder;

    [Header("Design")]
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color normalColor;

    [Header("Player")]
    [SerializeField] GameObject cineMachineObject;
    [SerializeField] PlayerMovement playerMovement;

    [Header("Test purposes")]
    [SerializeField] private SlotClass[] startingItems;


    //the slots the mouse hovers over
    private GameObject currentSlot;
    private GameObject oldCurrentSlot;

    //the chest or craft station that gets opened
    private CraftStationInventory craftStationInventory;
    private Inventory chestInventory;
    private GameObject chest;

    //the UI GameObjects that represent the slots
    private GameObject[] playerSlots;
    private GameObject[] chestSlots;
    private GameObject[] hotbarSlots;

    //temporary slots that are not shown anywhere
    //they hold information about dragged as long as you are dragging them (for example)
    private SlotClass movingSlot;
    private SlotClass tempSlot;
    private SlotClass originalSlot;

    //is currently dragging an item?
    private bool isMovingItem;
    public bool npcTradingIsActive;

    //the slot that is selected on the hotbar
    private int selectedSlotIndex;

    //the actual item of the selected slot on the hotbar
    private ItemClass selectedItem;

    public delegate void Action(InventoryManager _inventoryManager);
    public static event Action OnChestOpen;
    public static event Action OnChestClose;
    public static event Action OnCraftStationClose;

    public delegate void ClickAction(ItemClass _item);
    public static event ClickAction OnItemMoveEnd;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip blopSound, openSound;

    #region Unity Methods

    private void Awake()
    {
        activeInventoryUIObject = playerInventoryUIObject;
    }

    private void OnEnable()
    {
        CraftStationInventory.OnItemCraftFinish += RefreshCurrentUI;
    }

    public void Start()
    {
        playerSlotHolder = playerInventoryPlayerSlotHolder;
        itemCursor = playerInventoryCursor;

        InitializePlayerSlots();

        //set the hotbar playerSlots array's size
        hotbarSlots = new GameObject[hotbarSlotHolder.transform.childCount];

        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            hotbarSlots[i] = hotbarSlotHolder.transform.GetChild(i).gameObject;
        }



        RefreshUI();
        //AddItem(itemToAdd, 200);
        //RemoveItem(itemToRemove);

        StartCoroutine(WaitForNextFrame());
    }

    private void Update()
    {
        //open inventory
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (activeInventoryUIObject.activeInHierarchy)
            {
                if(activeInventoryUIObject == sharedInventoryUIObject)
                {
                    CloseChest();
                }
                else if (npcTradingIsActive)
                {
                    CloseNPCTrading();
                }
                else if(activeInventoryUIObject == craftStationUIObject)
                {
                    activeInventoryUIObject.SetActive(false);
                    craftStationManager.enabled = false;
                    OnCraftStationClose?.Invoke(this);
                }
                else if(activeInventoryUIObject == playerInventoryUIObject)
                {
                    activeInventoryUIObject.SetActive(false);
                }
                else if(activeInventoryUIObject == senderUI)
                {
                    CloseSender();
                }
            }
            else
            {
                OpenPlayerInventory();
            }

            cineMachineObject.SetActive(!activeInventoryUIObject.activeInHierarchy);
            playerMovement.movementDisabled = activeInventoryUIObject.activeInHierarchy;
        }

        CheckForMouseScroll();


        if (activeInventoryUIObject.activeInHierarchy)
        {
            HighlightCurrentSlot();

            if (Input.GetKeyDown(KeyCode.G))
            {
                DropCurrentItem();
            }

            if (isMovingItem)
            {
                itemCursor.GetComponent<Image>().sprite = movingSlot.GetItem().itemIcon;
                itemCursor.transform.position = Input.mousePosition;
            }

            CheckForMouseClick();
        }
    }

    private void OnDisable()
    {
        CraftStationInventory.OnItemCraftFinish -= RefreshCurrentUI;
    }

    #endregion Unity Methods

    private void InitializePlayerSlots()
    {
        //set the size of the arrays - this causes empty entries at this moment
        playerSlots = new GameObject[playerInventory.maxSize];

        //hide UI Slots so that the player can only use as many slots as his inventory size
        for (int i = playerSlotHolder.transform.childCount - 1; i > playerInventory.maxSize - 1; i--)
        {
            playerSlotHolder.transform.GetChild(i).gameObject.SetActive(false);
        }

        //assign the UI slot gameobjects to the playerSlots array
        for (int i = 0; i < playerInventory.maxSize; i++)
        {
            playerSlots[i] = playerSlotHolder.transform.GetChild(i).gameObject;
        }

        //fill those SLotClasses with starter Items
        for (int i = 0; i < startingItems.Length; i++)
        {
            playerInventory.items[i] = startingItems[i];
        }
    }

    private IEnumerator WaitForNextFrame()
    {
        yield return new WaitForEndOfFrame();

        hotbarSelector.transform.position = hotbarSlotHolder.transform.GetChild(0).transform.position;
        selectedItem = playerInventory.items[selectedSlotIndex].GetItem();
    }



    #region Chest Stuff
    public void OpenChest(Inventory _chestInventory, int chestSize, GameObject _chest, bool _isCraftStation)
    {
        if (activeInventoryUIObject.activeInHierarchy) return;

        cineMachineObject.SetActive(false);
        playerMovement.DisableMovement();

        OnChestOpen?.Invoke(this);

        chest = _chest;

        if (_isCraftStation)
        {
            craftStationManager.enabled = true;
            OpenCraftStation();
        }
        else
        {
            OpenSharedInventory();
        }

        activeInventoryUIObject.SetActive(true);

        for (int i = 0; i < playerInventory.maxSize; i++)
        {
            playerSlots[i] = playerSlotHolder.transform.GetChild(i).gameObject;
        }

        for (int i = playerSlotHolder.transform.childCount - 1; i > 0; i--)
        {
            if (i > playerInventory.maxSize - 1)
            {
                playerSlotHolder.transform.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                playerSlotHolder.transform.GetChild(i).gameObject.SetActive(true);
            }
        }



        chestInventory = _chestInventory;

        chestSlots = new GameObject[chestSize];

        for (int i = chestSlotHolder.transform.childCount - 1; i > 0; i--)
        {
            if(i > chestSize - 1)
            {
                chestSlotHolder.transform.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                chestSlotHolder.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < chestSize; i++)
        {
            chestSlots[i] = chestSlotHolder.transform.GetChild(i).gameObject;
        }

        RefreshUI();
        //chestInventory.items = new SlotClass[chestSlots.Length];
    }

    public void OpenNPCTrading(GameObject _npcObject)
    {
        if (activeInventoryUIObject.activeInHierarchy) return;

        cineMachineObject.SetActive(false);
        playerMovement.DisableMovement();

        OnChestOpen?.Invoke(this);

        npcTradingIsActive = true;

        activeInventoryUIObject = craftStationUIObject;

        activeInventoryUIObject.SetActive(true);

        for (int i = 0; i < playerInventory.maxSize; i++)
        {
            playerSlots[i] = craftStationPlayerSlotHolder.transform.GetChild(i).gameObject;
        }

        for (int i = craftStationPlayerSlotHolder.transform.childCount - 1; i > 0; i--)
        {
            if (i > playerInventory.maxSize - 1)
            {
                craftStationPlayerSlotHolder.transform.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                craftStationPlayerSlotHolder.transform.GetChild(i).gameObject.SetActive(true);
            }
        }


        RefreshUI();
        //chestInventory.items = new SlotClass[chestSlots.Length];
    }

    public void CloseNPCTrading()
    {
        npcTradingIsActive = false;
        currentTradeSlot = null;

        foreach (var item in tradeRequirementSlots)
        {
            item.SetActive(false);
        }

        tradeButtons.SetActive(false);
        activeInventoryUIObject.SetActive(false);

        activeInventoryUIObject = playerInventoryUIObject;
    }

    public void OpenSender()
    {
        cineMachineObject.SetActive(false);
        playerMovement.DisableMovement();

        activeInventoryUIObject = senderUI;
        senderUI.SetActive(true);
    }
    public void CloseSender()
    {
        OnChestClose?.Invoke(this);
        senderUI.SetActive(false);
    }

    public void CloseChest()
    {
        OnChestClose?.Invoke(this);

        if (activeInventoryUIObject == craftStationUIObject && craftStationInventory != null)
        {
            craftStationManager.enabled = false;
            craftStationInventory.CloseCraftStation();
            craftStationInventory = null;
        }

        chest = null;
        //sharedInventoryUIObject.SetActive(true);
        //activeInventoryUIObject = sharedInventoryUIObject;

        chestInventory = null;

        chestSlots = null;

        for (int i = chestSlotHolder.transform.childCount - 1; i >= 0; i--)
        {
            chestSlotHolder.transform.GetChild(i).gameObject.SetActive(true);
        }

        activeInventoryUIObject.SetActive(false);
        RefreshUI();
    }

    private void ContainsListInChest(ItemClass _compareItem, out List<SlotClass> _items)
    {
        _items = new List<SlotClass>();
        foreach (SlotClass slot in chestInventory.items)
        {
            if (slot.GetItem() == _compareItem)
            {
                _items.Add(slot);
            }
        }
    }

    private void EmptySlotListInChest(out List<SlotClass> _items)
    {
        _items = new List<SlotClass>();

        foreach (SlotClass slot in chestInventory.items)
        {
            if (slot.GetItem() == null)
            {
                _items.Add(slot);
            }
        }
    }

    public SlotClass ContainsInChest(ItemClass item)
    {
        foreach (SlotClass slot in chestInventory.items)
        {
            if (slot.GetItem() == item)
            {
                return slot;
            }
        }

        return null;
    }

    public SlotClass EmptySlotInChest()
    {
        foreach (SlotClass slot in chestInventory.items)
        {
            if (slot.GetItem() == null)
            {
                return slot;
            }
        }

        return null;
    }


    private void RefreshChestUI()
    {
        for (int i = 0; i < chestSlots.Length; i++)
        {
            try
            {
                //every slot has a child with an Image component to represent whether there's an item on that slot or not
                //every slot also has a child with a Text component to represent the amount of playerInventory.items in that slot
                chestSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                chestSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = chestInventory.items[i].GetItem().itemIcon;

                if (chestInventory.items[i].GetQuantity() > 1)
                {
                    chestSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = chestInventory.items[i].GetQuantity().ToString();
                }
                else
                {
                    chestSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
                }
            }
            catch
            {
                chestSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                chestSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                chestSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
        }

        if(craftStationInventory != null)
        {
            //craftStationInventory.TryCraftAgain();
        }
    }


    #endregion Chest Stuff




    #region NPC Trading

    public void ShowTradingRequirements()
    {
        
    }

    #endregion NPC Trading




    #region Inventory Utilities
    public void RefreshUI()
    {
        for (int i = 0; i < playerInventory.maxSize; i++)
        {
            try
            {
                //every slot has a child with an Image component to represent whether there's an item on that slot or not
                //every slot also has a child with a Text component to represent the amount of playerInventory.items in that slot
                playerSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                playerSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = playerInventory.items[i].GetItem().itemIcon;
                if (playerInventory.items[i].GetQuantity() > 1)
                {
                    playerSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = playerInventory.items[i].GetQuantity().ToString();
                }
                else
                {
                    playerSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
                }
            }
            catch
            {
                playerSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                playerSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                playerSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
        }

        if(chestInventory != null)
        {
            RefreshChestUI();
        }

        RefreshHotbar();
        RefreshEquippedItem();
    }

    public void RefreshHotbar()
    {
        for (int i = 0; i < hotbarSlots.Length; i++)
        {
            try
            {
                //every slot has a child with an Image component to represent whether there's an item on that slot or not
                //every slot also has a child with a Text component to represent the amount of playerInventory.items in that slot
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = playerInventory.items[i].GetItem().itemIcon;

                if (playerInventory.items[i].GetQuantity() > 1)
                {
                    hotbarSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = playerInventory.items[i].GetQuantity().ToString();
                }
                else
                {
                    hotbarSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
                }
            }
            catch
            {
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                hotbarSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                hotbarSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }

    public void SwitchInventoryUI()
    {
        if(activeInventoryUIObject == playerInventoryUIObject)
        {
            activeInventoryUIObject = sharedInventoryUIObject;
            itemCursor = sharedInventoryCursor;
            playerSlotHolder = sharedInventoryPlayerSlotHolder;
        }
        else if(activeInventoryUIObject == sharedInventoryUIObject)
        {
            activeInventoryUIObject = playerInventoryUIObject;
            itemCursor = playerInventoryCursor;
            playerSlotHolder = playerInventoryPlayerSlotHolder;
        }
    }

    public void OpenPlayerInventory()
    {
        playerInventoryUIObject.SetActive(true);
        audioSource.PlayOneShot(openSound);
        activeInventoryUIObject = playerInventoryUIObject;

        playerSlotHolder = playerInventoryPlayerSlotHolder;
        itemCursor = playerInventoryCursor;

        for (int i = 0; i < playerInventory.maxSize; i++)
        {
            playerSlots[i] = playerSlotHolder.transform.GetChild(i).gameObject;
        }

        craftStationManager.enabled = true;

        craftStationManager.craftSlotHolder = playercraftSlotHolder;
        craftStationManager.craftingQueueSlotsHolder = playercraftingQueueSlotsHolder;
        craftStationManager.craftSlotSelector = playercraftSlotSelector;
        craftStationManager.requirementSlotsHolder = playerrequirementSlotsHolder;
        craftStationManager.craftAmountText = playercraftAmountText;


        craftStationManager.craftStationInventory = playerInventory;
        craftStationInventory = playerInventory;
        craftStationInventory.OpenCraftStation(playerCraftProgressBar);
        craftStationManager.InitializeCraftSlots();

        RefreshUI();
    }

    public void OpenSharedInventory()
    {
        sharedInventoryUIObject.SetActive(true);
        activeInventoryUIObject = sharedInventoryUIObject;

        playerSlotHolder = sharedInventoryPlayerSlotHolder;
        itemCursor = sharedInventoryCursor;

        chestSlotHolder = sharedInventoryChestSlotHolder;
    }

    public void OpenCraftStation()
    {
        craftStationUIObject.SetActive(true);
        activeInventoryUIObject = craftStationUIObject;

        playerSlotHolder = craftStationPlayerSlotHolder;
        itemCursor = craftStationCursor;

        chestSlotHolder = craftStationChestSlotHolder;

        craftStationManager.craftSlotHolder = craftSlotHolder;
        craftStationManager.craftingQueueSlotsHolder = craftingQueueSlotsHolder;
        craftStationManager.craftSlotSelector = craftSlotSelector;
        craftStationManager.requirementSlotsHolder = requirementSlotsHolder;
        craftStationManager.craftAmountText = craftAmountText;

        craftStationManager.craftStationInventory = chest.GetComponent<CraftStationInventory>();
        craftStationInventory = craftStationManager.craftStationInventory;
        craftStationInventory.OpenCraftStation(craftProgressBar);
        craftStationManager.InitializeCraftSlots();
    }


    public void GetUIElementsForChest(GameObject _inventoryObject, GameObject _inventoryCursor, GameObject _playerSlotsHolder, GameObject _chestSlotsHolder)
    {
        sharedInventoryUIObject = _inventoryObject;
        sharedInventoryCursor = _inventoryCursor;
        sharedInventoryPlayerSlotHolder = _playerSlotsHolder;
        sharedInventoryChestSlotHolder = _chestSlotsHolder;
    }

    public void GetUIElementsForCraftStation(GameObject _inventoryObject, GameObject _inventoryCursor, GameObject _playerSlotsHolder, GameObject _chestSlotsHolder, Slider _craftProgressBar, GameObject _craftSlotsHolder, GameObject _craftingQueueSlotsHolder, GameObject _craftSlotSelector, GameObject _requirementSlotsHolder, TextMeshProUGUI _craftAmountText)
    {
        craftStationUIObject = _inventoryObject;
        craftStationCursor = _inventoryCursor;
        craftStationPlayerSlotHolder = _playerSlotsHolder;
        craftStationChestSlotHolder = _chestSlotsHolder;
        craftProgressBar = _craftProgressBar;
        craftSlotHolder = _craftSlotsHolder;
        craftingQueueSlotsHolder = _craftingQueueSlotsHolder;
        craftSlotSelector = _craftSlotSelector;
        requirementSlotsHolder = _requirementSlotsHolder;
        craftAmountText = _craftAmountText;
    }

    public void GetUIElementsForNPC(TradeSlotClass[] _tradeSlots, GameObject _inventoryObject, GameObject _inventoryCursor, GameObject _playerSlotsHolder, GameObject _tradableSlotsHolder, GameObject _tradableSlotSelector, GameObject _requirementSlotsHolder, TextMeshProUGUI _tradeAmountText)
    {
        itemCursor = _inventoryCursor;
        craftStationUIObject = _inventoryObject;
        craftStationPlayerSlotHolder = _playerSlotsHolder;
        craftSlotHolder = _tradableSlotsHolder;
        craftSlotSelector = _tradableSlotSelector;
        requirementSlotsHolder = _requirementSlotsHolder;
        craftAmountText = _tradeAmountText;
        tradeSlots = _tradeSlots;

        activeInventoryUIObject = _inventoryObject;
        tradeSlotObjects = new GameObject[craftSlotHolder.transform.childCount];

        for (int i = 0; i < tradeSlotObjects.Length; i++)
        {
            tradeSlotObjects[i] = craftSlotHolder.transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < _tradeSlots.Length; i++)
        {
            if(_tradeSlots[i].GetItem() != null)
            {
                tradeSlotObjects[i].transform.GetChild(0).GetComponent<Image>().sprite = _tradeSlots[i].GetItem().itemIcon; 
            }
            else
            {
                tradeSlotObjects[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
            }
        }
    }

    public bool AddItem(ItemClass item, int quantity)
    {
        //if the item is not stackable, just add it to an empty slot
        if (!item.isStackable)
        {
            SlotClass emptySlot = new SlotClass();
            if (tempSlot != null && !tempSlot.GetisFromPlayer())
            {
                emptySlot = EmptySlot();
            }
            else
            {
                emptySlot = EmptySlotInChest();
            }


            if (emptySlot == null)
            {
                //cant add this item
                return false;
            }
            {
                emptySlot.AddItem(item, 1);
            }
        }
        else
        {
            //try to add the items on slots that contain that item already
            //if this is not enough, try to add it on empty slots then

            List<SlotClass> compatibleSlots = new List<SlotClass>();
            
            //check if the item gets added to player inventory or to other inventory
            if (tempSlot != null && !tempSlot.GetisFromPlayer())
            {
                ContainsList(item, out List<SlotClass> _compatibleSlots);
                compatibleSlots = _compatibleSlots;
            }
            else
            {
                ContainsListInChest(item, out List<SlotClass> _compatibleSlots);
                compatibleSlots = _compatibleSlots;
            }

            //if there are slots that contain that item
            if(compatibleSlots != null || compatibleSlots.Count != 0)
            {
                foreach (SlotClass slot in compatibleSlots)
                {
                    int result = slot.GetQuantity() + quantity;

                    //quantity of added item is too much for the item in that slot
                    if (result > slot.GetItem().maxStackSize)
                    {
                        //put as much as you can on that slot and move on to the next slot
                        int slotQuantityLeft = slot.GetItem().maxStackSize - slot.GetQuantity();
                        slot.AddQuantity(slotQuantityLeft);
                        quantity -= slotQuantityLeft;
                    }
                    else //quantity of dragging item fits into that slot
                    {
                        slot.AddQuantity(quantity);
                        quantity = 0;
                        break;
                    }
                }
            }

            //if there is still quantity left
            if(quantity != 0)
            {
                //try to add the items on empty slots
                //if this is not enough, then there is no space in the inventory anymore

                List<SlotClass> emptySlots = new List<SlotClass>();

                //check if the item gets added to player inventory or to other inventory
                if (tempSlot != null && !tempSlot.GetisFromPlayer())
                {
                    EmptySlotList(out List<SlotClass> _emptySlots);
                    emptySlots = _emptySlots;
                }
                else
                {
                    EmptySlotListInChest(out List<SlotClass> _emptySlots);
                    emptySlots = _emptySlots;
                }

                //if there are empty slots
                if (emptySlots != null || emptySlots.Count != 0)
                {
                    foreach (SlotClass slot in emptySlots)
                    {
                        //if the quantity is still higher than the max stack size of that item
                        if(quantity > item.maxStackSize)
                        {
                            //add a whole stack on the empty slot and move on to the next slot
                            slot.AddItem(item, item.maxStackSize);
                            quantity -= item.maxStackSize;
                        }
                        else //if the quantity is lower than the max stack size of that item
                        {
                            slot.AddItem(item, quantity);
                            quantity = 0;
                            break;
                        }
                    }
                }
            }

            if(quantity != 0)
            {
                RefreshUI();
                return false;
            }
        }




        RefreshUI();
        return true;
    }

    public bool AddItemFromCraftStation(ItemClass item, int quantity)
    {
        //if the item is not stackable, just add it to an empty slot
        if (!item.isStackable)
        {
            EmptySlotListInChest(out List<SlotClass> _emptySlotsInChest);
            
            if (_emptySlotsInChest.Count != 0)
            {
                foreach (SlotClass slot in _emptySlotsInChest)
                {
                    slot.AddItem(item, 1);
                    quantity--;

                    if(quantity == 0)
                    {
                        RefreshUI();
                        return true;
                    }
                }
            }

            EmptySlotList(out List<SlotClass> _emptySlotListInPlayer);

            if (_emptySlotListInPlayer.Count != 0)
            {
                foreach (SlotClass slot in _emptySlotListInPlayer)
                {
                    slot.AddItem(item, 1);
                    quantity--;

                    if (quantity == 0)
                    {
                        RefreshUI();
                        return true;
                    }
                }
            }

            if(quantity > 0)
            {
                DropNotAddableItem(item, quantity);
            }
        }
        else
        {
            //try to add the items on slots that contain that item already
            //if this is not enough, try to add it on empty slots then

            List<SlotClass> compatibleSlots = new List<SlotClass>();
            
            //check if the item gets added to player inventory or to other inventory

            ContainsListInChest(item, out List<SlotClass> _compatibleSlotsInChest);

            if(_compatibleSlotsInChest == null)
            {
                ContainsList(item, out List<SlotClass> _compatibleSlotsInPlayer);
                compatibleSlots = _compatibleSlotsInPlayer;
            }
            else
            {
                compatibleSlots = _compatibleSlotsInChest;
            }

            //if there are slots that contain that item
            if(compatibleSlots != null || compatibleSlots.Count != 0)
            {
                foreach (SlotClass slot in compatibleSlots)
                {
                    int result = slot.GetQuantity() + quantity;

                    //quantity of added item is too much for the item in that slot
                    if (result > slot.GetItem().maxStackSize)
                    {
                        //put as much as you can on that slot and move on to the next slot
                        int slotQuantityLeft = slot.GetItem().maxStackSize - slot.GetQuantity();
                        slot.AddQuantity(slotQuantityLeft);
                        quantity -= slotQuantityLeft;
                    }
                    else //quantity of dragging item fits into that slot
                    {
                        slot.AddQuantity(quantity);
                        quantity = 0;
                        break;
                    }
                }
            }

            //if there is still quantity left
            if(quantity != 0)
            {
                //try to add the items on empty slots
                //if this is not enough, then there is no space in the inventory anymore

                List<SlotClass> emptySlots = new List<SlotClass>();

                //check if the item gets added to player inventory or to other inventory
                EmptySlotListInChest(out List<SlotClass> _emptySlotListInChest);

                if (_emptySlotListInChest == null)
                {
                    EmptySlotList(out List<SlotClass> _emptySlotListInPlayer);
                    emptySlots = _emptySlotListInPlayer;
                }
                else
                {
                    emptySlots = _emptySlotListInChest;
                }

                //if there are empty slots
                if (emptySlots != null || emptySlots.Count != 0)
                {
                    foreach (SlotClass slot in emptySlots)
                    {
                        //if the quantity is still higher than the max stack size of that item
                        if(quantity > item.maxStackSize)
                        {
                            //add a whole stack on the empty slot and move on to the next slot
                            slot.AddItem(item, item.maxStackSize);
                            quantity -= item.maxStackSize;
                        }
                        else //if the quantity is lower than the max stack size of that item
                        {
                            slot.AddItem(item, quantity);
                            quantity = 0;
                            break;
                        }
                    }
                }
            }

            if(quantity != 0)
            {
                DropNotAddableItem(item, quantity);
                RefreshUI();
                return false;
            }
        }




        RefreshUI();
        return true;
    }

    public bool AddItemFast(ItemClass item, int quantity)
    {
        //if the item is not stackable, just add it to an empty slot
        if (!item.isStackable)
        {
            SlotClass emptySlot = new SlotClass();

            if (tempSlot != null && !tempSlot.GetisFromPlayer())
            {
                emptySlot = EmptySlot();
            }
            else
            {
                emptySlot = EmptySlotInChest();
            }


            if (emptySlot == null)
            {
                //cant add this item
                originalSlot.AddItem(item, quantity);
                RefreshUI();
                return false;
            }
            {
                emptySlot.AddItem(item, 1);
            }
        }
        else
        {
            //try to add the items on slots that contain that item already
            //if this is not enough, try to add it on empty slots then

            List<SlotClass> compatibleSlots = new List<SlotClass>();
            
            //check if the item gets added to player inventory or to other inventory
            if (tempSlot != null && !tempSlot.GetisFromPlayer())
            {
                ContainsList(item, out List<SlotClass> _compatibleSlots);
                compatibleSlots = _compatibleSlots;
            }
            else
            {
                ContainsListInChest(item, out List<SlotClass> _compatibleSlots);
                compatibleSlots = _compatibleSlots;
            }

            //if there are slots that contain that item
            if(compatibleSlots != null || compatibleSlots.Count != 0)
            {
                foreach (SlotClass slot in compatibleSlots)
                {
                    int result = slot.GetQuantity() + quantity;

                    //quantity of added item is too much for the item in that slot
                    if (result > slot.GetItem().maxStackSize)
                    {
                        //put as much as you can on that slot and move on to the next slot
                        int slotQuantityLeft = slot.GetItem().maxStackSize - slot.GetQuantity();
                        slot.AddQuantity(slotQuantityLeft);
                        quantity -= slotQuantityLeft;
                    }
                    else //quantity of dragging item fits into that slot
                    {
                        slot.AddQuantity(quantity);
                        quantity = 0;
                        break;
                    }
                }
            }

            //if there is still quantity left
            if(quantity != 0)
            {
                //try to add the items on empty slots
                //if this is not enough, then there is no space in the inventory anymore

                List<SlotClass> emptySlots = new List<SlotClass>();

                //check if the item gets added to player inventory or to other inventory
                if (tempSlot != null && !tempSlot.GetisFromPlayer())
                {
                    EmptySlotList(out List<SlotClass> _emptySlots);
                    emptySlots = _emptySlots;
                }
                else
                {
                    EmptySlotListInChest(out List<SlotClass> _emptySlots);
                    emptySlots = _emptySlots;
                }

                //if there are empty slots
                if (emptySlots != null || emptySlots.Count != 0)
                {
                    foreach (SlotClass slot in emptySlots)
                    {
                        //if the quantity is still higher than the max stack size of that item
                        if(quantity > item.maxStackSize)
                        {
                            //add a whole stack on the empty slot and move on to the next slot
                            slot.AddItem(item, item.maxStackSize);
                            quantity -= item.maxStackSize;
                        }
                        else //if the quantity is lower than the max stack size of that item
                        {
                            slot.AddItem(item, quantity);
                            quantity = 0;
                            break;
                        }
                    }
                }
            }

            if(quantity != 0)
            {
                originalSlot.AddItem(item, quantity);
                RefreshUI();
                return false;
            }
        }


        if (activeInventoryUIObject == craftStationUIObject)
        {
            craftStationInventory.GetListOfLeftItems();
            craftStationManager.RefreshUI();
        }

        RefreshUI();
        return true;
    }

    public bool AddDropItem(Drops drop, ItemClass item, int quantity)
    {
        //if the item is not stackable, just add it to an empty slot
        if (!item.isStackable)
        {
            SlotClass emptySlot = EmptySlot();

            if(emptySlot == null)
            {
                //cant add this item
                return false;
            }
            {
                emptySlot.AddItem(item, 1);
            }
        }
        else
        {
            //try to add the items on slots that contain that item already
            //if this is not enough, try to add it on empty slots then

            ContainsList(item, out List<SlotClass> _compatibleSlots);
            List<SlotClass> compatibleSlots = _compatibleSlots;

            //if there are slots that contain that item
            if(compatibleSlots != null || compatibleSlots.Count != 0)
            {
                foreach (SlotClass slot in compatibleSlots)
                {
                    int result = slot.GetQuantity() + quantity;

                    //quantity of added item is too much for the item in that slot
                    if (result > slot.GetItem().maxStackSize)
                    {
                        //put as much as you can on that slot and move on to the next slot
                        int slotQuantityLeft = slot.GetItem().maxStackSize - slot.GetQuantity();
                        slot.AddQuantity(slotQuantityLeft);
                        quantity -= slotQuantityLeft;
                    }
                    else //quantity of dragging item fits into that slot
                    {
                        slot.AddQuantity(quantity);
                        quantity = 0;
                        break;
                    }
                }
            }

            //if there is still quantity left
            if(quantity != 0)
            {
                //try to add the items on empty slots
                //if this is not enough, then there is no space in the inventory anymore

                EmptySlotList(out List<SlotClass> _emptySlots);
                List<SlotClass> emptySlots = _emptySlots;

                //if there are empty slots
                if (emptySlots != null || emptySlots.Count != 0)
                {
                    foreach (SlotClass slot in emptySlots)
                    {
                        //if the quantity is still higher than the max stack size of that item
                        if(quantity > item.maxStackSize)
                        {
                            //add a whole stack on the empty slot and move on to the next slot
                            slot.AddItem(item, item.maxStackSize);
                            quantity -= item.maxStackSize;
                        }
                        else //if the quantity is lower than the max stack size of that item
                        {
                            slot.AddItem(item, quantity);
                            quantity = 0;
                            break;
                        }
                    }
                }
            }

            if(quantity != 0)
            {
                RefreshUI();
                drop.quantity = quantity;
                return false;
            }
        }




        RefreshUI();
        return true;
    }

    public bool RemoveItem(ItemClass item)
    {
        SlotClass temp = Contains(item);

        if (temp != null)
        {
            if (temp.GetQuantity() > 1)
            {
                temp.SubQuantity(1);
            }
            else
            {
                int slotToRemoveIndex = 0;

                for (int i = 0; i < playerInventory.items.Length; i++)
                {
                    if (playerInventory.items[i].GetItem() == item)
                    {
                        slotToRemoveIndex = i;
                        break;
                    }
                }

                playerInventory.items[slotToRemoveIndex].Clear();
            }
        }
        else
        {
            return false;
        }

        RefreshUI();
        return true;
    }

    public bool UseEquippedItem()
    {
        if(playerInventory.items[selectedSlotIndex].GetQuantity() == 1)
        {
            if (playerInventory.items[selectedSlotIndex].GetItem().GetStructure())
            {
                playerInventory.items[selectedSlotIndex].Clear();
                buildSystem.ClearCurrentStructure();
                buildSystem.buildMode = false;
            }
            else
            {
                playerInventory.items[selectedSlotIndex].Clear();
            }
        }
        else
        {
            playerInventory.items[selectedSlotIndex].SubQuantity(1);
        }

        RefreshHotbar();
        RefreshEquippedItem();
        return true;
    }

    public void DropCurrentItem()
    {
        GetClosestSlot(out SlotClass _closestSlot, out int _slotID, out bool _isFromPlayerInventory);

        if(_closestSlot != null)
        {
            if(_closestSlot.GetItem() != null)
            {
                if (_isFromPlayerInventory)
                {
                    GameObject DropCurrentItem = Instantiate(playerInventory.items[_slotID].GetItem().dropReference, gameObject.transform.position, Quaternion.identity);
                    DropCurrentItem.GetComponent<Drops>().quantity = _closestSlot.GetQuantity();
                    DropCurrentItem.GetComponent<Rigidbody>().AddForce(transform.forward * 100, ForceMode.Impulse);
                    playerInventory.items[_slotID].Clear();
                }
                else
                {
                    GameObject DropCurrentItem = Instantiate(chestInventory.items[_slotID].GetItem().dropReference, gameObject.transform.position, Quaternion.identity);
                    DropCurrentItem.GetComponent<Drops>().quantity = _closestSlot.GetQuantity();
                    DropCurrentItem.GetComponent<Rigidbody>().AddForce(chest.transform.forward * 100, ForceMode.Impulse);
                    chestInventory.items[_slotID].Clear();
                }
            }

            RefreshUI();
        }
    }

    public void DropNotAddableItem(ItemClass _itemToDrop, int _dropAmount)
    {
        GameObject DropCurrentItem = Instantiate(_itemToDrop.dropReference, gameObject.transform.position, Quaternion.identity);
        DropCurrentItem.GetComponent<Drops>().quantity = _dropAmount;
        DropCurrentItem.GetComponent<Rigidbody>().AddForce(transform.forward * 100, ForceMode.Impulse);

        RefreshUI();
    }

    public bool AddItemFastToPlayer()
    {
        GetClosestSlot(out SlotClass _closestSlot, out int _slotID);
        tempSlot = new SlotClass(_closestSlot.GetItem(), _closestSlot.GetQuantity());

        if (_closestSlot != null)
        {
            playerInventory.items[_slotID].Clear();
        }
        else
        {
            return false;
        }

        RefreshUI();
        AddItem(tempSlot.GetItem(), tempSlot.GetQuantity());

        if (activeInventoryUIObject == craftStationUIObject)
        {
            craftStationInventory.GetListOfLeftItems();
            craftStationManager.RefreshUI();
        }

        //tempSlot = null;
        return true;
    }

    public bool AddItemFastToOther()
    {
        GetClosestSlot(out SlotClass _closestSlot, out int _slotID, out bool _isFromPlayerInventory);
        originalSlot = _closestSlot;

        if (_closestSlot != null && _closestSlot.GetItem() != null)
        {
            tempSlot = new SlotClass(_closestSlot.GetItem(), _closestSlot.GetQuantity(), _isFromPlayerInventory);

            if (_isFromPlayerInventory)
            {
                playerInventory.items[_slotID].Clear();
            }
            else
            {
                chestInventory.items[_slotID].Clear();
            }
        }
        else
        {
            return false;
        }

        RefreshUI();
        AddItemFast(tempSlot.GetItem(), tempSlot.GetQuantity());

        if (activeInventoryUIObject == craftStationUIObject)
        {
            craftStationInventory.GetListOfLeftItems();
            craftStationManager.RefreshUI();
        }
        //tempSlot = null;
        return true;
    }

    public SlotClass Contains(ItemClass item)
    {
        foreach (SlotClass slot in playerInventory.items)
        {
            if (slot.GetItem() == item)
            {
                return slot;
            }
        }

        return null;
    }

    public int ContainsAmount(ItemClass _item)
    {
        int i = 0;

        foreach (SlotClass slot in playerInventory.items)
        {
            if (slot.GetItem() == _item)
            {
                i += slot.GetQuantity();
            }
        }

        return i;
    }

    public SlotClass EmptySlot()
    {
        foreach (SlotClass slot in playerInventory.items)
        {
            if (slot.GetItem() == null)
            {
                return slot;
            }
        }

        return null;
    }

    private void ContainsList(ItemClass _compareItem, out List<SlotClass> _items)
    {
        _items = new List<SlotClass>();
        foreach (SlotClass slot in playerInventory.items)
        {
            if (slot.GetItem() == _compareItem)
            {
                _items.Add(slot);
            }
        }
    }

    private void EmptySlotList(out List<SlotClass> _items)
    {
        _items = new List<SlotClass>();

        foreach (SlotClass slot in playerInventory.items)
        {
            if (slot.GetItem() == null)
            {
                _items.Add(slot);
            }
        }
    }

    #endregion Inventory Utilities




    #region Trading

    private bool CheckIfClickedOnTradeSlot()
    {
        GetClosestTradeSlot(out TradeSlotClass _currentTradeSlotClass);

        //if there is no item to move
        if (_currentTradeSlotClass == null || _currentTradeSlotClass.GetItem() == null)
        {
            return false;
        }
        else
        {
            if (!tradeRequirementSlots[0].activeInHierarchy)
            {
                foreach (var item in tradeRequirementSlots)
                {
                    item.SetActive(true);
                }

                tradeButtons.SetActive(true);
            }

            currentTradeSlot = _currentTradeSlotClass;

            tradeAmount = 1;
            craftAmountText.text = tradeAmount.ToString();

            tradeRequirementSlots[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = currentTradeSlot.GetNeededAmount().ToString();
            tradeRequirementSlots[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "1";

            tradeRequirementSlots[0].transform.GetChild(0).GetComponent<Image>().sprite = currentTradeSlot.GetNeededItem().itemIcon;
            tradeRequirementSlots[1].transform.GetChild(0).GetComponent<Image>().sprite = currentTradeSlot.GetItem().itemIcon;
        }


        RefreshUI();

        return true;
    }

    public void ChangeTradeAmount(int _change)
    {
        if (tradeAmount + _change < 1) return;

        tradeAmount += _change;

        RefreshTradeRequirements();
    }

    private void RefreshTradeRequirements()
    {
        craftAmountText.text = tradeAmount.ToString();

        tradeRequirementSlots[0].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = (currentTradeSlot.GetNeededAmount() * tradeAmount).ToString();
        tradeRequirementSlots[1].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = tradeAmount.ToString();
    }

    public void OnTradeButton()
    {
        CheckIfPlayerHasEnoughItemsForTrade();
    }

    private void CheckIfPlayerHasEnoughItemsForTrade()
    {
        int quantityRemoved = 0;

        int quantityToRemove = currentTradeSlot.GetNeededAmount() * tradeAmount;
        int quantityAvailable = 0;

        //check if player has enough quantity of the required item
        for (int i = 0; i < playerInventory.items.Length; i++)
        {
            if(playerInventory.items[i].GetItem() == currentTradeSlot.GetNeededItem())
            {
                if (playerInventory.items[i].GetItem().isStackable)
                {
                    quantityAvailable += playerInventory.items[i].GetQuantity();
                }
                else
                {
                    quantityAvailable++;
                }
            }

            if (quantityAvailable >= quantityToRemove)
            {
                break;
            }
        }

        if(quantityAvailable < quantityToRemove)
        {
            Debug.Log("Player has not enough items to buy this");
            return;
        }


        //remove items from player
        if (quantityAvailable >= quantityToRemove)
        {
            for (int i = 0; i < playerInventory.items.Length; i++)
            {
                if(playerInventory.items[i].GetItem() == currentTradeSlot.GetNeededItem())
                {
                    if (quantityToRemove >= playerInventory.items[i].GetQuantity())
                    {
                        quantityToRemove -= playerInventory.items[i].GetQuantity();
                        quantityRemoved += playerInventory.items[i].GetQuantity();

                        playerInventory.items[i].Clear();
                    }
                    else
                    {
                        playerInventory.items[i].SubQuantity(quantityToRemove);
                        quantityRemoved += quantityToRemove;
                        quantityToRemove = 0;
                        break;
                    }
                }
            }
        }


        //check if player has enough space for the item he gets
        int quantityAvailableForTradeItem = 0;
        int quantityOfTradeItem = tradeAmount;

        if (!currentTradeSlot.GetItem().isStackable)
        {
            for (int i = 0; i < playerInventory.items.Length; i++)
            {
                if (playerInventory.items[i].GetItem() == null)
                {
                    quantityAvailableForTradeItem++;
                }

                if (quantityAvailableForTradeItem >= tradeAmount)
                {
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < playerInventory.items.Length; i++)
            {
                if (playerInventory.items[i].GetItem() == currentTradeSlot.GetItem())
                {
                    quantityAvailableForTradeItem += (playerInventory.items[i].GetItem().maxStackSize - playerInventory.items[i].GetQuantity());
                }
                else if(playerInventory.items[i].GetItem() == null)
                {
                    quantityAvailableForTradeItem += currentTradeSlot.GetItem().maxStackSize;
                }

                if (quantityAvailableForTradeItem >= tradeAmount)
                {
                    break;
                }
            }
        }

        if(quantityAvailableForTradeItem < tradeAmount)
        {
            Debug.Log("Player has not enough space for this");
            return;
        }


        //if not enough space, put the removed items back
        if (quantityAvailableForTradeItem < quantityOfTradeItem)
        {
            for (int i = 0; i < playerInventory.items.Length; i++)
            {
                if (playerInventory.items[i].GetItem() == currentTradeSlot.GetItem())
                {
                    int amountLeftInSlot = playerInventory.items[i].GetItem().maxStackSize - playerInventory.items[i].GetQuantity();

                    if (quantityRemoved > amountLeftInSlot)
                    {
                        playerInventory.items[i].AddItem(currentTradeSlot.GetNeededItem(), amountLeftInSlot);
                        quantityRemoved -= amountLeftInSlot;
                    }
                    else
                    {
                        playerInventory.items[i].AddItem(currentTradeSlot.GetNeededItem(), quantityRemoved);
                        quantityRemoved = 0;
                        RefreshUI();
                        return;
                    }
                }
                else if (playerInventory.items[i].GetItem() == null)
                {
                    if (quantityRemoved > currentTradeSlot.GetNeededItem().maxStackSize)
                    {
                        playerInventory.items[i].AddItem(currentTradeSlot.GetNeededItem(), currentTradeSlot.GetNeededItem().maxStackSize);
                        quantityRemoved -= currentTradeSlot.GetNeededItem().maxStackSize;
                    }
                    else
                    {
                        playerInventory.items[i].AddItem(currentTradeSlot.GetNeededItem(), quantityRemoved);
                        quantityRemoved = 0;
                        RefreshUI();
                        return;
                    }
                }
            }
        }
        //if enough space, add traded item to player
        else
        {
            for (int i = 0; i < playerInventory.items.Length; i++)
            {
                if (playerInventory.items[i].GetItem() == currentTradeSlot.GetItem())
                {
                    int amountLeftInSlot = playerInventory.items[i].GetItem().maxStackSize - playerInventory.items[i].GetQuantity();

                    if (amountLeftInSlot > quantityOfTradeItem)
                    {
                        playerInventory.items[i].AddQuantity(quantityOfTradeItem);
                        quantityOfTradeItem = 0;
                        break;
                    }
                    else
                    {
                        playerInventory.items[i].AddQuantity(amountLeftInSlot);
                        quantityOfTradeItem -= amountLeftInSlot;
                    }
                }
                else if(playerInventory.items[i].GetItem() == null)
                {
                    if (quantityOfTradeItem > currentTradeSlot.GetItem().maxStackSize)
                    {
                        playerInventory.items[i].AddItem(currentTradeSlot.GetItem(), currentTradeSlot.GetItem().maxStackSize);
                        quantityOfTradeItem -= currentTradeSlot.GetItem().maxStackSize;
                    }
                    else
                    {
                        playerInventory.items[i].AddItem(currentTradeSlot.GetItem(), quantityOfTradeItem);
                        quantityOfTradeItem = 0;
                        RefreshUI();
                        return;
                    }
                }
            }
        }

        RefreshUI();

    }

    #endregion Trading




    #region Moving Stuff
    private void CheckForMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(Input.GetKey(KeyCode.LeftShift) && !isMovingItem)
            {
                if(activeInventoryUIObject == playerInventoryUIObject)
                {
                    AddItemFastToPlayer();
                }
                else
                {
                    AddItemFastToOther();
                }
            }
            else
            {
                if (isMovingItem)
                {
                    EndItemMove();
                }
                else
                {
                    BeginItemMove();
                }
            }

            if (npcTradingIsActive)
            {
                CheckIfClickedOnTradeSlot();
            }

        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (isMovingItem)
            {
                EndItemMove_Single();
            }
            else
            {
                BeginItemMove_Half();
            }
        }
    }

    private bool BeginItemMove()
    {
        originalSlot = GetClosestSlot();

        //if there is no item to move
        if (originalSlot == null || originalSlot.GetItem() == null)
        {
            return false;
        }

        audioSource.PlayOneShot(blopSound);
        movingSlot = new SlotClass(originalSlot);
        originalSlot.Clear();
        isMovingItem = true;
        itemCursor.SetActive(true);

        if (movingSlot.GetItem().isStackable)
        {
            itemCursor.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = movingSlot.GetQuantity().ToString();
        }
        else
        {
            itemCursor.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }

        if (craftStationManager.enabled == true)
        {
            craftStationManager.RefreshUI();
        }

        //this is executed in the Update Method but you could see the cursor on the old position for 1 frame if not executing it here
        itemCursor.GetComponent<Image>().sprite = movingSlot.GetItem().itemIcon;
        itemCursor.transform.position = Input.mousePosition;

        if(activeInventoryUIObject == craftStationUIObject && !npcTradingIsActive)
        {
            craftStationInventory.GetListOfLeftItems();
            craftStationManager.RefreshUI();
        }
        RefreshUI();

        return true;
    }

    private bool BeginItemMove_Half()
    {
        originalSlot = GetClosestSlot();

        //if there is no item to move
        if (originalSlot == null || originalSlot.GetItem() == null)
        {
            return false;
        }

        audioSource.PlayOneShot(blopSound);
        movingSlot = new SlotClass(originalSlot.GetItem(), Mathf.CeilToInt(originalSlot.GetQuantity() / 2f));
        originalSlot.SubQuantity(Mathf.CeilToInt(originalSlot.GetQuantity() / 2f));

        if (originalSlot.GetQuantity() == 0)
        {
            originalSlot.Clear();
        }

        isMovingItem = true;
        itemCursor.SetActive(true);

        if (movingSlot.GetItem().isStackable)
        {
            itemCursor.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = movingSlot.GetQuantity().ToString();
        }
        else
        {
            itemCursor.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        }

        if (craftStationManager.enabled == true)
        {
            craftStationManager.RefreshUI();
        }

        //this is executed in the Update Method but you could see the cursor on the old position for 1 frame if not executing it here
        itemCursor.GetComponent<Image>().sprite = movingSlot.GetItem().itemIcon;
        itemCursor.transform.position = Input.mousePosition;

        if (activeInventoryUIObject == craftStationUIObject && !npcTradingIsActive)
        {
            craftStationInventory.GetListOfLeftItems();
            craftStationManager.RefreshUI();
        }
        RefreshUI();


        return true;
    }

    private bool EndItemMove()
    {
        originalSlot = GetClosestSlot();

        //if there is no slot nearby
        if (originalSlot == null)
        {
            AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
            movingSlot.Clear();
        }
        else //if you clicked a slot
        {
            //if there is already an item on that slot
            if (originalSlot.GetItem() != null)
            {
                //if this item is the same as you are dragging
                if (originalSlot.GetItem() == movingSlot.GetItem())
                {
                    //if the item on the slot is stackable
                    if (originalSlot.GetItem().isStackable)
                    {
                        //if that item is full stacked
                        if(originalSlot.GetQuantity() == originalSlot.GetItem().maxStackSize)
                        {
                            return false;
                        }
                        else //if there is some space on that slot
                        {
                            int result = originalSlot.GetQuantity() + movingSlot.GetQuantity();

                            //quantity of dragging item is too much for the item in that slot
                            if(result > originalSlot.GetItem().maxStackSize)
                            {
                                int slotQuantityLeft = originalSlot.GetItem().maxStackSize - originalSlot.GetQuantity();
                                originalSlot.AddQuantity(slotQuantityLeft);
                                movingSlot.SubQuantity(slotQuantityLeft);
                                itemCursor.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = movingSlot.GetQuantity().ToString();
                                RefreshUI();
                                return false;
                            }
                            else //quantity of dragging item fits into that slot
                            {
                                originalSlot.AddQuantity(movingSlot.GetQuantity());
                                movingSlot.Clear();
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else //if the dragging item is a different item than the item on that slot
                {
                    //swap item from slot with the item you are dragging
                    tempSlot = new SlotClass(originalSlot);
                    originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                    movingSlot.AddItem(tempSlot.GetItem(), tempSlot.GetQuantity());
                    itemCursor.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = movingSlot.GetQuantity().ToString();
                    audioSource.PlayOneShot(blopSound);
                    RefreshUI();

                    if(craftStationManager.enabled == true)
                    {
                        craftStationManager.RefreshUI();
                    }

                    return true;
                }
            }
            else //if the slot is empty
            {
                originalSlot.AddItem(movingSlot.GetItem(), movingSlot.GetQuantity());
                movingSlot.Clear();
            }
        }

        if (craftStationManager.enabled == true)
        {
            craftStationManager.RefreshUI();
        }

        isMovingItem = false;
        itemCursor.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "";
        itemCursor.SetActive(false);
        if (activeInventoryUIObject == craftStationUIObject && !npcTradingIsActive)
        {
            craftStationInventory.GetListOfLeftItems();
            craftStationManager.RefreshUI();
        }
        RefreshUI();
        audioSource.PlayOneShot(blopSound);

        OnItemMoveEnd?.Invoke(movingSlot.GetItem());

        return true;
    }

    private bool EndItemMove_Single()
    {
        originalSlot = GetClosestSlot();

        //if there is no slot nearby
        if (originalSlot == null)
        {
            return false;
        }
        else //if you clicked on a slot
        {
            //if there is an item on that slot
            if (originalSlot.GetItem() != null)
            {
                //if it's the same item as the one you are dragging
                if (originalSlot.GetItem() == movingSlot.GetItem())
                {
                    //if the item on that slot is stackable
                    if (originalSlot.GetItem().isStackable)
                    {
                        //if the item on that slot is already full stacked
                        if(originalSlot.GetQuantity() >= originalSlot.GetItem().maxStackSize)
                        {
                            return false;
                        }
                        else //if there is space left on that slot, add one quantity
                        {
                            originalSlot.AddQuantity(1);
                            movingSlot.SubQuantity(1);
                        }
                    }
                    else //if it's not stackable
                    {
                        return false;
                    }
                }
                else //if the item on that slot is a different item than the item you are dragging
                {
                    return false;
                }
            }
            else //if there is no item on that slot
            {
                originalSlot.AddItem(movingSlot.GetItem(), 1);
                movingSlot.SubQuantity(1);
            }



            if (movingSlot.GetQuantity() < 1)
            {
                isMovingItem = false;
                itemCursor.SetActive(false);
                movingSlot.Clear();
            }
            else
            {
                isMovingItem = true;
                itemCursor.SetActive(true);
            }
        }

        if (craftStationManager.enabled == true)
        {
            craftStationManager.RefreshUI();
        }

        itemCursor.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = movingSlot.GetQuantity().ToString();

        if (activeInventoryUIObject == craftStationUIObject && !npcTradingIsActive)
        {
            craftStationInventory.GetListOfLeftItems();
            craftStationManager.RefreshUI();
        }
        RefreshUI();
        audioSource.PlayOneShot(blopSound);


        return true;
    }

    private SlotClass GetClosestSlot()
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (Vector2.Distance(playerSlots[i].transform.position, Input.mousePosition) <= 50)
            {
                return playerInventory.items[i];
            }
        }

        if(chestInventory != null)
        {
            for (int i = 0; i < chestSlots.Length; i++)
            {
                if (Vector2.Distance(chestSlots[i].transform.position, Input.mousePosition) <= 50)
                {
                    return chestInventory.items[i];
                }
            }
        }

        return null;
    }

    private void GetClosestSlot(out SlotClass _closestSlot, out int _slotID)
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (Vector2.Distance(playerSlots[i].transform.position, Input.mousePosition) <= 50)
            {
                _closestSlot = playerInventory.items[i];
                _slotID = i;
                return;
            }
        }

        if(chestInventory != null)
        {
            for (int i = 0; i < chestSlots.Length; i++)
            {
                if (Vector2.Distance(chestSlots[i].transform.position, Input.mousePosition) <= 50)
                {
                    _closestSlot = chestInventory.items[i];
                    _slotID = i;
                    return;
                }
            }
        }

        _closestSlot = null;
        _slotID = 99999;
    }

    private void GetClosestSlot(out SlotClass _closestSlot, out int _slotID, out bool _playerInventory)
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (Vector2.Distance(playerSlots[i].transform.position, Input.mousePosition) <= 50)
            {
                _closestSlot = playerInventory.items[i];
                _slotID = i;
                _playerInventory = true;
                return;
            }
        }

        if(chestInventory != null)
        {
            for (int i = 0; i < chestSlots.Length; i++)
            {
                if (Vector2.Distance(chestSlots[i].transform.position, Input.mousePosition) <= 50)
                {
                    _closestSlot = chestInventory.items[i];
                    _slotID = i;
                    _playerInventory = false;
                    return;
                }
            }
        }

        _closestSlot = null;
        _slotID = 99999;
        _playerInventory = true;
    }

    private void GetClosestSlot(out SlotClass _item, out bool _playerInventory)
    {
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (Vector2.Distance(playerSlots[i].transform.position, Input.mousePosition) <= 50)
            {
                _item = playerInventory.items[i];
                _playerInventory = true;
                return;
            }
        }

        for (int i = 0; i < chestSlots.Length; i++)
        {
            if (Vector2.Distance(chestSlots[i].transform.position, Input.mousePosition) <= 50)
            {
                _item = chestInventory.items[i];
                _playerInventory = false;
                return;
            }
        }

        _item = null;
        _playerInventory = false;
    }

    private TradeSlotClass GetClosestTradeSlot(out TradeSlotClass _currentTradeSlotClass)
    {
        _currentTradeSlotClass = null;

        for (int i = 0; i < tradeSlots.Length; i++)
        {
            if (Vector2.Distance(tradeSlotObjects[i].transform.position, Input.mousePosition) <= 50)
            {
                _currentTradeSlotClass = tradeSlots[i];
            }
        }

        return _currentTradeSlotClass;
    }


    #endregion Moving Stuff




    #region HotBar Stuff
    private void CheckForMouseScroll()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (selectedSlotIndex == 8)
            {
                selectedSlotIndex = 0;
            }
            else
            {
                selectedSlotIndex = Mathf.Clamp(selectedSlotIndex + 1, 0, hotbarSlots.Length - 1);
            }

            hotbarSelector.transform.position = hotbarSlots[selectedSlotIndex].transform.position;
            selectedItem = playerInventory.items[selectedSlotIndex].GetItem();

            equipItemHandler.TryToEquipItem(selectedItem);
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (selectedSlotIndex == 0)
            {
                selectedSlotIndex = 8;
            }
            else
            {
                selectedSlotIndex = Mathf.Clamp(selectedSlotIndex - 1, 0, hotbarSlots.Length - 1);
            }

            hotbarSelector.transform.position = hotbarSlots[selectedSlotIndex].transform.position;
            selectedItem = playerInventory.items[selectedSlotIndex].GetItem();

            equipItemHandler.TryToEquipItem(selectedItem);
        }
    }

    public void EquipSelectedItem()
    {
        if(selectedItem == null)
        {
            if (playerItemHolder.transform.childCount != 0)
            {
                for (int i = 0; i < playerItemHolder.transform.childCount; i++)
                {
                    Destroy(playerItemHolder.transform.GetChild(i).gameObject);
                }
            }

            equipItemHandler.UnequipItem();
        }
        else
        {
            if(selectedItem.equipReference != null)
            {
                if (playerItemHolder.transform.childCount != 0)
                {
                    for (int i = 0; i < playerItemHolder.transform.childCount; i++)
                    {
                        Destroy(playerItemHolder.transform.GetChild(i).gameObject);
                    }

                    GameObject equippedItem = Instantiate(selectedItem.equipReference, playerItemHolder.transform);
                    equippedItem.transform.localScale = selectedItem.equipReference.transform.localScale;
                    equippedItem.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    equippedItem.transform.localPosition = Vector3.zero;
                }
                else
                {
                    GameObject equippedItem = Instantiate(selectedItem.equipReference, playerItemHolder.transform);
                    equippedItem.transform.localScale = selectedItem.equipReference.transform.localScale;
                    equippedItem.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    equippedItem.transform.localPosition = Vector3.zero;
                }
            }
            else
            {
                for (int i = 0; i < playerItemHolder.transform.childCount; i++)
                {
                    Destroy(playerItemHolder.transform.GetChild(i).gameObject);
                }

                Debug.LogError("Selected Item has no EquipReference!");
            }
        }


        if (selectedItem != null && selectedItem.GetStructure())
        {
            buildSystem.ClearCurrentStructure();

            if(buildSystem.enabled == false)
            {
                buildSystem.enabled = true;
            }

            buildSystem.buildMode = true;
            buildSystem.ActivateCurrentStructure(selectedItem.GetStructure());
        }
        else if (buildSystem.enabled == true && buildSystem.currentStructure != null)
        {
            buildSystem.buildMode = false;
            buildSystem.DeactivateCurrentStructure();

            buildSystem.enabled = false;
        }
    }

    private void RefreshEquippedItem()
    {
        if (selectedItem == null) return;

        selectedItem = playerInventory.items[selectedSlotIndex].GetItem();

        if (selectedItem == null)
        {
            if (playerItemHolder.transform.childCount != 0)
            {
                for (int i = 0; i < playerItemHolder.transform.childCount; i++)
                {
                    Destroy(playerItemHolder.transform.GetChild(i).gameObject);
                }
            }
        }
        else
        {
            if (playerItemHolder.transform.childCount != 0)
            {
                for (int i = 0; i < playerItemHolder.transform.childCount; i++)
                {
                    Destroy(playerItemHolder.transform.GetChild(i).gameObject);
                }

                GameObject equippedItem = Instantiate(selectedItem.equipReference, playerItemHolder.transform);
                equippedItem.transform.localScale = selectedItem.equipReference.transform.localScale;
                equippedItem.transform.localRotation = Quaternion.Euler(Vector3.zero);
                equippedItem.transform.localPosition = Vector3.zero;
            }
            else
            {
                GameObject equippedItem = Instantiate(selectedItem.equipReference, playerItemHolder.transform);
                equippedItem.transform.localScale = selectedItem.equipReference.transform.localScale;
                equippedItem.transform.localRotation = Quaternion.Euler(Vector3.zero);
                equippedItem.transform.localPosition = Vector3.zero;
            }
        }

    }

    #endregion Hotbar Stuff




    private void CalculateQuantity(int slotStackSize, int slotQuantity, int moveSlotQuantity)
    {
        int result = slotQuantity + moveSlotQuantity;

        if(result > slotStackSize)
        {

        }
    }

    private void HighlightCurrentSlot()
    {
        //find nearest Inventory slot
        for (int i = 0; i < playerSlots.Length; i++)
        {
            if (Vector2.Distance(playerSlots[i].transform.position, Input.mousePosition) <= 50)
            {
                currentSlot = playerSlots[i];
            }
        }

        if (chestInventory != null)
        {
            for (int i = 0; i < chestSlots.Length; i++)
            {
                if (Vector2.Distance(chestSlots[i].transform.position, Input.mousePosition) <= 50)
                {
                    currentSlot = chestSlots[i];
                }
            }
        }

        if (npcTradingIsActive)
        {
            for (int i = 0; i < tradeSlotObjects.Length; i++)
            {
                if (Vector2.Distance(tradeSlotObjects[i].transform.position, Input.mousePosition) <= 50)
                {
                    currentSlot = tradeSlotObjects[i];
                }
            }
        }

        if(currentSlot == null)
        {
            if(oldCurrentSlot != null)
            {
                oldCurrentSlot.GetComponent<Image>().color = normalColor;
                oldCurrentSlot = null;
            }
        }
        else
        {
            if(oldCurrentSlot != null)
            {
                if(oldCurrentSlot != currentSlot)
                {
                    oldCurrentSlot.GetComponent<Image>().color = normalColor;
                    currentSlot.GetComponent<Image>().color = highlightColor;
                    //deactivate oldcurrentslot highlight
                    //activate currentslot highlight
                }

                oldCurrentSlot = currentSlot;
            }
            else
            {
                currentSlot.GetComponent<Image>().color = highlightColor;
                oldCurrentSlot = currentSlot;
            }
        }

        currentSlot = null;
    }

    private void RefreshCurrentUI(CraftStationInventory _playerInventory)
    {
        RefreshUI();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftStationManager : MonoBehaviour
{
    [SerializeField] 
    private InventoryManager inventoryManager;

    public CraftStationInventory craftStationInventory;

    private CraftableItemClass selectedItem;




    [Header("UI GameObjects")]
    public GameObject craftSlotHolder;

    public GameObject craftingQueueSlotsHolder;

    public GameObject craftSlotSelector;

    public GameObject requirementSlotsHolder;

    public TextMeshProUGUI craftAmountText;




    [Header("Design")]
    [SerializeField] 
    private Color craftableColor;

    [SerializeField] 
    private Color notCraftableColor;




    public GameObject[] craftSlots, queueSlots;

    private GameObject[] requirementSlots;




    private int craftAmount;

    private bool selectedItemIsCraftable;




    #region Unity Methods
    private void OnEnable()
    {
        CraftStationInventory.OnItemCraftFinish += OnItemCrafted;
        craftAmount = 1;
        craftAmountText.text = craftAmount.ToString();
        StartCoroutine(WaitForEndOfFrame());
    }

    private void Update()
    {
        CheckForMouseClick();

        if(selectedItem != null)
        {
            CheckForKeyInput();
        }
    }

    private void OnDisable()
    {
        CraftStationInventory.OnItemCraftFinish -= OnItemCrafted;
        selectedItem = null;
        craftSlotSelector.SetActive(false);
        requirementSlotsHolder.SetActive(false);
    }

    #endregion Unity Methods



    private IEnumerator WaitForEndOfFrame()
    {
        yield return new WaitForEndOfFrame();

        RefreshUI();
    }

    private void CheckForKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            craftStationInventory.OnCraft(selectedItem, 1);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            craftStationInventory.OnCraft(selectedItem, 5);
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            craftStationInventory.GetMaxCraftAmountForRequestedItem(selectedItem);
        }
    }



    #region UI

    public void OnItemCrafted(CraftStationInventory _craftStationInventory)
    {
        if (!this.enabled) return;

        RefreshQueue(_craftStationInventory);
        RefreshRequirements();
        RefreshAllCraftableItemRequirements();
    }

    public void RefreshUI()
    {
        RefreshCraftableItemSlots();
        RefreshAllCraftableItemRequirements();

        if(selectedItem != null)
        {
            RefreshRequirements();
        }
    }

    private void RefreshCraftableItemSlots()
    {
        for (int i = 0; i < craftSlots.Length; i++)
        {
            try
            {
                //every slot has a child with an Image component to represent whether there's an item on that slot or not
                //every slot also has a child with a Text component to represent the amount of playerInventory.items in that slot
                craftSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                craftSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = craftStationInventory.craftableItems[i].GetItem().itemIcon;
            }
            catch
            {
                craftSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                craftSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                craftSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
        }
    }

    private void RefreshRequirements()
    {
        if (selectedItem == null) return;

        int requirementsMet = 0;

        for (int i = 0; i < selectedItem.GetNeededItems().Length; i++)
        {
            int actualAmount = Contains(selectedItem.GetNeededItems()[i]);
            int neededAmount = selectedItem.GetNeededAmounts()[i] * craftAmount;
            requirementSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = actualAmount.ToString() + " / " + neededAmount.ToString();

            if(actualAmount >= neededAmount)
            {
                requirementsMet++;
            }
        }

        if(requirementsMet == selectedItem.GetNeededItems().Length)
        {
            selectedItemIsCraftable = true;
        }
        else
        {
            selectedItemIsCraftable = false;
        }
    }

    private void RefreshAllCraftableItemRequirements()
    {
        List<SlotClass> _craftStationLeftItems = craftStationInventory.sumInventoryItemsCopy;

        for (int i = 0; i < craftStationInventory.craftableItems.Length; i++)
        {
            int requirementsMet = 0;



            for (int x = 0; x < craftStationInventory.craftableItems[i].GetNeededItems().Length; x++)
            {
                int actualAmount = ContainsInLeftItems(craftStationInventory.craftableItems[i].GetNeededItems()[x], _craftStationLeftItems);
                int neededAmount = craftStationInventory.craftableItems[i].GetNeededAmounts()[x];

                if (actualAmount >= neededAmount)
                {
                    requirementsMet++;
                }
                else
                {
                    craftSlots[i].GetComponent<Image>().color = notCraftableColor;
                    break;
                }
            }



            if (requirementsMet == craftStationInventory.craftableItems[i].GetNeededItems().Length)
            {
                craftSlots[i].GetComponent<Image>().color = craftableColor;
            }
        }
    }

    private void RefreshQueue(CraftStationInventory _craftStationInventory)
    {
        if (_craftStationInventory != craftStationInventory) return;

        for (int i = 0; i < queueSlots.Length; i++)
        {
            if(craftStationInventory.craftingQueue[i].GetItem())
            {
                //every slot has a child with an Image component to represent whether there's an item on that slot or not
                //every slot also has a child with a Text component to represent the amount of playerInventory.items in that slot
                queueSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                queueSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = craftStationInventory.craftingQueue[i].GetItem().itemIcon;

                if (craftStationInventory.craftingQueue[i].GetQuantity() > 1)
                {
                    queueSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = craftStationInventory.craftingQueue[i].GetQuantity().ToString();
                }
                else
                {
                    queueSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
                }
            }
            else
            {
                queueSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                queueSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                queueSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
        }

        RefreshUI();
    }

    public void InitializeCraftSlots()
    {
        //initialize crafting slots where craftable items are showed
        //this creates slots array and adjusts the amount of slots that are shown
        craftStationInventory.craftStationOpened = true;

        craftSlots = new GameObject[craftStationInventory.craftableItems.Length];

        for (int i = craftSlotHolder.transform.childCount - 1; i > 0; i--)
        {
            if (i > craftStationInventory.craftableItems.Length - 1)
            {
                craftSlotHolder.transform.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                craftSlotHolder.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < craftStationInventory.craftableItems.Length; i++)
        {
            craftSlots[i] = craftSlotHolder.transform.GetChild(i).gameObject;
        }


        queueSlots = new GameObject[craftingQueueSlotsHolder.transform.childCount];

        for (int i = 0; i < craftingQueueSlotsHolder.transform.childCount; i++)
        {
            queueSlots[i] = craftingQueueSlotsHolder.transform.GetChild(i).gameObject;
        }


        craftStationInventory.queueSlots = new GameObject[craftingQueueSlotsHolder.transform.childCount];

        for (int i = 0; i < craftingQueueSlotsHolder.transform.childCount; i++)
        {
            craftStationInventory.queueSlots[i] = craftingQueueSlotsHolder.transform.GetChild(i).gameObject;
        }

        RefreshCraftableItemSlots();
        RefreshQueue(craftStationInventory);

    }

    private void InitializeRequirements()
    {
        requirementSlots = new GameObject[requirementSlotsHolder.transform.childCount];

        for (int i = requirementSlotsHolder.transform.childCount - 1; i > 0; i--)
        {
            if (i > selectedItem.GetNeededItems().Length - 1)
            {
                requirementSlotsHolder.transform.GetChild(i).gameObject.SetActive(false);
            }
            else
            {
                requirementSlotsHolder.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        for (int i = 0; i < requirementSlotsHolder.transform.childCount; i++)
        {
            requirementSlots[i] = requirementSlotsHolder.transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < selectedItem.GetNeededItems().Length; i++)
        {
            try
            {
                //every slot has a child with an Image component to represent whether there's an item on that slot or not
                //every slot also has a child with a Text component to represent the amount of playerInventory.items in that slot
                requirementSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = true;
                requirementSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = selectedItem.GetNeededItems()[i].GetItem().itemIcon;
            }
            catch
            {
                craftSlots[i].transform.GetChild(0).GetComponent<Image>().sprite = null;
                craftSlots[i].transform.GetChild(0).GetComponent<Image>().enabled = false;
                craftSlots[i].transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = "";
            }
        }

    }

    #endregion UI




    #region OnClick

    private CraftableItemClass GetClosestCraftableItem()
    {
        for (int i = 0; i < craftSlots.Length; i++)
        {
            if (Vector2.Distance(craftSlots[i].transform.position, Input.mousePosition) <= 50)
            {
                craftSlotSelector.SetActive(true);
                craftSlotSelector.transform.position = craftSlots[i].transform.position;
                requirementSlotsHolder.SetActive(true);

                return craftStationInventory.craftableItems[i];
            }
        }

        return null;
    }

    private void SelectItem()
    {
        CraftableItemClass _closestItem = GetClosestCraftableItem();

        if(_closestItem != null)
        {
            if(selectedItem != null && selectedItem != _closestItem)
            {
                craftAmount = 1;
                craftAmountText.text = craftAmount.ToString();
            }

            selectedItem = _closestItem;
            InitializeRequirements();
            RefreshRequirements();
        }
    }

    public void ChangeCraftAmount(bool increase)
    {
        if (selectedItem == null) return;

        if (increase)
        {
            craftAmount++;
        }
        else if(craftAmount != 1)
        {
            craftAmount--;
        }

        RefreshRequirements();
        craftAmountText.text = craftAmount.ToString();
    }

    private void CheckForMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SelectItem();
        }
    }

    public void CancelQueue()
    {
        if(craftStationInventory != null)
        {
            if (craftStationInventory.craftingQueue[0].GetQuantity() > 1)
            {
                craftStationInventory.CancelQueue(false);
                RefreshQueue(craftStationInventory);
            }

            craftStationInventory.GetListOfLeftItems();
            RefreshAllCraftableItemRequirements();
        }
    }

    #endregion OnClick




    #region Accessing CraftStationInventory
    private void ContainsList(ItemClass _compareItem, out List<SlotClass> _items)
    {
        _items = new List<SlotClass>();

        foreach (SlotClass slot in craftStationInventory.items)
        {
            if (slot.GetItem() == _compareItem)
            {
                _items.Add(slot);
            }
        }
    }

    private int Contains(ItemClass _compareItem)
    {
        int _itemAmount = 0;

        foreach (SlotClass slot in craftStationInventory.items)
        {
            if (slot.GetItem() == _compareItem)
            {
                _itemAmount += slot.GetQuantity();
            }
        }

        return _itemAmount;
    }

    private int ContainsInLeftItems(ItemClass _compareItem, List<SlotClass> _leftItems)
    {
        int _itemAmount = 0;

        foreach (SlotClass slot in _leftItems)
        {
            if (slot.GetItem() == _compareItem)
            {
                _itemAmount += slot.GetQuantity();
            }
        }

        return _itemAmount;
    }

    #endregion Accessing CraftStationInventory
    
}

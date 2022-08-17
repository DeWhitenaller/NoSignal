using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerBase : MonoBehaviour
{
    private ItemClass itemFromOre;

    private Inventory inventory;

    public Sender connectedSender;

    private InventoryManager inventoryManager;

    private AudioSource audioSource;



    [SerializeField] 
    private float miningSpeed;

    [SerializeField] 
    private int amountPerTick;

    private int ticks;



    private bool inventoryIsOpen;
    public bool isConnectedToSender;





    #region Unity Methods

    private void Start()
    {
        if (!audioSource)
        {
            if (!TryGetComponent<AudioSource>(out audioSource))
            {
                Debug.LogError("AudioSource missing!", gameObject);
            }
        }

        inventory = GetComponent<Inventory>();
        itemFromOre = GetComponentInParent<MinableOre>().ore;
        StartCoroutine(MineTimer());
    }

    private void OnEnable()
    {
        InventoryManager.OnChestOpen += OnInventoryOpen;
        InventoryManager.OnChestClose += OnInventoryClose;
    }

    private void OnDisable()
    {
        InventoryManager.OnChestOpen -= OnInventoryOpen;
        InventoryManager.OnChestClose -= OnInventoryClose;
    }

    #endregion Unity Methods




    private IEnumerator MineTimer()
    {
        yield return new WaitForSeconds(miningSpeed);

        ticks++;

        if (inventoryIsOpen)
        {
            inventoryManager.RefreshUI();
        }

        UpdateInventory();

        StartCoroutine(MineTimer());
    }


    public void UpdateInventory()
    {
        int amountObtained = ticks * amountPerTick;

        if (isConnectedToSender)
        {
            SlotClass itemSlotToSend = new SlotClass(itemFromOre, amountObtained);
            if(connectedSender.CheckReceiverInventories(itemSlotToSend, out SlotClass _updatedItemSlot))
            {
                //items got stored successfully
            }
            else
            {
                //items could not be completely stored, put them in own inventory
                PutItemsInOwnInventory(_updatedItemSlot.GetQuantity());
            }
        }
        else
        {
            PutItemsInOwnInventory(amountObtained);
        }

        ticks = 0;
    }


    private void PutItemsInOwnInventory(int _amountObtained)
    {
        for (int i = 0; i < inventory.items.Length; i++)
        {
            if (inventory.items[i].GetItem() == itemFromOre)
            {
                int amountLeftInSlot = inventory.items[i].GetItem().maxStackSize - inventory.items[i].GetQuantity();

                if(_amountObtained > amountLeftInSlot)
                {
                    inventory.items[i].AddQuantity(amountLeftInSlot);
                    _amountObtained -= amountLeftInSlot;
                }
                else
                {
                    inventory.items[i].AddQuantity(_amountObtained);
                    _amountObtained = 0;
                }
            }
            else if (inventory.items[i].GetItem() == null)
            {
                if (_amountObtained > itemFromOre.maxStackSize)
                {
                    inventory.items[i].AddItem(itemFromOre, itemFromOre.maxStackSize);
                    _amountObtained -= itemFromOre.maxStackSize;
                }
                else
                {
                    inventory.items[i].AddItem(itemFromOre, _amountObtained);
                    _amountObtained = 0;
                }
            }

            if (_amountObtained == 0)
            {
                break;
            }
        }
    }




    #region Open / Close

    private void OnInventoryOpen(InventoryManager _inventoryManager)
    {
        inventoryIsOpen = true;
        inventoryManager = _inventoryManager;
    }

    private void OnInventoryClose(InventoryManager _inventoryManager)
    {
        inventoryIsOpen = false;
    }

    #endregion Open / Close
}

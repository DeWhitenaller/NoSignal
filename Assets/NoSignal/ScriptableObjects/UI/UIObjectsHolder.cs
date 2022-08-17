using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIObjectsHolder : MonoBehaviour
{
    [Header("Chest UI")]
    public GameObject chestInventoryObject;
    public GameObject chestInventoryCursor;
    public GameObject chestPlayerSlotsHolder;
    public GameObject chestSlotsHolder;

    [Header("CraftStationClass UI")]
    public GameObject craftStationInventoryObject;
    public GameObject craftStationInventoryCursor;
    public GameObject craftStationPlayerSlotsHolder;
    public GameObject craftStationChestSlotsHolder;
    public Slider craftProgressBar;
    public GameObject craftSlotsHolder;
    public GameObject craftingQueueSlotsHolder;
    public GameObject craftSlotSelector;
    public GameObject requirementSlotsHolder;
    public TextMeshProUGUI craftAmountText;

    [Header("NPC Trading UI")]
    public GameObject npcInventoryObject;
    public GameObject npcInventoryCursor;
    public GameObject npcPlayerSlotsHolder;
    public GameObject tradableSlotsHolder;
    public GameObject tradableSlotSelector;
    public GameObject npcRequirementSlotsHolder;
    public TextMeshProUGUI tradeAmountText;

    [Header("Sender UI")]
    public TextMeshProUGUI senderChannelText;
}

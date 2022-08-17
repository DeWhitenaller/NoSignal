using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipItemHandler : MonoBehaviour
{
    /// <summary>
    /// this class reacts to events that get called from EquippableBase
    /// it keeps track of the currently equipped Item and handles the equipping itself
    /// it triggers the "use animation" of the item and EquippableTool reacts to that
    /// </summary>



    [SerializeField] 
    private InventoryManager inventoryManager;

    [SerializeField] 
    private ItemClass itemToEquip;

    [SerializeField] 
    private EquippableBase currentEquippedItemScript;



    [SerializeField] 
    private Animator anim;



    [SerializeField] 
    private bool canEquipItem = true;






    #region Unity Methods

    private void OnEnable()
    {
        EquippableBase.OnPrimaryUse += TriggerAnimation;
        EquippableBase.OnPrimaryEnd += OnAnimationEnd;
        EquippableBase.OnSpawn += SetCurrentItemScript;
    }

    private void OnDisable()
    {
        EquippableBase.OnPrimaryUse -= TriggerAnimation;
        EquippableBase.OnPrimaryEnd -= OnAnimationEnd;
        EquippableBase.OnSpawn -= SetCurrentItemScript;
    }

    #endregion Unity Methods




    #region EquipItem

    public void TryToEquipItem(ItemClass _itemToEquip)
    {
        if (canEquipItem)
        {
            EquipItem(_itemToEquip);
        }
        else
        {
            itemToEquip = _itemToEquip;
            currentEquippedItemScript.isUsable = false;
            StartCoroutine(CheckIfItemCanBeEquipped());
        }
    }

    IEnumerator CheckIfItemCanBeEquipped()
    {
        yield return new WaitForSeconds(0.5f);

        TryToEquipItem(itemToEquip);
    }

    public void EquipItem(ItemClass _itemToEquip)
    {
        inventoryManager.EquipSelectedItem();
    }

    public void UnequipItem()
    {
        anim.SetLayerWeight(1, 0f);
    }

    #endregion EquipItem




    #region Animation

    private void TriggerAnimation()
    {
        anim.SetLayerWeight(1, 1f);
        anim.SetTrigger("PrimaryUse");
        canEquipItem = false;
    }

    private void OnAnimationEnd()
    {
        anim.SetLayerWeight(1, 0f);
        canEquipItem = true;
    }

    #endregion Animation



    public void SetCurrentItemScript(EquippableBase _currentItemScript)
    {
        currentEquippedItemScript = _currentItemScript;
        currentEquippedItemScript.isUsable = true;
    }

}

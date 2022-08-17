using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippableBase : MonoBehaviour
{
    /// <summary>
    /// if the player gives mouse input, this class will execute OnPrimaryUseEvent()
    /// then it sends an event so the EquipItemHandler can react to it whenever an item gets used and/or finished its "use animation"
    /// </summary>


    public delegate void Action();
    public static event Action OnPrimaryUse;
    public static event Action OnPrimaryEnd;


    public delegate void Spawn(EquippableBase _equippedItem);
    public static event Spawn OnSpawn;


    public bool isUsable = true;





    protected virtual void OnPrimaryUseEvent()
    {
        OnPrimaryUse?.Invoke();
    }
    protected virtual void OnPrimaryEndEvent()
    {
        OnPrimaryEnd?.Invoke();
    }
    protected virtual void OnSpawnEvent(EquippableBase _equippedItem)
    {
        OnSpawn?.Invoke(_equippedItem);
    }
}

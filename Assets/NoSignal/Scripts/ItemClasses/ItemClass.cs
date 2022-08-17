using System.Collections;
using UnityEngine;

public abstract class ItemClass : ScriptableObject
{
    public enum State
    {
        Tool,
        Misc,
        Consumable,
        Structure,
        Resource,
        Plant
    }



    [Header("Item")]
    public string itemName;

    public State type;

    public int itemID;

    public Sprite itemIcon;

    public bool isStackable = true;

    public int maxStackSize;




    [Header("References")]
    public GameObject dropReference;
    public GameObject equipReference;
    public GameObject plantGrowObject;




    public abstract ItemClass GetItem();

    public abstract ToolClass GetTool();

    public abstract MiscClass GetMisc();

    public abstract ConsumableClass GetConsumable();

    public abstract StructureClass GetStructure();

    public abstract RessourceClass GetRessource();

    public abstract MeleeClass GetMeleeWeapon();

    public abstract GunClass GetGun();

    public abstract AmmoClass GetAmmo();

}

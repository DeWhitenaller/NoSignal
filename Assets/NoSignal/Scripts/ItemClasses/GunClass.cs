using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Gun Class", menuName = "Item/Gun")]
public class GunClass : ItemClass
{
    public enum AmmoType
    {
        TranqDart,
        Rifle,
        Pistol
    }


    public AmmoType ammoType;

    public float range;

    public float damage;

    public int magSize;

    public GameObject bulletPrefab;






    public override ItemClass GetItem() { return this; }

    public override ToolClass GetTool() { return null; }

    public override MiscClass GetMisc() { return null; }

    public override ConsumableClass GetConsumable() { return null; }

    public override StructureClass GetStructure() { return null; }

    public override RessourceClass GetRessource() { return null; }

    public override MeleeClass GetMeleeWeapon() { return null; }

    public override GunClass GetGun() { return this; }

    public override AmmoClass GetAmmo() { return null; }

}

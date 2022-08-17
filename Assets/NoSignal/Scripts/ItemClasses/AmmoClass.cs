using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Ammo Class", menuName = "Item/Ammo")]
public class AmmoClass : ItemClass
{
    public override ItemClass GetItem() { return this; }

    public override ToolClass GetTool() { return null; }

    public override MiscClass GetMisc() { return null; }

    public override ConsumableClass GetConsumable() { return null; }

    public override StructureClass GetStructure() { return null; }

    public override RessourceClass GetRessource() { return null; }

    public override MeleeClass GetMeleeWeapon() { return null; }

    public override GunClass GetGun() { return null; }

    public override AmmoClass GetAmmo() { return this; }

}

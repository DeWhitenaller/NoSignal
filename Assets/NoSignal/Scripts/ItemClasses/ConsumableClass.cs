using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "new Consumable Class", menuName = "Item/Consumable")]
public class ConsumableClass : ItemClass
{
    [Header("Consumable")]
    public float healthAdded;

    public override ItemClass GetItem() { return this; }

    public override ToolClass GetTool() { return null; }

    public override MiscClass GetMisc() { return null; }

    public override ConsumableClass GetConsumable() { return this; }

    public override StructureClass GetStructure() { return null; }

    public override RessourceClass GetRessource() { return null; }

    public override MeleeClass GetMeleeWeapon() { return null; }

    public override GunClass GetGun() { return null; }

    public override AmmoClass GetAmmo() { return null; }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Structure Class", menuName = "Item/Structure")]
public class StructureClass : ItemClass
{
    public enum StructureType
    {
        foundation,
        wall,
        ceiling,
        craftStation,
        chest
    }


    [Header("Structure")]

    public StructureType structureType;

    public GameObject structureObjectReference;

    public GameObject structureObjectPreviewReference;

    public LayerMask snapPointLayer, solidLayer, previewGetsInterruptedBy;

    public bool canBeRotated;





    public override ItemClass GetItem() { return this; }

    public override ToolClass GetTool() { return null; }

    public override MiscClass GetMisc() { return null; }

    public override ConsumableClass GetConsumable() { return null; }

    public override StructureClass GetStructure() { return this; }

    public override RessourceClass GetRessource() { return null; }

    public override MeleeClass GetMeleeWeapon() { return null; }

    public override GunClass GetGun() { return null; }

    public override AmmoClass GetAmmo() { return null; }

}

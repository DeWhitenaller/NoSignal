using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceHealth : HealthBase
{
    [SerializeField] 
    protected DestroyableBase destroyableObject;




    public override void Start()
    {
        base.Start();
        destroyableObject = GetComponent<DestroyableBase>();
    }

    protected override void OnHealthUnderZero()
    {
        destroyableObject.OnDeath();
    }

    protected override void Hit()
    {
        destroyableObject.OnHit();
    }
}

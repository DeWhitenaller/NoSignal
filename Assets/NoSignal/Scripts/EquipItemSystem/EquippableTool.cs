using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquippableTool : EquippableBase
{
    /// <summary>
    /// this class reacts to player input
    /// </summary>
    
    
    [SerializeField] 
    protected ToolClass toolStats;

    protected Collider[] hittedObjects;

    [SerializeField] 
    protected GameObject hitPoint;

    [SerializeField] 
    protected LayerMask hitMask;



    protected bool active, attacking;




    #region Unity Methods

    protected virtual void OnEnable()
    {
        OnSpawnEvent(this);

        PlayerAnimEvents.OnPrimaryUse += CreateHitSphere;
        PlayerAnimEvents.OnPrimaryEnd += OnAttackAnimationFinished;
    }

    protected virtual void OnDisable()
    {
        PlayerAnimEvents.OnPrimaryUse -= CreateHitSphere;
        PlayerAnimEvents.OnPrimaryEnd -= OnAttackAnimationFinished;
    }

    protected virtual void Update()
    {
        if (!isUsable) return;

        if (Input.GetMouseButton(0) && !attacking)
        {
            OnAttackButtonPressed();
        }
    }

    #endregion Unity Methods



    #region Item Functionality

    protected virtual void CreateHitSphere()
    {
        Collider[] hits = Physics.OverlapSphere(hitPoint.transform.position, toolStats.hitRadius, hitMask);

        if (hits.Length > 0)
        {
            DamageOpponent(hits);
        }
    }

    protected virtual void DamageOpponent(Collider[] _hittingColliders)
    {
        _hittingColliders[0].gameObject.GetComponent<HealthBase>().ChangeHealth(-toolStats.damage);
    }

    #endregion Item Functionality



    protected virtual void OnAttackButtonPressed()
    {
        attacking = true;
        OnPrimaryUseEvent();
    }

    protected virtual void OnAttackAnimationFinished()
    {
        attacking = false;
        OnPrimaryEndEvent();
    }

}

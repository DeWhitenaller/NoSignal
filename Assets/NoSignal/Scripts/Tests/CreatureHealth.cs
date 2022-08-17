using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureHealth : HealthBase
{
    public delegate void Action(GameObject objectThatDied, GameObject objectThatKilled);
    public event Action OnDeath;



    public virtual void ChangeHealth(float _healthChange, GameObject _hittingObject)
    {
        health += _healthChange;

        if (health <= 0)
        {
            OnDeath?.Invoke(gameObject, _hittingObject);
            OnHealthUnderZero();
        }
        else
        {
            Hit();
        }
    }

    protected override void OnHealthUnderZero()
    {
        Destroy(gameObject);
    }

    protected override void Hit()
    {

    }
}

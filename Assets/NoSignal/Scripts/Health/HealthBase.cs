using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBase : MonoBehaviour
{
    /// <summary>
    /// simple Health Class with some functionality to decrease or increase it's health
    /// </summary>


    [SerializeField] 
    protected float health;

    [SerializeField] 
    protected float maxHealth;




    public virtual void Start()
    {
        health = maxHealth;
    }

    public virtual void ChangeHealth(float healthChange)
    {
        health += healthChange;

        if (health <= 0)
        {
            OnHealthUnderZero();
        }
        else
        {
            Hit();
        }
    }

    protected virtual void OnHealthUnderZero()
    {

    }

    protected virtual void Hit()
    {

    }
}

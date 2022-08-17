using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour
{
    /// <summary>
    /// these methods get called via animation events and lets an equipped item execute it's ability 
    /// (for example creating a hit sphere if it's a tool)
    /// </summary>


    public delegate void Action();
    public static event Action OnPrimaryUse;
    public static event Action OnPrimaryEnd;



    public void UsePrimary()
    {
        OnPrimaryUse?.Invoke();
    }

    public void UseEnd()
    {
        OnPrimaryEnd?.Invoke();
    }
}

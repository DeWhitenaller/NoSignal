using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEvents : MonoBehaviour
{
    /// <summary>
    /// sends events on specific UI Buttons
    /// </summary>
    
    public delegate void Action();
    public static event Action OnIncrease;
    public static event Action OnDecrease;
    public static event Action OnSetChannel;

    public void Increase()
    {
        OnIncrease?.Invoke();
    }
    public void Decrease()
    {
        OnDecrease?.Invoke();
    }
    public void SetChannel()
    {
        OnSetChannel?.Invoke();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorLock : MonoBehaviour
{
    [SerializeField] 
    private bool lockMouse = true;




    private void Awake()
    {
        if(lockMouse)
            Cursor.lockState = CursorLockMode.Locked;
    }
}

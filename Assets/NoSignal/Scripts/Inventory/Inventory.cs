using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public SlotClass[] items;

    [HideInInspector] 
    public int maxSize;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropSpawn : MonoBehaviour
{
    public GameObject drop;



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Instantiate(drop, transform.position, Quaternion.identity);
        }
    }
}

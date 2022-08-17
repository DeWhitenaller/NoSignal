using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShrinkGrowth : MonoBehaviour
{
    private Vector3 scaleBig;
    private Vector3 scaleSmall;
    private void Start()
    {
        scaleBig = new Vector3(1f, 1f, 1f);
        scaleSmall = new Vector3(0.2f, 0.2f, 0.2f);
    }
    private void Update()
    {
        if (Input.GetKeyDown("s"))
        {
            Plantshrink();
        }
        if (Input.GetKeyDown("g"))
        {
            PlantGrow();
        }
    }
    // Shrink the Plant
    void Plantshrink()
    {
        transform.localScale = scaleSmall;
    }

    // Lets the Plant grow
    void PlantGrow()
    {
        transform.localScale = scaleBig;
    }
}

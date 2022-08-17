using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Randomizer : MonoBehaviour
{
    public GameObject[] plants;
    public int objNum;
    public int objCount;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            objNum = Random.Range(0,6);
            objCount = 0;
            while (objCount < 6)
            {
                plants[objCount].SetActive(false);
                objCount+= 1;
            }
            plants[objNum].SetActive(true);
        }
    }
}

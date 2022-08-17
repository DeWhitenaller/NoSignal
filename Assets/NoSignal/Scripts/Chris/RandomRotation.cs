using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotation : MonoBehaviour
{
    public new Transform transform;
    Vector3 lastPos;
    Vector3 newPos;
    float t;

    void Start()
    {
        lastPos = transform.eulerAngles;
        NewAngle();
    }

    void Update()
    {
        transform.eulerAngles += new Vector3(0, 1, 0);
        transform.eulerAngles = Vector3.Lerp(lastPos, newPos, t);
        t += 0.01f;
        if (t > 1)
            NewAngle();
    }

    void NewAngle()
    {
        lastPos = newPos;
        newPos = new Vector3(
                     Random.Range(-10f, 10f),
                     Random.Range(0f, 360f),
                     Random.Range(-10f, 10f));
        t = 0;
    }
}
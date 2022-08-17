using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TrailerCamera : MonoBehaviour
{
    [SerializeField] 
    private GameObject camPoint;
    
    [SerializeField] 
    private GameObject[] camPoints;
    
    [SerializeField] 
    private GameObject[] camSpawnPoints;


    
    [SerializeField] 
    private float speed, rotateAngle, doMoveTime;
    
    [SerializeField] 
    private bool lerpToCamPoint, rotateAround, doMoveToCamPoint;





    void Start()
    {
        if (doMoveToCamPoint)
        {
            StartCoroutine(TrailerAnimation());
        }
    }

    void Update()
    {
        if (lerpToCamPoint)
        {
            transform.position = Vector3.Lerp(transform.position, camPoint.transform.position, Time.deltaTime * speed);
        }
        else if (rotateAround)
        {
            transform.LookAt(camPoint.transform.position);
            transform.RotateAround(camPoint.transform.position, Vector3.up, rotateAngle);
        }
    }

    IEnumerator TrailerAnimation()
    {
        transform.position = camSpawnPoints[0].transform.position;
        transform.rotation = camSpawnPoints[0].transform.rotation;
        transform.DOMove(camPoints[0].transform.position, doMoveTime);

        yield return new WaitForSeconds(doMoveTime);

        transform.position = camSpawnPoints[1].transform.position;
        transform.rotation = camSpawnPoints[1].transform.rotation;
        transform.DOMove(camPoints[1].transform.position, doMoveTime);

        yield return new WaitForSeconds(doMoveTime);
     
        transform.position = camSpawnPoints[2].transform.position;
        transform.rotation = camSpawnPoints[2].transform.rotation;
        transform.DOMove(camPoints[2].transform.position, doMoveTime);
    }
}

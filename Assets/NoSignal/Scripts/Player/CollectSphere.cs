using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectSphere : MonoBehaviour
{
    /// <summary>
    /// creates a OverlapSphere to suck in all collectable items near the player
    /// </summary>
    
    
    
    [SerializeField] private Collider[] sphereHits;

    [SerializeField] private LayerMask objectMask;


    [SerializeField] private float sphereRadius;

    public bool sphereActive;





    void Update()
    {
        if (!sphereActive) return;

        // OverlapSphere registers all interactables in a radius of the player
        sphereHits = Physics.OverlapSphere(gameObject.transform.position, sphereRadius, objectMask);

        // if there is any Object near the player
        if (sphereHits.Length != 0)
        {
            foreach (var item in sphereHits)
            {
                item.GetComponent<InteractableDrops>().OnSight();
            }
        }
    }
}
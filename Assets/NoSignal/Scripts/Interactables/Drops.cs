using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class Drops : InteractableDrops
{
    protected InventoryManager inventoryManager;

    [SerializeField] 
    protected ItemClass itemReference;



    public int quantity;


    protected bool triedToCollect;





    public override void Start()
    {
        base.Start();
        StartCoroutine(CollectableCooldown());
    }

    public virtual IEnumerator CollectableCooldown()
    {
        yield return new WaitForSeconds(1);
        collectable = true;
    }

    public override void OnSight()
    {
        if (collectable)
        {
            if (!collecting)
            {
                collecting = true;
            } 
        }
    }

    public override void Update()
    {
        Rotate();

        if (collecting)
        {
            MoveToPlayer();


            if (Vector3.Distance(transform.position, player.transform.position) < destroyDistance && !collected)
            {
                //PlaySound(sounds);
                inventoryManager = GameObject.FindGameObjectWithTag("Player").GetComponent<InventoryManager>();

                collected = inventoryManager.AddDropItem(this, itemReference, quantity);

                if (!collected)
                {
                    NotCollectable();
                }
                else
                {
                    DestroyMe();
                }
            }
        }
    }

    public virtual void Rotate()
    {
        transform.Rotate(rotateVector * rotationSpeed * Time.deltaTime);
    }

    public virtual void MoveToPlayer()
    {
        Vector3 dir = player.transform.position - transform.position;
        transform.position += dir.normalized * collectSpeed * Time.deltaTime;
    }

    public virtual void NotCollectable()
    {
        collectable = false;
        collecting = false;
        triedToCollect = false;
    }

    public virtual void DestroyMe()
    {
        Destroy(gameObject);
    }
}

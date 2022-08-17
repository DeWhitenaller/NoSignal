using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class InteractableDrops : InteractableBase
{

    protected GameObject player;



    [SerializeField] 
    protected float collectSpeed;

    [SerializeField] 
    protected float destroyDistance;

    [SerializeField] 
    protected float rotationSpeed;

    [SerializeField] 
    protected Vector3 rotateVector;



    public bool collectable;
    public bool collecting;
    public bool collected;





    public override void Start()
    {
        base.Start();
        collected = false;
        collecting = false;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public virtual void Update()
    {
        transform.Rotate(rotateVector * rotationSpeed * Time.deltaTime);

        if (!collecting) return;

        transform.DOMove(player.transform.position, collectSpeed);

        if (Vector3.Distance(transform.position, player.transform.position) < destroyDistance && !collected)
        {
            collected = true;
            if (audioSource) PlayRandomAudio();
            Destroy(gameObject);
        }
    }

    public override void OnSight()
    {
        if (collecting) return;


        ShowInteractIcon();
    }

    public override void OnInteract()
    {
        if (collecting) return;

        Interaction();
    }


    public override void Interaction()
    {
        collecting = true;
    }
}

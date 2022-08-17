using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropEffect : MonoBehaviour
{
    private InteractableDrops dropScript;

    private Rigidbody rb;
    
    private Vector3 velocity = Vector3.up;



    [SerializeField] 
    private LayerMask groundLayer;

    [SerializeField] 
    private float gravity;

    [SerializeField] 
    private float velocityLimit;

    [SerializeField] 
    private float jumpHeightMin;

    [SerializeField] 
    private float jumpHeightMax;

    [SerializeField] 
    private float spread;

    [SerializeField] 
    private float stayHeight;

    [SerializeField] 
    private float delayUntilDropIsCollectable;



    bool checkGround = false;





    public void Start()
    {
        dropScript = GetComponent<InteractableDrops>();
        velocity *= Random.Range(jumpHeightMin, jumpHeightMax);
        velocity += new Vector3(Random.Range(-spread, spread), 0, Random.Range(-spread, spread));
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false;
    }

    public void Update()
    {
        if (rb == null) return;

        velocity.y -= gravity;

        rb.position += velocity * Time.deltaTime;


        if (velocity.y < -velocityLimit) velocity.y = -velocityLimit;
        else velocity -= Vector3.up * 5 * Time.deltaTime;


        if (!checkGround && velocity.y <= 0f)
        {
            checkGround = true;
        }

        if (!checkGround) return;

        if (Physics.Raycast(gameObject.transform.position, Vector3.down, stayHeight, groundLayer))
        {
            rb.isKinematic = true;
            Destroy(rb);
            StartCoroutine("Delay");
        }
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(delayUntilDropIsCollectable);
        dropScript.collectable = true;
        Destroy(this);
    }

}

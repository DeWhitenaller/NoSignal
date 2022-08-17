using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CreatureSoloBehaviour : MonoBehaviour
{
    public enum State
    {
        Idle,
        Roaming,
        Watching
    }

    public State currentState;

    public Transform player;
    protected float randomIdleTime, detectTimer;

    public LayerMask groundMask, playerMask;
    public CharacterController controller;
    public Animator anim;
    
    [SerializeField] protected bool isGrounded;
    protected Vector3 velocity, distanceToTarget;
    public float gravity, rotateSpeed, speed, attackRange, lookRange, groundDistance, roamRange;
    public GameObject randomPointOrigin;

    protected Vector3 roamPosition;

    GameObject[] list;

    public virtual void Awake()
    {
        list = new GameObject[0];
        list = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < list.Length; i++)
        {
            Debug.Log(list[i]);
        }
    }

    public virtual void OnEnable()
    {
        ChangeState(State.Idle);
    }

    public virtual void Start()
    {
        ChangeState(State.Idle);
    }

    public virtual void Update()
    {
        CheckIfGrounded();
        ApplyPhysics();
        CheckCurrentState();
    }

    public virtual void OnDisable()
    {
        anim.SetBool("isRunning", false);
    }

    public virtual void CheckIfGrounded()
    {
        //send a raycast downwards to check if the object is grounded
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit rayHit, groundDistance, groundMask);

        if (isGrounded)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.FromToRotation(transform.up, rayHit.normal) * transform.rotation, Time.deltaTime * 15f);
        }
        //apply gravity if grounded
        //if (isGrounded && velocity.y < 0)
        //{
        //    velocity.y = -2;
        //}
    }

    public virtual void ApplyPhysics()
    {
        //apply gravity to the CharacterController
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    

    public virtual void RotateToRoamPosition(Vector3 _position)
    {
        distanceToTarget = _position - transform.position;
        float rotationStep = rotateSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, distanceToTarget, rotationStep, 0.0f);
        newDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    public virtual void CheckPlayer()
    {
        Collider[] col = Physics.OverlapSphere(transform.position, lookRange, playerMask);
        if (col.Length != 0)
        {
            ChangeState(State.Watching);
        }
        else
        {
            Debug.Log("Hi");
            if (currentState == State.Watching)
            {
                ChangeState(State.Idle);
                Debug.Log("LOL");
            }
            
        }
        
    }
    
    public virtual void MoveToRoamPosition()
    {
        if (distanceToTarget.magnitude > attackRange)
        {
            controller.Move(transform.forward * speed * Time.deltaTime);
        }
        else
        {
            ChangeState(State.Idle);
        }
    }

    public virtual void CheckCurrentState()
    {
        switch (currentState)
        {
            case State.Idle:
                IdleState();
                break;

            case State.Roaming:
                RoamingState();
                break;
            
            case State.Watching:
                WatchingState();
                break;
        }
    }

    public virtual void ChangeState(State switchTo)
    {
        switch (switchTo)
        {
            case State.Idle:
                StartCoroutine(WaitRandomIdleTime());
                anim.SetBool("isRunning", false);
                break;

            case State.Roaming:
                GetRandomRoamPosition();
                anim.SetBool("isRunning", true);
                break;
            
            case State.Watching:
                anim.SetBool("isRunning", false);
                break;
            
        }

        currentState = switchTo;
    }

    public virtual void IdleState()
    {
        detectTimer += Time.deltaTime;

        if(detectTimer >= 2)
        {
            detectTimer = 0f;
        }
        
        CheckPlayer();
    }

    public virtual void RoamingState()
    {
        RotateToRoamPosition(roamPosition);
        MoveToRoamPosition();

        detectTimer += Time.deltaTime;

        if (detectTimer >= 2)
        {
            detectTimer = 0f;
        }

        CheckPlayer();
    }

    public virtual void WatchingState()
    {
        Vector3 playerPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        //transform.LookAt(playerPos);
        RotateToRoamPosition(playerPos);
        CheckPlayer();
    }
    
    
    public virtual IEnumerator WaitRandomIdleTime()
    {
        randomIdleTime = Random.Range(0f, 5f);
        yield return new WaitForSeconds(randomIdleTime);

        if(currentState == State.Idle)
        {
            ChangeState(State.Roaming);
        }
    }

    public virtual void GetRandomRoamPosition()
    {
        float x = Random.Range(-roamRange, roamRange);
        float z = Random.Range(-roamRange, roamRange);
        Vector3 randomPosition = new Vector3(x, 0, z) + randomPointOrigin.transform.position;
        roamPosition = randomPosition;
    }
    
}


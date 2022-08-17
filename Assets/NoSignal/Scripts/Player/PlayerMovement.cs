using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public enum State
    {
        Idle,
        Walking,
        Running,
        Sprinting,
        Jumping
    }

    public enum AnimState
    {
        Idle,
        RunningForwards,
        RunningBackwards,
        RunningLeft,
        RunningRight
    }

    public State currentState;

    public AnimState currentAnimState;




    [SerializeField]
    private KeyCode jumpKey, primaryUseKey;

    [SerializeField]
    private CharacterController controller;

    [SerializeField] 
    private Animator anim;

    [SerializeField]
    private InputActionAsset playerControls;

    private PlayerInput playerInput;

    private InputAction moveAction;




    [SerializeField] 
    private GameObject cam;

    [SerializeField] 
    private LayerMask groundMask;

    private RaycastHit slopeHit;

    private Vector3 velocity, movement;




    private float slopeSpeed = 2f;

    private float slopeAngle;

    public float walkSpeed, sprintSpeed, speed, rotationSpeed, jumpStrength, groundDistance, gravity, tempGravityMultiplier, turnSmoothTime, groundCheckRadiusMultiplier;
   


    [SerializeField] 
    private bool isGrounded, isAiming, jumpKeyPressed, sprinting;

    public bool movementDisabled;









    #region Unity Methods

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions["Move"];
    }

    void Start()
    {
        currentAnimState = AnimState.Idle;
    }

    void Update()
    {
        CheckMoveInput();
        ApplyPhysics();
        CheckCurrentState();

        if (OnSteepSlope())
        {
            SteepSlopeMovement();
        }
        else
        {
            isGrounded = CheckIfGrounded();
            slopeSpeed = 0f;

            ApplyMovement();

            if(currentState != State.Idle)
            {
                RotatePlayer();
            }
        }
    }

    #endregion Unity Methods





    #region Input and Physics

    public void DisableMovement()
    {
        movement = Vector3.zero;
        movementDisabled = true;
        ChangeState(State.Idle);
        ChangeAnimState(AnimState.Idle);
    }


    void CheckMoveInput()
    {
        if (movementDisabled) return;

        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        movement = new Vector3(moveInput.x, 0, moveInput.y);

        CheckMovementValueForAnimationState();
    }


    private void CheckMovementValueForAnimationState()
    {
        if (movement.x == 0 && movement.z == 0)
        {
            ChangeAnimState(AnimState.Idle);
        }
        else
        {
            if (movement.x < -0.3f)
            {
                ChangeAnimState(AnimState.RunningLeft);
            }
            else if (movement.x > 0.3f)
            {
                ChangeAnimState(AnimState.RunningRight);
            }
            else
            {
                if (movement.z < 0)
                {
                    ChangeAnimState(AnimState.RunningBackwards);
                }
                else
                {
                    ChangeAnimState(AnimState.RunningForwards);
                }
            }
        }
    }


    void RotatePlayer()
    {
        Quaternion toRotation = Quaternion.LookRotation(movement, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
    }


    void CheckJumpInput()
    {
        jumpKeyPressed = Input.GetKeyDown(jumpKey);

        if (isGrounded)
        {
            if (jumpKeyPressed)
            {
                //OnJump();
            }
        }
    }


    void ApplyPhysics()
    {
        velocity.y += gravity * Time.deltaTime;
        
        controller.Move(velocity * Time.deltaTime);
    }


    void ApplyMovement()
    {
        //Vector3 camRot = new Vector3(0, cam.transform.rotation.y, 0);
        Vector3 fakeForward = cam.transform.forward;
        fakeForward.y = 0.0f;

        movement = movement.x * cam.transform.right.normalized + movement.z * fakeForward.normalized;
        controller.Move(movement * speed * Time.deltaTime);
    }


    private bool OnSteepSlope()
    {
        
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, (controller.height / 2) + 1))
        {
            slopeSpeed += 0.7f * Time.deltaTime;
            Mathf.Clamp(slopeSpeed, 0, 10);
            slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);
            if (slopeAngle > controller.slopeLimit) return true;
        }
        return false;
    }


    private void SteepSlopeMovement()
    {
        slopeSpeed += Time.deltaTime;
        slopeSpeed = Mathf.Clamp(slopeSpeed, 0f, 5f);

        Vector3 slopeDirection = Vector3.up - slopeHit.normal * Vector3.Dot(Vector3.up, slopeHit.normal);

        movement = slopeDirection * -speed;
        movement.y = movement.y - slopeHit.point.y;

        controller.Move(movement * slopeSpeed * Time.deltaTime);

    }


    bool CheckIfGrounded()
    {
        float sphereCastRadius = controller.radius * groundCheckRadiusMultiplier;
        float sphereCastTravelDistance = controller.bounds.extents.y - sphereCastRadius + groundDistance;
        return Physics.SphereCast(transform.position, sphereCastRadius, Vector3.down, out RaycastHit rayHit, sphereCastTravelDistance, groundMask);
    }



    #endregion Input and Physics





    #region States

    void CheckCurrentState()
    {
        switch (currentState)
        {
            case State.Idle:
                IdleState();
                break;

            case State.Walking:
                WalkingState();
                break;

            case State.Running:
                RunningState();
                break;

            case State.Sprinting:
                SprintingState();
                break;

            case State.Jumping:
                JumpingState();
                break;
        }
    }

    public void ChangeState(State switchTo)
    {
        switch (switchTo)
        {
            case State.Idle:
                break;

            case State.Walking:
                break;

            case State.Running:
                break;

            case State.Sprinting:
                break;

            case State.Jumping:
                anim.SetTrigger("Jump");
                break;
        }

        currentState = switchTo;
    }

    public void ChangeAnimState(AnimState switchTo)
    {
        switch (switchTo)
        {
            case AnimState.Idle:
                anim.SetBool("isRunningForwards", false);
                anim.SetBool("isRunningBackwards", false);
                anim.SetBool("isRunningLeft", false);
                anim.SetBool("isRunningRight", false);
                break;

            case AnimState.RunningForwards:
                anim.SetBool("isRunningForwards", true);
                anim.SetBool("isRunningBackwards", false);
                anim.SetBool("isRunningLeft", false);
                anim.SetBool("isRunningRight", false);
                break;

            case AnimState.RunningBackwards:
                anim.SetBool("isRunningForwards", false);
                anim.SetBool("isRunningBackwards", true);
                anim.SetBool("isRunningLeft", false);
                anim.SetBool("isRunningRight", false);
                break;

            case AnimState.RunningLeft:
                anim.SetBool("isRunningForwards", false);
                anim.SetBool("isRunningBackwards", false);
                anim.SetBool("isRunningLeft", true);
                anim.SetBool("isRunningRight", false);
                break;

            case AnimState.RunningRight:
                anim.SetBool("isRunningForwards", false);
                anim.SetBool("isRunningBackwards", false);
                anim.SetBool("isRunningLeft", false);
                anim.SetBool("isRunningRight", true);
                break;
        }

        currentAnimState = switchTo;
    }

    public void IdleState()
    {
        if (movement != Vector3.zero)
        {
            ChangeState(State.Running);
        }
    }

    public void WalkingState()
    {
        if (movement == Vector3.zero)
        {
            ChangeState(State.Idle);
        }
    }

    public void RunningState()
    {

        if(movement == Vector3.zero)
        {
            ChangeState(State.Idle);
        }
    }

    public void SprintingState()
    {
        if(movement == Vector3.zero)
        {
            ChangeState(State.Idle);
        }
    }

    public void JumpingState()
    {
        if (isGrounded)
        {
            ChangeState(State.Idle);
        }
    }

    #endregion States





    void OnJump(InputValue value)
    {
        if (!isGrounded) return;
        //let player jump by applying a negative force to the gravity
        velocity.y = Mathf.Sqrt(jumpStrength * -2f * gravity);
        ChangeState(State.Jumping);
    }

    void OnSprint(InputValue value)
    {
        if (!sprinting)
        {
            sprinting = true;
            speed = sprintSpeed;
            anim.SetFloat("RunSpeed", 1.8f);
        }
        else
        {
            sprinting = false;
            speed = walkSpeed;
            anim.SetFloat("RunSpeed", 1f);
        }
    }

}


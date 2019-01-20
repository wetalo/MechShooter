using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure; // Required in C#

public class Movement : MonoBehaviour {
    [Space]


    private CharacterController myController = null;
    //movement
    private float myXVelocity = 0.0f;
    private float myZVelocity = 0.0f;
    private float myYVelocity = 0.0f;
    

    [Space]
    [Header("Walking Variables")]
    //walk
    public float movementSpeed = 20.0f; // 20 unit per sec
    private Vector3 moveDirection = Vector3.zero;
    private float currentMovementSpeed;

    //Gravity
    public float gravity = -9.8f; //meters per second per second
    private float terminalYVelocity = -53f; //meters per second
    public AudioClip walkClip;


    [Header("Dashing")]
    public float dashSpeed = 50.0f;
    public float dashDeceleration = -10f;
    public float dashCooldown = 2.0f;
    public float dashTime = 0.2f;
    float timeTillNextDash;
    float dashTimer;
    [SerializeField]
    AudioClip dashClip;
    public float timeBetweenStepSounds = 0.1f;
    float timeTillNextStep;

    [Header("Jumping")]
    //jump
    public float jumpYVelocity = 10f;
    public float jumpCancelYVelocity = 2f; //the up to which the jump can be cancel
    public float initialjumpYVelocity = 2f;
    public float finaljumpYVelocity = 10f;
    public float jumpYVelocitytIncrement = 2f;
    public float jumpMovementSpeed = 2f;
    public float jumpMaxTime = 1.0f;
    public float hoverTime = 1.0f;
    float jumpTimer = 0f;
    float hoverTimer = 0f;
    bool jumpCancelled = false;
    bool gravityCancel = false;
    bool hover = false;

    [Space]
    [Header("Infection")]
    public float movementSpeed1Infected = 12.0f;
    public float movementSpeed2Infected = 6.0f;
    public MechPiece leftLeg;
    public MechPiece rightLeg;

    //State Machine
    private CharacterState currentState = CharacterState.isIdle;

    //Controller variables
    bool playerIndexSet = false;
    PlayerIndex playerIndex;
    GamePadState state;
    GamePadState prevState;

    [SerializeField]
    AudioSource walkAudioSource;
    [SerializeField]
    AudioSource dashAudioSource;



    public enum CharacterState
    {
        isIdle,
        isMoving,
        isJumping,
        isDashing,
        isDashCooling
    }
    


    void Awake()
    {
        myController = GetComponent<CharacterController>();
    }

    void Update()
    {

        SetController();
        Gravity();
        RunStates();
    }

    void SetController()
    {
        if (!playerIndexSet || !prevState.IsConnected)
        {
            for (int i = 0; i < 4; ++i)
            {
                PlayerIndex testPlayerIndex = (PlayerIndex)i;
                GamePadState testState = GamePad.GetState(testPlayerIndex);
                if (testState.IsConnected)
                {
                    playerIndex = testPlayerIndex;
                    playerIndexSet = true;
                }
            }
        }


        prevState = state;
        state = GamePad.GetState(playerIndex);
    }

    void LateUpdate()
    {

        
        //Set direction
        moveDirection = transform.TransformDirection(new Vector3(myXVelocity, myYVelocity, myZVelocity));



        //apply movement
        myController.Move(moveDirection * Time.deltaTime);
    }

    //---STATE MACHINE FUNCTIONS
    void RunStates()
    {
        //Start new state
       /* if (Input.GetKeyDown(jumpButton)  && (currentState == CharacterState.isIdle || currentState == CharacterState.isMoving || currentState == CharacterState.isJumping))
        {
            if (currentState != CharacterState.isJumping)
            {
                StartJump();
            }

        } */

        

        if (currentState == CharacterState.isIdle && (state.ThumbSticks.Left.X != 0 || state.ThumbSticks.Left.X != 0))
        {
            StartMove();
        } else if (state.Buttons.RightShoulder == ButtonState.Pressed && Time.time > timeTillNextDash && !rightLeg.isInfected && !rightLeg.isDead && !leftLeg.isInfected && !leftLeg.isDead && (currentState == CharacterState.isIdle || currentState == CharacterState.isMoving))
        {
            StartDash();
        }
        

        //Jump();
        Move();
        Dash();

    }

    void ResetStates()
    {

    }

    //Jump
    void StartJump()
    {
        ResetStates();
        currentState = CharacterState.isJumping;
        jumpYVelocity = initialjumpYVelocity;
        myYVelocity = jumpYVelocity;
        jumpTimer = Time.time;
        jumpCancelled = false;

        //myXVelocity = 0f;
    }

    void Jump()
    {
        if (currentState == CharacterState.isJumping)
        {
            jumpYVelocity += jumpYVelocitytIncrement * Time.deltaTime;

            if (hover)
            {
                if(myYVelocity < 0)
                {
                    myYVelocity = 0f;
                    gravityCancel = true;
                }

                if(Time.time > hoverTimer + hoverTime)
                {
                    hover = false;
                    gravityCancel = false;
                }
            }            
            //jump cancel
            else if ((state.Buttons.LeftShoulder == ButtonState.Released || (Time.time > jumpTimer+jumpMaxTime && !jumpCancelled) ) && myYVelocity > jumpCancelYVelocity)
            {
                jumpCancelled = true;
                //hover = true;
                hoverTimer = Time.time;
                myYVelocity = jumpCancelYVelocity;
                
            } else if (!jumpCancelled)
            {
                myYVelocity = jumpYVelocity;
            }

            //air control
            //Change X velocity
            myXVelocity = state.ThumbSticks.Left.X * jumpMovementSpeed;

            //Change Z velocity
            myZVelocity = state.ThumbSticks.Left.Y * jumpMovementSpeed;




            //Exit condition
            if (myYVelocity < 0 && myController.isGrounded)
            {
                StopJump();
            }
        }
    }

    void StopJump()
    {
        currentState = CharacterState.isIdle;
        myYVelocity = 0;
    }

    void StartDash()
    {
        currentState = CharacterState.isDashing;
        //Change x velocity
        myXVelocity = state.ThumbSticks.Left.X * dashSpeed;
        //Change Z velocity
        myZVelocity = state.ThumbSticks.Left.Y * dashSpeed;

        if(myXVelocity == 0 && myZVelocity == 0)
        {
            myZVelocity = dashSpeed;
        }

        dashTimer = Time.time;
        timeTillNextDash = Time.time + dashCooldown;

        dashAudioSource.PlayOneShot(dashClip);
    }

    void Dash()
    {
        if(currentState == CharacterState.isDashing || currentState == CharacterState.isDashCooling)
        {

            if(currentState == CharacterState.isDashing)
            {
                if(Time.time > dashTimer + dashTime)
                {
                    currentState = CharacterState.isDashCooling;
                }
            }
            else if(currentState == CharacterState.isDashCooling)
            {

                if (myZVelocity > movementSpeed)
                {
                    myZVelocity += dashDeceleration * Time.deltaTime;
                    if (myZVelocity < movementSpeed)
                    {
                        myZVelocity = movementSpeed;
                    }
                }

                if (myXVelocity > movementSpeed)
                {
                    myXVelocity += dashDeceleration * Time.deltaTime;
                    if (myXVelocity < movementSpeed)
                    {
                        myXVelocity = movementSpeed;
                    }
                }

                if (myXVelocity <= movementSpeed && myZVelocity <= movementSpeed)
                {
                    StopDash();
                }
            }

            
        }
    }

    void StopDash()
    {
        currentState = CharacterState.isMoving;
    }

    //Walk
    void StartMove()
    {
        ResetStates();
        currentState = CharacterState.isMoving;
    }

    void Move()
    {
        if ((leftLeg.isInfected || leftLeg.isDead) && (rightLeg.isInfected || rightLeg.isDead))
        {
            currentMovementSpeed = movementSpeed2Infected;
        } else if ((leftLeg.isInfected || leftLeg.isDead) || (rightLeg.isInfected || rightLeg.isDead))
        {
            currentMovementSpeed = movementSpeed1Infected;
        } else
        {
            currentMovementSpeed = movementSpeed;
        }

            if (currentState == CharacterState.isMoving)
        {
            if (!walkAudioSource.isPlaying && Time.time > timeTillNextStep)
            {
                timeTillNextStep = Time.time + timeBetweenStepSounds;
                walkAudioSource.PlayOneShot(walkClip);
            }
            //Walk Animation

            //Change X velocity
            myXVelocity = state.ThumbSticks.Left.X * currentMovementSpeed;

            //Change Z velocity
            myZVelocity = state.ThumbSticks.Left.Y * currentMovementSpeed;




            //Exit condition
            if (state.ThumbSticks.Left.X == 0 && state.ThumbSticks.Left.Y == 0)
            {
                StopMove();
            }
        }
    }

    void StopMove()
    {
        currentState = CharacterState.isIdle;
    }



    void Gravity()
    {
        if (!gravityCancel)
        {
            if (!myController.isGrounded)
            {

                if (myYVelocity > terminalYVelocity)
                {
                    myYVelocity += gravity * Time.deltaTime;
                }
                else
                {
                    myYVelocity = terminalYVelocity;
                }

            }
            else
            {
                myYVelocity = -1;
            }
        }
       
    }
    


   
}

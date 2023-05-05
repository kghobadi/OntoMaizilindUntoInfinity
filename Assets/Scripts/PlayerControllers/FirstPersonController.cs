using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using InControl;
using NPC;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class FirstPersonController : MonoBehaviour
{
    //timers and values for speed
    public float currentSpeed, walkSpeed, sprintSpeed;
    public float scrollSpeed = 2.0f;
    float sprintTimer = 0;
    public float sprintTimerMax = 1;

    float footStepTimer = 0;
    public float footStepTimerTotal = 0.5f;

    public float footstepVol = 1f;

    CharacterController player;
    GroundCamera mouseLook;
    Vector3 movement;
    ResetNearbyAudioSources resetAudio;

    //for footstep sounds
    public AudioClip[] currentFootsteps/*, indoorFootsteps, gardenFootsteps, pathFootsteps*/;
    AudioSource playerAudSource;
    
    public bool canMove = true;
    public bool moving;
    public bool mirrorControls;
    public bool disableOnStart;

    Vector3 lastPosition;

    //for start of radio room
    public GameObject startCam;

    [Header("Holding Objects")] 
    public Transform holdingSpot;
    public bool holding;
    public PickUpObject pickUp;
    public float holdingRadius = 2f;
    private float normalRadius = 0.5f;
    public UnityEvent beingHeld;
    public Animator personAnimator;
    public Animations npcAnimator;
    
    void Awake()
    {
        player = GetComponent<CharacterController>();
        playerAudSource = GetComponent<AudioSource>();
        mouseLook = GetComponentInChildren<GroundCamera>();
        resetAudio = GetComponent<ResetNearbyAudioSources>();
        normalRadius = player.radius;
    }

    private void Start()
    {
        if (disableOnStart)
        {
            DisableMovement();
        }
    }

    private void OnEnable()
    {
        if (mouseLook)
        {
            beingHeld.AddListener(mouseLook.ToggleClamp);
        }
    }

    private void OnDisable()
    {
        if (mouseLook)
        {
            beingHeld.RemoveListener(mouseLook.ToggleClamp);
        }
    }
    
    void FixedUpdate()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        if (canMove)
        {
            //controller 
            if (inputDevice.DeviceClass == InputDeviceClass.Controller)
            {
                ControllerMovement();
            }
            //mouse & keyboard 
            else
            {
                MouseKeyboardMovement();
            }

            //actual movement
            if (moving)
            {
                if (startCam)
                    DeactivateStartCam();
                
                movement = transform.rotation * movement;
                player.Move(movement * Time.deltaTime);
                
                //if(resetAudio)
                    //resetAudio.ResetNearbyAudio();

                if (npcAnimator)
                {
                    npcAnimator.SetAnimator("moving");
                }
            }
            else
            {
                if (npcAnimator)
                {
                    npcAnimator.SetAnimator("idle");
                }
            }

            //fall down over time 
            player.Move(new Vector3(0, -0.5f, 0));
        }
    }

    void ControllerMovement()
    {  
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //moving 
        if (inputDevice.LeftStickY != 0 && inputDevice.LeftStickX != 0)
        {
            moving = true;
            
            float moveForwardBackward;
            float moveLeftRight;
            
            if (mirrorControls)
            {
                moveForwardBackward = -inputDevice.LeftStickY * currentSpeed; 
                moveLeftRight = -inputDevice.LeftStickX * currentSpeed;
            }
            else
            { 
                moveForwardBackward = inputDevice.LeftStickY * currentSpeed; 
                moveLeftRight = inputDevice.LeftStickX * currentSpeed;
            }
            
            movement = new Vector3(moveLeftRight, 0, moveForwardBackward);

            SprintSpeed();

        }
        //when not moving
        else
        {
            moving = false;
            movement = Vector3.zero;
            currentSpeed = walkSpeed;
        }
    }

    void MouseKeyboardMovement()
    {
        //when hold mouse 1, you begin to move in that direction
        /*if (Input.GetMouseButton(0))
        {
            moving = true;

            movement = new Vector3(0, 0, currentSpeed);

            SprintSpeed();
        }
        //move backwards
        else if (Input.GetMouseButton(1))
        {
            moving = true;

            movement = new Vector3(0, 0, -currentSpeed);

            SprintSpeed();
        }*/
        //WASD controls
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            moving = true;

            float moveForwardBackward;
            float moveLeftRight;
            
            if (mirrorControls)
            {
                moveForwardBackward = -Input.GetAxis("Vertical") * currentSpeed;
                moveLeftRight = -Input.GetAxis("Horizontal") * currentSpeed;
            }
            else
            { 
                moveForwardBackward =Input.GetAxis("Vertical") * currentSpeed;
                moveLeftRight = Input.GetAxis("Horizontal") * currentSpeed;
            }

            movement = new Vector3(moveLeftRight, 0, moveForwardBackward);

            SprintSpeed();

        }
        //when not moving
        else
        {
            moving = false;
            movement = Vector3.zero;
            currentSpeed = walkSpeed;
        }
    }

    void DeactivateStartCam()
    {
        if(startCam.activeSelf)
            startCam.SetActive(false);
    }
    
    //increases move speed while player is moving over time
    public void SprintSpeed()
    {
        //increment and play footstep sounds
        footStepTimer -= Time.deltaTime;
        if (footStepTimer < 0)
        {
            PlayFootStepAudio();
            footStepTimer = footStepTimerTotal;
        }

        sprintTimer += Time.fixedDeltaTime;
        //while speed is less than sprint, autoAdd
        if (sprintTimer > sprintTimerMax && currentSpeed < sprintSpeed)
        {
            currentSpeed += Time.fixedDeltaTime;
        }
    }

    private void PlayFootStepAudio()
    {
        if (currentFootsteps.Length <= 0)
        {
            return;
        }
        
        int n = Random.Range(1, currentFootsteps.Length);
        playerAudSource.clip = currentFootsteps[n];
        playerAudSource.PlayOneShot(playerAudSource.clip, footstepVol);
        // move picked sound to index 0 so it's not picked next time
        currentFootsteps[n] = currentFootsteps[0];
        currentFootsteps[0] = playerAudSource.clip;
    }

    public void SetHolding(PickUpObject pickUpObject)
    {
        pickUp = pickUpObject;
        player.radius = holdingRadius;
        player.stepOffset = 0.25f;
        holding = true;
    }

    public void DropObject()
    {
        pickUp = null;
        player.radius = normalRadius;
        player.stepOffset = 0.75f;
        holding = false;
    }

    public void ResetStepOffset()
    {
        player.stepOffset = 0.75f;
    }

    public void DisableMovement()
    {
        canMove = false;
        player.enabled = false;
    }

    public void EnableMovement()
    {
        canMove = true;
        player.enabled = true;
    }
}

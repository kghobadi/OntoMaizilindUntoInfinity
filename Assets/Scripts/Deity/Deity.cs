﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// General manager of the Deity behaviors. 
/// </summary>
public class Deity : MonoBehaviour {

    [HideInInspector] public MoveTowards mover;
    ThePilot pilot;
    DeityHealth _Health;
    DeityAnimations _Animations;
    Rigidbody deityBody;

    //Properties
    public DeityHealth DeityHealth => _Health;
    public DeityAnimations DeityAnimations => _Animations;

    [SerializeField] private string deityName;
    public string DeityName => deityName;
    [SerializeField] private TMP_Text deityTitleText;
    private CanvasGroup deityTitleGroup;
    [Header("Movements")]
    public bool strafe;
    public bool moveForward;
    public float strafeSpeed;
    public float moveSpeed;
    public float xDestination;
    public float maxVelocityZ = 500f;
    public float maxVelocityX = 500f;

    [Tooltip("Ship position in Deity formation")]
    public FlightPos flightPos;
    public enum FlightPos
    {
        CENTER, LEFT, RIGHT,
    }
    float origXPos;
    private Vector3 enabledPos;
    int strafeCount = 0;
    public bool strafingDirection;
    public float xMin, xMax;

    [Header("Deity Weapons")] 
    public ParticleSystem destructionBeam;
    public ParticleSystem deathTendrils;
    
    [Header("Player Interaction")] 
    public bool engagingPlayer;
    public float engageDistance;
    public float distanceFromPlayer;
    public float xDistFromPlayer;
    public float yDistFromPlayer;
    public float zDistFromPlayer;

    void Awake()
    {
        pilot = FindObjectOfType<ThePilot>();
        _Health = GetComponentInChildren<DeityHealth>();
        _Animations = GetComponent<DeityAnimations>();
        deityBody = GetComponent<Rigidbody>();
        mover = GetComponent<MoveTowards>();
        if (deityTitleText)
        {
            deityTitleGroup = deityTitleText.GetComponent<CanvasGroup>();
        }
    }
    
    void Start()
    {
        //grab orig x pos 
        origXPos = transform.position.x;
        if(deityTitleText)
            deityTitleText.text = deityName;

        //reset count & start strafe
        strafeCount = 0;
        strafe = true;

        //set dirs & start moving 
        SetDirections(flightPos);
    }

    private void OnEnable()
    {
        enabledPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (moveForward)
            FlyForward();

        if (strafe)
            Strafe();
    }

    /// <summary>
    /// Accessible method for animation based cloud gen timing.
    /// </summary>
    public void SpitClouds()
    {
        for (int i = 0; i < _Health.DeityCloudGens.Length; i++)
        {
            _Health.DeityCloudGens[i].Generate();
        }
    }

    /// <summary>
    /// Call a specific cloud gen to spawn. 
    /// </summary>
    /// <param name="index"></param>
    public void SpitCloudsFromIndex(int index)
    {
        _Health.DeityCloudGens[index].Generate();
    }

    private Vector3 lastVelocity;
    public void FreezeMovement()
    {
        moveForward = false;
        strafe = false;

        //zero vel
        if (deityBody != null && gameObject.activeSelf)
        {
            lastVelocity = deityBody.velocity; //TODO find out why this is throwing error 
            deityBody.velocity = Vector3.zero;
        }
    }

    public void ResumeMovement()
    {
        moveForward = true;
        strafe = true;

        //return to last vel
        if(deityBody != null && gameObject.activeSelf)
        {
            deityBody.velocity = lastVelocity;
        }
    }

    public void SetForwardSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetMaxZSpeed(float max)
    {
        maxVelocityZ = max;
    }

    public void SetFall()
    {
        moveForward = false;
        strafe = false;
        destructionBeam.Stop();
        //enable death particles 
        deathTendrils.Play();
        
        //fade out deity title
        if(deityTitleGroup)
            LeanTween.alphaCanvas(deityTitleGroup, 0f, 3f);
    }

    public void FadeInDeityTitle()
    {
        if(deityTitleGroup)
            LeanTween.alphaCanvas(deityTitleGroup, 1f, 3f);
    }

    public void SetCrash()
    {
        //disable death particles 
        deathTendrils.Stop();
        //disable movement
        FreezeMovement();
    }
    
    void FlyForward()
    {
        if(Mathf.Abs(deityBody.velocity.z)  < maxVelocityZ)
            deityBody.AddForce(0, 0, moveSpeed);
    }

    void Strafe()
    {
        if (Mathf.Abs(deityBody.velocity.x) < maxVelocityX)
            deityBody.AddForce(strafeSpeed, 0, 0);
    }

    void LateUpdate()
    {
        if (strafe)
        {
            //right
            if (strafingDirection)
            {
                //past dest - switch
                if (transform.position.x > xDestination)
                    StrafeOpposite();
            }
            //left
            else
            {
                //past dest - switch
                if (transform.position.x < xDestination)
                    StrafeOpposite();
            }
        }
        
        //DistanceCalcs();
    }

    //TODO disabling for now, will decide what to do with this now that we have RailLandscape. 
    void DistanceCalcs()
    {
        //calc distance 
        distanceFromPlayer = Vector3.Distance(transform.position, pilot.transform.position);
        //x dist
        xDistFromPlayer = Mathf.Abs(transform.position.x - pilot.transform.position.x);
        //y dist
        yDistFromPlayer = Mathf.Abs(transform.position.y - pilot.transform.position.y);
        //z dist
        zDistFromPlayer = Mathf.Abs(transform.position.z - pilot.transform.position.z);

        if (zDistFromPlayer < engageDistance)
        {
            EngagePlayer();   
        }
    }

    public void EngagePlayer()
    {
        if(engagingPlayer)
            return;
        
        engagingPlayer = true;
        
        //TODO here we can do unique deity behavior commands?
    }

    public void DisengagePlayer()
    {
        if(!engagingPlayer)
            return;

        engagingPlayer = false;
    }

    void SetDirections(FlightPos fPos)
    {
        switch (fPos)
        {
            case FlightPos.CENTER:
                //lets go right first
                strafingDirection = true;
                break;
            case FlightPos.LEFT:
                //lets go left first
                strafingDirection = false;
                strafeSpeed = -strafeSpeed;
                break;
            case FlightPos.RIGHT:
                //lets go right first
                strafingDirection = true;
                break;
        }
    }

    public void StrafeOpposite()
    {
        //switch dir
        if(strafeCount > 0)
        {
            strafingDirection = !strafingDirection;
            strafeSpeed = -strafeSpeed;
            //zero x velocity
            deityBody.velocity = new Vector3(0, deityBody.velocity.y, deityBody.velocity.z);
        }
            
        //go right
        if (strafingDirection)
        {
            xDestination = xMax;
        }
        //go left 
        else
        {
            xDestination = xMin;
        }
        
        //inc 
        strafeCount++;
    }
}

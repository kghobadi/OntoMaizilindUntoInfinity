using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deity : MonoBehaviour {

    [HideInInspector] public MoveTowards mover;
    ThePilot pilot;
    DeityHealth _Health;
    DeityAnimations _Animations;
    Rigidbody deityBody;

    [HideInInspector]
    public float deitySpeed = 25f;
    public Transform pilotFightPos;

    void Awake()
    {
        pilot = FindObjectOfType<ThePilot>();
        _Health = GetComponentInChildren<DeityHealth>();
        _Animations = GetComponent<DeityAnimations>();
        deityBody = GetComponent<Rigidbody>();
    }

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
    int strafeCount = 0;
    public bool strafingDirection;
    public float xMin, xMax;

    void Start()
    {
        //grab orig x pos 
        origXPos = transform.position.x;

        //reset count & start strafe
        strafeCount = 0;
        strafe = true;

        //set dirs & start moving 
        SetDirections(flightPos);
    }

    private void FixedUpdate()
    {
        if (moveForward)
            FlyForward();

        if (strafe)
            Strafe();
    }

    public void SetForwardSpeed(float speed)
    {
        moveSpeed = speed;
    }

    public void SetMaxZSpeed(float max)
    {
        maxVelocityZ = max;
    }

    void FlyForward()
    {
        if(deityBody.velocity.magnitude < maxVelocityZ)
            deityBody.AddForce(0, 0, moveSpeed);
    }

    void Strafe()
    {
        if (deityBody.velocity.magnitude < maxVelocityX)
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

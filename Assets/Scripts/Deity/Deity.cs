using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deity : MonoBehaviour {

    [HideInInspector] public MoveTowards mover;
    ThePilot pilot;
    DeityHealth _Health;
    DeityAnimations _Animations;

    public float deitySpeed = 25f;
    public Transform pilotFightPos;

	void Awake ()
    {
        mover = GetComponent<MoveTowards>();
        pilot = FindObjectOfType<ThePilot>();
        _Health = GetComponentInChildren<DeityHealth>();
        _Animations = GetComponent<DeityAnimations>();
    }

    [Header("Strafing Movements")]
    public float moveSpeed;
    public bool strafe;
    public bool moving;
    public float xDestination;

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
        SetDirections();
        SetMove();
    }

    void LateUpdate()
    {
        if (strafe)
        {
            if (moving)
            {
                //actual move
                transform.Translate(moveSpeed * Time.deltaTime, 0, 0);

                //Debug.Log(gameObject.name + "moving!");

                //right
                if (strafingDirection)
                {
                    //past dest - switch
                    if(transform.position.x > xDestination)
                        moving = false;
                }
                //left
                else
                {
                    //past dest - switch
                    if (transform.position.x < xDestination)
                        moving = false;
                }
            }
            else
            {
                SetMove();
            }
        }
    }

    void SetDirections()
    {
        switch (flightPos)
        {
            case FlightPos.CENTER:
                //lets go right first
                strafingDirection = true;
                break;
            case FlightPos.LEFT:
                //lets go left first
                strafingDirection = false;
                moveSpeed = -moveSpeed;
                break;
            case FlightPos.RIGHT:
                //lets go right first
                strafingDirection = true;
                break;
        }
    }

    public void SetMove()
    {
        //switch dir
        if(strafeCount > 0)
        {
            strafingDirection = !strafingDirection;
            moveSpeed = -moveSpeed;
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
        //move!
        moving = true;
    }
}

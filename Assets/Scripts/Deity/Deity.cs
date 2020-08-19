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
    [HideInInspector] public float origSpeed;

    public bool strafe;
    public bool moving;
    public Vector3 destination;
    public float necessaryDist = 1f;

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
        origSpeed = moveSpeed;
        origXPos = transform.position.x;
        strafe = true;
    }

    void Update()
    {
        if (strafe)
        {
            if (moving)
            {
                SetDestination();

                transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);

                if (Vector3.Distance(transform.position, destination) < necessaryDist)
                {
                    moving = false;
                    moveSpeed = origSpeed;
                }
            }
            else
            {
                SetMove();
            }
        }
    }

    public void SetMove()
    {
        //switch dir
        if(strafeCount > 0)
            strafingDirection = !strafingDirection;

        //set strafe pattern at start 
        if (strafeCount == 0)
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
                    break;
                case FlightPos.RIGHT:
                    //lets go right first
                    strafingDirection = true;
                    break;
            }
        }

        //call move
        Move();

        //inc 
        strafeCount++;
    }

    //called each frame while moving to reevaluate move pos 
    void SetDestination()
    {
        //go left
        if (strafingDirection)
        {
            destination = new Vector3(xMin, transform.position.y, transform.position.z);
        }
        //go right 
        else
        {
            destination = new Vector3(xMax, transform.position.y, transform.position.z);
        }
    }

    void Move()
    {
        moving = true;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//controls the spirit writing after your parents die
public class SpiritWriting : MonoBehaviour {

    Vector3 origPos;
    TrailRenderer trail;
    MoveTowards mover;

    //the projection corner assigned when I enter the mosque 
    public Transform[] writingPoints;
    private int currentPoint;

    //bool states
    public bool activated;

    void Awake ()
    {
        trail = GetComponent<TrailRenderer>();
        mover = GetComponent<MoveTowards>();
        trail.enabled = false;
    }

    private void Start()
    {
        EnableSpirit();
    }

    public void EnableSpirit()
    {
        //set pos
        currentPoint = 0;
        transform.position = writingPoints[currentPoint].position;
        origPos = transform.position;
        //enable trail
        trail.enabled = true;
        trail.Clear();
        activated = true;
    }
	
    void Update ()
    {
        if (activated)
        {
            //whenever 
            if(mover.moving == false )
            {
                SetNextPoint(); 
            }
        }
    }

    void SetNextPoint()
    {
        if (currentPoint < writingPoints.Length - 1)
        {
            //move to next point
            currentPoint++;
            mover.MoveTo(writingPoints[currentPoint].position, 15f);
        }
        else
        {
            //ResetTrail();
            activated = false;
        }
    }

    //starts reset
    void ResetTrail()
    {
        trail.emitting = false;
        transform.position = origPos;
        trail.emitting = true;

        currentPoint = 0;
    }
}
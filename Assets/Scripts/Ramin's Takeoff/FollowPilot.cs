﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPilot : MonoBehaviour {
    GameObject pilot;
    MoveTowards mover;

    [Tooltip("Place an object in here to make this follow that object instead of the pilot.")]
    public GameObject optionalFollowObject;
    public FollowType followType;
    public enum FollowType
    {
        INSTANT, MOVETOWARDS, 
    }
   
    [Header("Axes to Follow")]
    public bool followX;
    public bool followY;
    public bool followZ;

    [Header("Start Dists from Pilot")]
    public float xDistAtStart;
    public float yDistAtStart;
    public float zDistAtStart;

    void Awake()
    {
        if (optionalFollowObject)
        {
            pilot = optionalFollowObject;
        }
        else
        {
            pilot = FindObjectOfType<ThePilot>().gameObject;
        }

        if (followType == FollowType.MOVETOWARDS)
        {
            mover = GetComponent<MoveTowards>();
            if (mover == null)
                mover = gameObject.AddComponent<MoveTowards>();
        }
    }

    void Start ()
    {
        xDistAtStart =  transform.position.x - pilot.transform.position.x;
        yDistAtStart = transform.position.y - pilot.transform.position.y;
        zDistAtStart = transform.position.z - pilot.transform.position.z;
    }
    
    private void FixedUpdate()
    {
        FollowThePilot();
    }

    void FollowThePilot()
    {
        Vector3 targetPos = transform.position;
        
        //X
        if (followX)
        {
            targetPos.x = pilot.transform.position.x + xDistAtStart;
        }
        //Y
        if (followY)
        {
            targetPos.y = pilot.transform.position.y + yDistAtStart;
        }
        //Z
        if (followZ)
        {
            targetPos.z = pilot.transform.position.z + zDistAtStart;
        }

        //only set pos if there is a follow 
        if(followX || followY || followZ)
            SetPos(targetPos);
    }

    //sets position to fed v3 based on follow type
    void SetPos(Vector3 target)
    {
        if (followType == FollowType.INSTANT)
            transform.position = target;
        else if (followType == FollowType.MOVETOWARDS)
        {
            if (mover.moving == false)
            {
                mover.MoveTo(target, mover.moveSpeed);
            }
        }
    }

    //allows you to overwrite dist
    public void ResetZDist(float newDist)
    {
        zDistAtStart = newDist;
    }

    //Allow you to set follow bools for each axes
    public void SetXFollow(bool follow)
    {
        followX = follow;
    }
    public void SetYFollow(bool follow)
    {
        followY = follow;
    }
    public void SetZFollow(bool follow)
    {
        followZ = follow;
    }
}

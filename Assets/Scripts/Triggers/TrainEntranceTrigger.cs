using System;
using System.Collections;
using System.Collections.Generic;
using NPC;
using UnityEngine;

public class TrainEntranceTrigger : TriggerBase
{
    [Header("Train Entrance Settings")]
    public Transform[] seats;
    private bool[] seatsTaken;
    public bool hasSeats = true;

    public MovementPath newMovement;
    private Movement npcMover;
    public Transform lookAtObject;

    private void Awake()
    {
        GetSeats();
    }

    void GetSeats()
    {
        seatsTaken = new bool[seats.Length];
    }

    protected override void OnTriggerEnter(Collider other)
    {
        npcMover = other.GetComponent<Movement>();
        
        base.OnTriggerEnter(other);
    }

    public override void SetTrigger()
    {
        base.SetTrigger();
    }

    public override void ActivateTriggerEffect()
    {
        Transform seat = AssignSeat();

        //reset movement
        if (npcMover)
        {
            //idle
            npcMover.SetIdle();

            //teleport to seat
            npcMover.transform.position = seat.position;
            
            //set look at obj
            npcMover.SetLook(lookAtObject);
            
            //set sitting idle 
            npcMover.ResetMovement(newMovement);
        }

        //only reactivate if there are seats
        if (hasSeats)
        {
            base.ActivateTriggerEffect();
        }
    }

    Transform AssignSeat()
    {
        Transform retVal = null;
        for (int i = 0; i < seats.Length; i++)
        {
            if (seatsTaken[i] == false)
            {
                //take seat
                seatsTaken[i] = true;
                retVal = seats[i];
                break;
            }
        }

        //when there is no seat, change has seats to false  
        if (retVal == null)
        {
            hasSeats = false;
        }
        
        return retVal;
    }
}

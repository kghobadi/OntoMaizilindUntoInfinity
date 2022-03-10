using System;
using System.Collections;
using System.Collections.Generic;
using NPC;
using UnityEngine;

public class TrainEntranceTrigger : TriggerBase
{
    [Header("Train Entrance Settings")]
    public Transform[] seats;
    public bool hasSeats = true;

    public MovementPath newMovement;
    private Movement npcMover;
    public Transform lookAtObject;


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

        if (seat == null)
        {
            Debug.Log("Train car has no more seats!");
            return;
        }

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
            
            //make npc child of seat 
            npcMover.transform.SetParent(seat);
            npcMover.transform.localPosition = Vector3.zero;
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
            if (seats[i].childCount == 0)
            {
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

using System;
using System.Collections;
using System.Collections.Generic;
using NPC;
using UnityEngine;

public class TrainEntranceTrigger : TriggerBase
{
    private SoulExplosion soulExplosion;
    
    [Header("Train Entrance Settings")]
    public Transform[] seats;
    public bool[] seatsTaken;
    public bool hasSeats = true;

    public MovementPath newMovement;
    private Movement npcMover;
    public Transform lookAtObject;
    public Transform standingPos;

    private void Start()
    {
        soulExplosion = FindObjectOfType<SoulExplosion>();
        
        seatsTaken = new bool[seats.Length];
    }

    protected override void OnTriggerEnter(Collider other)
    {
        npcMover = other.GetComponent<Movement>();

        if (other.gameObject.CompareTag("Player"))
        {
            soulExplosion.SetPlayerEntered();
        }
        
        base.OnTriggerEnter(other);
    }

    public override void SetTrigger()
    {
        base.SetTrigger();
    }

    public override void ActivateTriggerEffect()
    {
        //reset movement
        if (npcMover)
        {
            //check that soul explosion does NOT contain the npc 
            if (soulExplosion.npcMovers.Contains(npcMover) == false)
            {
                //idle 
                npcMover.SetIdle();
            
                //npc has a seat, not a standing pos.
                // if (seat != standingPos)
                // {
                //     //set sitting idle 
                //     npcMover.ResetMovement(newMovement);
                // }
                // //npc will be standing -- so just set idle. 
                // else
                // {
                //     npcMover.SetIdle();
                // }

                //set look at obj
                if (lookAtObject)
                {
                    npcMover.SetLook(lookAtObject);
                }
                
                //get seat 
                Transform seat = AssignSeat();

                //navigate to seat pos 
                npcMover.NavigateToPoint(seat.position, false);
                
                //add to soul explosion list. 
                soulExplosion.npcMovers.Add(npcMover);
            }
        }

        //check soul explosion
        soulExplosion.CheckAllTrainsFull();
        
        //reactivate 
        base.ActivateTriggerEffect();
    }

    Transform AssignSeat()
    {
        Transform retVal = null;
        for (int i = 0; i < seats.Length; i++)
        {
            if (!seatsTaken[i])
            {
                //assign transform return value
                retVal = seats[i];
                //set seat taken to true. 
                seatsTaken[i] = true;
                //break for loop.
                break;
            }
        }

        //when there is no seat, change has seats to false  
        if (retVal == null)
        {
            hasSeats = false;

            //use standing pos 
            retVal = standingPos;
        }
        
        return retVal;
    }
}

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace NPC
{
    public class Movement : MonoBehaviour
    {
        //controller pieces 
        GameObject currentPlayer;
        Controller controller;
        Animations npcAnimations;
        Sounds npcSounds;
        Camera mainCam;
        NPCMovementManager movementManager;
        MonologueManager monoManager;

        [Header("AI Movement Settings")]
        public LayerMask grounded;
        [HideInInspector]
        public NavMeshAgent myNavMesh;
        Vector3 origPosition;
        public MovementPath startBehavior;
        public Vector3 targetPosition;
        float distFromPlayer;
        public bool AIenabled = true;

        //state timers 
        public float stateTimer, actionTimer;
        public float idleTime, actionTime;
        public float interactDistance;
        public float lookSmooth = 5f;

        [Header("Monologue Wait")]
        public bool waitingToGiveMonologue;
        public float monologueWaitTimer = 0f, monoWaitTime = 30f;
        
        //chosen in editor 
        public NPCMovementTypes npcType;
        public enum NPCMovementTypes
        {
            WAYPOINT, RANDOM, IDLE, PATHFINDER,
        }

        [Header("Wanderer Settings")]
        public Transform[] waypoints;
        public int waypointCounter = 0;

        [Header("Random Settings")]
        public float movementRadius;
        
        void Awake()
        {
            GetRefs();
        }

        void GetRefs()
        {
            controller = GetComponent<Controller>();
            npcAnimations = GetComponentInChildren<Animations>();
            npcSounds = GetComponent<Sounds>();
            mainCam = Camera.main;
            myNavMesh = GetComponent<NavMeshAgent>();
            movementManager = FindObjectOfType<NPCMovementManager>();
            monoManager = GetComponent<MonologueManager>();
        }

        void Start()
        { 
            origPosition = transform.position;
            myNavMesh.speed += Random.Range(-5f, 10f);
            ResetMovement(startBehavior);
            SetIdle();
        }

        void Update()
        {
            //player ref
            currentPlayer = controller.camSwitcher.cameraObjects[controller.camSwitcher.currentCam].gameObject;
            if (AIenabled)
            {
                //dist from player 
                DistanceCheck();
                //idle state
                Idle();
                //moving state
                Moving();
                //talking state
                Talking();
            }
        }
        
        //checks distance from player && runs corresponding behaviors
        void DistanceCheck()
        {
            //distance from player calc
            distFromPlayer = Vector3.Distance(transform.position, currentPlayer.transform.position);

            //check dist against talking
            if (distFromPlayer < interactDistance)
            {

            }
            //player is far away 
            else
            {

            }
        }

        //NPC idle state 
        void Idle()
        {
            if (controller.npcState == Controller.NPCStates.IDLE)
            {
                stateTimer -= Time.deltaTime;

                //NPC will perform its action 
                ActionCountdown();

                //if we are not IDLE npc, idle state has a countdown until movement 
                if (npcType == NPCMovementTypes.IDLE)
                {
                    //play a sound
                    if (stateTimer < 0  && !controller.Monologues.inMonologue)
                    {
                        //idle action?

                        stateTimer = idleTime;
                    }
                }
                //Set destination based on npc type 
                else if (npcType == NPCMovementTypes.RANDOM)
                {
                    if (stateTimer < 0 )
                    {
                        //Debug.Log("calling radials");
                        SetRandomDestination();
                    }

                }
                //looping waypoints npc 
                else if (npcType == NPCMovementTypes.WAYPOINT)
                {
                    //wait for monologue 
                    if (!waitingToGiveMonologue)
                    {
                        if (stateTimer < 0 )
                        {
                            //Debug.Log("calling waypoints");
                            SetWaypoint(true);
                        }
                    }
                    //waiting to give monologue -- look at player 
                    else
                    {
                        WaitToGiveMonologue();
                    }
                }
                //waits until player is near then walks to next point 
                else if (npcType == NPCMovementTypes.PATHFINDER)
                {
                
                    //goes to next point if timer reaches 0 or player is near 
                    //Only does this if there are currenltly points in my list 
                    if (!waitingToGiveMonologue)
                    {
                        //make sure there is more waypoints!
                        if (waypointCounter < waypoints.Length - 1)
                        {
                            float playerDist = 0;
                            float myDist = 0;
                            if (waypointCounter + 1 < waypoints.Length - 1 && waypoints.Length > 1)
                            {
                                //player dist from next waypoint
                                playerDist = Vector3.Distance(currentPlayer.transform.position, waypoints[waypointCounter + 1].position);
                                myDist = Vector3.Distance(transform.position, waypoints[waypointCounter + 1].position);
                            }
                            
                            //if I am close to player, stateTimer ended, or player is closer to my next dest
                            if (distFromPlayer < interactDistance || stateTimer < 0 || playerDist < myDist)
                            {
                                SetWaypoint(false);
                            }
                        }
                    }
                    //waiting to give monologue -- look at player 
                    else
                    {
                        WaitToGiveMonologue();
                    }
                }
            }
        }

        //called from within Idle state only 
        void WaitToGiveMonologue()
        {
            LookAtObject(currentPlayer.transform.position, true);

            //dont want this to count until start view is inactive 
            monologueWaitTimer += Time.deltaTime;
            
            //stop waiting for monologue IF not in monologue 
            if (monologueWaitTimer > monoWaitTime && !monoManager.inMonologue)
            {
                waitingToGiveMonologue = false;

                monologueWaitTimer = 0;

                //deactivate monologue trigger if it does not repeat 
                if (!monoManager.allMyMonologues[monoManager.currentMonologue].repeatsAtFinish)
                {
                    MonologueTrigger m_Trigger = controller.wmManager.allMonologues[monoManager.allMyMonologues[monoManager.currentMonologue].worldMonoIndex].mTrigger;
                    
                    m_Trigger.gameObject.SetActive(false);
                }
            }
        }

        //can be called during IDLE state 
        void ActionCountdown()
        {
            actionTimer -= Time.deltaTime;

            //play a sound (cough)
            if (actionTimer < 0  && !controller.Monologues.inMonologue)
            {
                if (npcSounds.myAudioSource.isPlaying == false)
                {
                    if (npcSounds.idleSounds.Length > 0)
                        npcSounds.PlayRandomSoundRandomPitch(npcSounds.idleSounds, npcSounds.myAudioSource.volume);

                    //npcAnimations.Animator.SetTrigger("action1");

                    actionTimer = actionTime;
                }
            }
        }

        //resets the AI's movement type / path 
        public void ResetMovement(MovementPath newMove)
        {
            npcType = movementManager.movementPaths[newMove.pathIndex].moveType;

            //random npc move type 
            if(npcType == NPCMovementTypes.RANDOM)
            {
                movementRadius = movementManager.movementPaths[newMove.pathIndex].moveRadius;
            }
            //pathfinder or waypoint looper 
            else if(npcType == NPCMovementTypes.PATHFINDER || npcType == NPCMovementTypes.WAYPOINT)
            {
                waypoints = movementManager.movementPaths[newMove.pathIndex].movementPoints;
                waypointCounter = 0;

                waitingToGiveMonologue = false;
            }
        }

        //MOVING state
        void Moving()
        {
            //state check
            if (controller.npcState == Controller.NPCStates.MOVING)
            {
                //looks at targetPos when not waving 
                LookAtObject(targetPosition, false);

                //stop running after we are close to position
                if (Vector3.Distance(transform.position, targetPosition) < myNavMesh.stoppingDistance + 3f)
                {
                    SetIdle();
                }
            }
        }

        //Talking state --
        // can move to targetPosition, then ready to deliver monologue until player is near 
        void Talking()
        {
            //state check 
            if(controller.npcState == Controller.NPCStates.TALKING)
            {

            }
        }

        //looks at object
        void LookAtObject(Vector3 pos, bool useMyY)
        {
            //empty Vector 3
            Vector3 direction;

            //use my y Pos in Look pos
            if (useMyY)
            {
                //find direction from me to obj
                Vector3 posWithMyY = new Vector3(pos.x, transform.position.y, pos.z);
                direction = posWithMyY - transform.position;
            }
            //use obj y pos in Look pos
            else
            {
                //find direction from me to obj
                direction = pos - transform.position;
            }
           
            //find target look
            Quaternion targetLook = Quaternion.LookRotation(direction);
            //actually rotate the character 
            transform.rotation = Quaternion.Lerp(transform.rotation, targetLook, lookSmooth * Time.deltaTime);
        }

        //stops movement
        public virtual void SetIdle()
        {
            myNavMesh.isStopped = true;
            ResetStateTimer(idleTime);
            npcAnimations.SetAnimator("idle");
            controller.npcState = Controller.NPCStates.IDLE;
        }
        
        //resets state timer to float time + random range 
        void ResetStateTimer(float time)
        {
            stateTimer = time + Random.Range(-1f, 1f);
        }

        //this function sets a random point as the nav mesh destination
        //checks if the NPC can walk there before setting it
        //sets animator correctly
        public virtual void SetWaypoint(bool looping)
        {
            //when it reaches final point in list, it will reset counter to 0
            if (looping)
            {
                //increment waypoint counter
                if (waypointCounter < waypoints.Length - 1)
                {
                    waypointCounter++;
                }
                else
                {
                    waypointCounter = 0;
                }
            }
            //just goes until last point then stops
            else
            {
                //increment waypoint counter
                if (waypointCounter < waypoints.Length - 1)
                {
                    waypointCounter++;
                }
            }

            //set point to cast from 
            Vector3 castPoint = waypoints[waypointCounter].position;

            NavigateToPoint(castPoint, false);
        }

        //this function sets a random point as the nav mesh destination
        //checks if the NPC can walk there before setting it
        //sets animator correctly
        public virtual void SetRandomDestination()
        {
            Vector2 xz = Random.insideUnitCircle * movementRadius;

            Vector3 castPoint = new Vector3(xz.x + origPosition.x, transform.position.y + 10, xz.y + origPosition.z);

            NavigateToPoint(castPoint, false);
        }

        //base function for actually navigating to a point 
        public virtual void NavigateToPoint(Vector3 castPoint, bool hasMono)
        {
            RaycastHit hit;
            // Does the ray intersect any objects excluding the player layer
            if (Physics.Raycast(castPoint, Vector3.down, out hit, Mathf.Infinity, grounded))
            {
                targetPosition = hit.point;
            }

            myNavMesh.SetDestination(targetPosition);

            myNavMesh.isStopped = false;

            //set moving 
            controller.npcState = Controller.NPCStates.MOVING;
            npcAnimations.SetAnimator("moving");

            //character will wait when it arrives at point 
            if (hasMono)
            {
                waitingToGiveMonologue = true;
            }
        }
    }
}
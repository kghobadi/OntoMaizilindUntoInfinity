using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private CamObject camObj;

        [Header("AI Movement Settings")]
        public LayerMask grounded;
        [HideInInspector]
        public NavMeshAgent myNavMesh;
        public bool randomSpeed = true;
        [Tooltip("Random value within this range will be added to navmesh speed.")]
        public Vector2 speedRange = new Vector2(-5f, 10f);
        Vector3 origPosition;
        public MovementPath startBehavior;
        public Vector3 targetPosition;
        float distFromPlayer;
        public bool AIenabled = true;
        public Transform lookAtTransform;

        [Tooltip("Can set this true so that at the end of a moving state, we reset Movement")]
        public bool resetsMovement;
        public MovementPath newMovement;
        
        //state timers 
        public float stateTimer, actionTimer;
        public float idleTime, actionTime;
        public float interactDistance;
        public float lookSmooth = 5f;

        [Header("Monologue Wait")]
        public bool waitingToGiveMonologue;
        public float monologueWaitTimer = 0f, monoWaitTime = 30f;

        //chosen in editor 
        [Tooltip("Chosen by the Movement Path assigned to the NPC")]
        public NPCMovementTypes npcType;
        public enum NPCMovementTypes
        {
            WAYPOINT, RANDOM, IDLE, PATHFINDER, FINDPLAYER, FOLLOWER,
        }

        [Tooltip("Various Animation Types!")]
        public IdleType idleType;
        public enum IdleType
        {
            STANDING, SITTING, PRAYING, DEAD,
        }
        
        public RunType runType;
        public enum RunType
        {
            FASTRUN, HOLDINGCHILD, 
        }

        //allows mosque to access this 
        [HideInInspector] public SpiritTrail spiritTrail;

        [Header("Wanderer Settings")]
        public Transform[] waypoints;
        public int waypointCounter = 0;

        [Header("Random Settings")]
        public float movementRadius;

        [Header("Retrieve Player and Take to Shelter")]
        public MovementPath toShelter;
        public bool holdingPlayer;
        public Transform holdingSpot;
        private HeavyBreathing breathingSounds;
        public FaceAnimation faceAnimation;

        [Header("Follower Logic")] 
        public Transform followObject;

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
            spiritTrail = GetComponentInChildren<SpiritTrail>();
            breathingSounds = GetComponentInChildren<HeavyBreathing>();
            camObj = GetComponent<CamObject>();

            //player ref
            if (controller.camSwitcher)
                currentPlayer = controller.camSwitcher.currentPlayer;
            else
                currentPlayer = GameObject.FindGameObjectWithTag("Player");
        }

        void Start()
        { 
            origPosition = transform.position;
            if (randomSpeed)
            {
                RandomizeSpeed();
            }
            ResetMovement(startBehavior);
            SetIdle();
        }

        void RandomizeSpeed()
        {
            if (myNavMesh)
            {
                myNavMesh.speed += Random.Range(speedRange.x, speedRange.y);
            }
        }

        void Update()
        {
            //player ref -- this is giving OutOfRange Exception when player dies? or switches? 
            if (controller.camSwitcher)
                currentPlayer = controller.camSwitcher.currentPlayer;
                
            if (AIenabled)
            {
                //dist from player 
                //DistanceCheck();
                //idle state
                Idle();
                //moving state
                Moving();
                //talking state
                Talking();
                //finding player
                FindPlayer();
                //Follower behavior
                FollowObject();
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

        #region Idle State Methods

        //NPC idle state 
        void Idle()
        {
            if (controller.npcState == Controller.NPCStates.IDLE)
            {
                stateTimer -= Time.deltaTime;

                //only look at if we set it to 
                if (lookAtTransform)
                {
                    LookAtObject(lookAtTransform.position, true);
                }

                //NPC will perform its action 
                //ActionCountdown();

                //if we are not IDLE npc, idle state has a countdown until movement 
                if (npcType == NPCMovementTypes.IDLE)
                {
                    //play a sound
                    if (stateTimer < 0  && !controller.Monologues.inMonologue)
                    {
                        //idle action?

                        stateTimer = idleTime;
                    }

                    //if we are praying, start the Spiritus
                    if(idleType == IdleType.PRAYING)
                    {
                        if (spiritTrail)
                        {
                            if(spiritTrail.activated == false)
                            {
                                spiritTrail.EnableSpirit();
                            }
                        }
                    }
                    
                    //if we are Dead
                    if(idleType == IdleType.DEAD)
                    {
                        //zero local pos & rot
                        if (camObj)
                        {
                            camObj.myBody.transform.localPosition = Vector3.zero;
                            camObj.myBody.transform.localRotation = Quaternion.identity;
                        }
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
        
        //stops movement
        public virtual void SetIdle()
        {
            if(myNavMesh)
                myNavMesh.isStopped = true;
            stateTimer = idleTime;
            CheckIdleType();
            npcAnimations.SetAnimator("idle");
            controller.npcState = Controller.NPCStates.IDLE;
        }

        //switch idle type in animator!
        void CheckIdleType()
        {
            switch (idleType)
            {
                case IdleType.STANDING:
                    npcAnimations.Animator.SetFloat("IdleType", 0f);
                    break;
                case IdleType.SITTING:
                    npcAnimations.Animator.SetFloat("IdleType", 0.3333333f);
                    break;
                case IdleType.PRAYING:
                    npcAnimations.Animator.SetFloat("IdleType", 0.6666667f);
                    break;
                case IdleType.DEAD:
                    npcAnimations.Animator.SetFloat("IdleType", 1f);
                    
                    //disable sounds component
                    if (controller.Sounds)
                    {
                        controller.Sounds.animateFaceToSound = false;
                    }
                    
                    //set dead faces 
                    if (faceAnimation)
                    {
                        faceAnimation.SetAnimator("dead");
                        //if it has a back face
                        if(faceAnimation.back)
                            faceAnimation.back.SetActive(false);
                    }
                        
                    //drop player
                    if (holdingPlayer)
                    {
                        DropPlayer();
                    }
                    break;
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
        

        #endregion
        
        #region State Management

        //resets the AI's movement type / path behavior
        public void ResetMovement(MovementPath newMove)
        {
            npcType = movementManager.movementPaths[newMove.pathIndex].moveType;

            idleType = movementManager.movementPaths[newMove.pathIndex].idleType;
            
            runType =  movementManager.movementPaths[newMove.pathIndex].runType;
            
            SetRunType(runType);

            followObject = movementManager.movementPaths[newMove.pathIndex].followObject;

            //random npc move type 
            if (npcType == NPCMovementTypes.RANDOM)
            {
                movementRadius = movementManager.movementPaths[newMove.pathIndex].moveRadius;
            }
            //IDlE -- set new idle type?
            else if(npcType == NPCMovementTypes.IDLE)
            {
                SetIdle();
            }
            //pathfinder or waypoint looper 
            else if(npcType == NPCMovementTypes.PATHFINDER || npcType == NPCMovementTypes.WAYPOINT)
            {
                waypoints = movementManager.movementPaths[newMove.pathIndex].movementPoints;
                waypointCounter = 0;

                waitingToGiveMonologue = false;
            }

            resetsMovement = false;
        }

        //resets state timer to float time + random range 
        void ResetStateTimer(float time)
        {
            stateTimer = time + Random.Range(-1f, 1f);
        }

        #endregion

        #region Find & Carry Player Logic
        //have this NPC run to wherever the player is and pick them up. 
        void FindPlayer()
        {
            if (npcType == NPCMovementTypes.FINDPLAYER)
            {
                //are we close to player?
                if (Vector3.Distance(transform.position, currentPlayer.transform.position) < myNavMesh.stoppingDistance + 3f)
                {
                    PickUpPlayer();
                }
                //keep loooking
                else
                {
                    NavigateToPoint(currentPlayer.transform.position, false);
                }
            }   
        }

        /// <summary>
        /// Picks up player and attaches them to this NPC's holding spot. Triggers toShelter Movement behavior. 
        /// </summary>
        void PickUpPlayer()
        {
            //get fps
            FirstPersonController fps = controller.camSwitcher.currentPlayer.GetComponent<FirstPersonController>();

            //disable movement
            fps.DisableMovement();
            fps.beingHeld.Invoke();
            //set pos
            controller.camSwitcher.currentPlayer.transform.position = holdingSpot.position;
            controller.camSwitcher.currentPlayer.transform.SetParent(holdingSpot);
            holdingPlayer = true;
            //reset movement to shelter
            ResetMovement(toShelter);
            SetRunType(RunType.HOLDINGCHILD);
            //nullify look at transform
            lookAtTransform = null;
            //set heavy breathing
            if(breathingSounds) 
                breathingSounds.StartBreathing();
        }

        /// <summary>
        /// Drops player and lets them move
        /// </summary>
        public void DropPlayer()
        {
            //get fps
            FirstPersonController fps = controller.camSwitcher.currentPlayer.GetComponent<FirstPersonController>();

            //enable movement
            fps.EnableMovement();
            //set parent null
            controller.camSwitcher.currentPlayer.transform.SetParent(null);
          
            //stop breathing
            if(breathingSounds) 
                breathingSounds.StopBreathing();
            //done
            holdingPlayer = false;
        }
        #endregion

        #region Look At Logic
        //sets the npc look at  
        public void SetLook(Transform point)
        {
            lookAtTransform = point;
        }

        //set npc look at using move manager array 
        public void SetLookAt(int pointInManager)
        {
            Transform point = movementManager.lookAtObjects[pointInManager];

            lookAtTransform = point;
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

        

        #endregion

        #region Movement Methods
        //MOVING state
        void Moving()
        {
            //state check
            if (controller.npcState == Controller.NPCStates.MOVING)
            {
                //looks at targetPos when not waving 
                LookAtObject(targetPosition, false);
                
                //check holding
                if (holdingPlayer)
                {
                    //auto zero body's local rotation
                    camObj.myBody.transform.localRotation = Quaternion.identity;
                }

                //stop running after we are close to position
                if (Vector3.Distance(transform.position, targetPosition) < myNavMesh.stoppingDistance + 3f)
                {
                    //can be called by triggers or smth
                    if (resetsMovement)
                        ResetMovement(newMovement);

                    SetIdle();
                }
            }
        }
        
        /// <summary>
        /// changes the NPC's run type 
        /// </summary>
        /// <param name="run"></param>
        public void SetRunType(RunType run)
        {
            runType = run;
            
            switch (runType)
            {
                case RunType.FASTRUN:
                    npcAnimations.Animator.SetFloat("RunType", 0f);
                    break;
                case RunType.HOLDINGCHILD:
                    npcAnimations.Animator.SetFloat("RunType", 1f);
                    break;
            }
        }
        
        /// <summary>
        /// Follows the follow object set by a Follower behavior. 
        /// </summary>
        void FollowObject()
        {
            if (npcType == NPCMovementTypes.FOLLOWER)
            {
                //are we close to player?
                if (Vector3.Distance(transform.position, currentPlayer.transform.position) > myNavMesh.stoppingDistance + 1f)
                {
                    NavigateToPoint(followObject.position, false);
                }
            }
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
        

        #endregion
        
        //Talking state --
        // can move to targetPosition, then ready to deliver monologue until player is near 
        void Talking()
        {
            //state check 
            if(controller.npcState == Controller.NPCStates.TALKING)
            {

            }
        }
        
    }
}

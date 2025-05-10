using System.Collections;
using System.Collections.Generic;
using InControl;
using NPC;
using UnityEngine;
using UnityEngine.Serialization;
using Yarn.Unity;

/// <summary>
/// An interactive trigger for making a character animate a certain way & for holding/picking up the player. 
/// </summary>
public class AnimateCharacter : Interactive
{
    private ObjectAnimator _objectAnimator;
    private DialogueRunner _dialogueRunner;
    [SerializeField] private string animParamTrigger;
    
    //TODO plug into Facial animation system to trigger reactions? 
    //Maybe reactions should be triggered elsewhere. 

    [Tooltip("Check this to set an NPC idle type")]
    [SerializeField] private bool setIdleType;
    [SerializeField] private Movement npc;
    [SerializeField] private Movement.IdleType newIdle;
    private Movement.IdleType origIdleType;

    [Header("Hold Player?")] 
    [SerializeField]
    private bool holdsPlayer;
    public Transform spotToHold;
    public Transform standSpot; 
    public bool holding;
    private Vector3 playerPosition;
    private FirstPersonController fps;
    private InputDevice inputDevice;
    
    [SerializeField] private Sprite clickToGetDown;
    [SerializeField] private string getDown = "Get Down";
    
    [Header("Dialogue on Pickup?")] 
    [SerializeField]
    private bool triggersDialogue;
    [SerializeField] private string dialogueNode;

    [Header("Monologue on Pickup?")] 
    [SerializeField]
    private bool triggersMonologue;
    [SerializeField] private MonologueManager monoMgr;
    [SerializeField] private int monoIndex;
    
    protected override void Start()
    {
        base.Start();

        _dialogueRunner = FindObjectOfType<DialogueRunner>();
        //fetch object animator 
        _objectAnimator = GetComponent<ObjectAnimator>();
        if (_objectAnimator == null)
        {
            _objectAnimator = GetComponentInChildren<ObjectAnimator>();
            if (_objectAnimator == null)
            {
                _objectAnimator = GetComponentInParent<ObjectAnimator>();
            }
        }
    }
    
    protected override void Interact()
    {
        base.Interact();
        
        //trigger anim params
        if(!string.IsNullOrEmpty(animParamTrigger))
            _objectAnimator.Animator.SetTrigger(animParamTrigger);

        //Update a given NPC's idle type 
        if (setIdleType && npc)
        {
	        origIdleType = npc.idleType;
            npc.SetIdleType(newIdle);
        }

        if (holdsPlayer)
        {
	        //grab player pos
	        playerPosition = _cameraSwitcher.currentPlayer.transform.position;
	        //get fps
	        fps = _cameraSwitcher.currentPlayer.GetComponent<FirstPersonController>();

	        //only if the player is free to move can they be picked up by the character  
	        if (fps.canMove)
	        {
		        GoToCharacter();
	        }
        }

        //trigger monologue on pick up 
        if (triggersMonologue && monoMgr)
        {
	        monoMgr.WaitToSetNewMonologue(monoIndex);
        }

        //trigger dialogue on pickup 
        if (triggersDialogue && !string.IsNullOrEmpty(dialogueNode))
        {
	        //Try not to overlap with Monologue system for this character. 
	        if (monoMgr.inMonologue)
	        {
		        //Interrupt mono system to avoid clutter. 
		        if (monoMgr.interruptible)
		        {
			        monoMgr.MonologueReader.ManualEnd();
			        _dialogueRunner.StartDialogue(dialogueNode);
		        }
		        //Wait for mono to end. 
		        else
		        {
			        DoWaitToStartDialogue();
		        }
	        }
	        else
	        {
		        _dialogueRunner.StartDialogue(dialogueNode);
	        }
        }
    }

    void DoWaitToStartDialogue()
    {
	    if (waitToStartDialogue != null)
	    {
		    StopCoroutine(waitToStartDialogue);
	    }

	    waitToStartDialogue = WaitToStartDialogue();
	    StartCoroutine(waitToStartDialogue);
    }

    private IEnumerator waitToStartDialogue;
    IEnumerator WaitToStartDialogue()
    {
	    //Wait until I do not have a monologue - there is No dialogue, and I am still holding the player. 
	    yield return new WaitUntil(() => !monoMgr.inMonologue && !_dialogueRunner.IsDialogueRunning && holding);
	    
	    _dialogueRunner.StartDialogue(dialogueNode);
    }

	protected override void SetActive()
	{
		//get fps
		if (holdsPlayer)
		{
			fps = _cameraSwitcher.currentPlayer.GetComponent<FirstPersonController>();
			//check not already sitting && can move
			if (!holding && fps.canMove)
			{
				ShowAsInteractive();
			}
		}
		else
		{
			base.SetActive();
		}
	}
	
	void GoToCharacter()
	{
		//disable movement
		fps.canMove = false;
		//set pos
		_cameraSwitcher.currentPlayer.transform.position = spotToHold.position;
		//set rot
		GroundCamera cam = _cameraSwitcher.currentCamObj.GetGroundCam();
		cam.SetLookOverride(transform.position);
		if (npc.lookAtTransform == cam.transform)
		{
			npc.SetLook(null); //since they will look at you by default this is no longer necessary
		}

		//add event listener for disable sitting 
		fps.beingHeld.AddListener(DisableHolding);
		//set bool
		holding = true;
        
		//so its not highlighted anymore 
		SetInactive();
	}

	private void Update()
	{
		if (holding)
		{
			//get input device.
			inputDevice = InputManager.ActiveDevice;
			
			//set pos
			_cameraSwitcher.currentPlayer.transform.position = spotToHold.position;
			
			//check that we are not viewing an obj up close. could also check if we are holding something.
			if (_cameraSwitcher.objViewer.viewing == false && !_dialogueRunner.IsDialogueRunning)
			{
				//Interact again to get down 
				if ((Input.GetMouseButtonDown(0) || inputDevice.Action1.WasPressed || Input.GetKeyDown(KeyCode.Space))
				    && iCursor.CurrentText == getDown)
				{
					ReleasePlayer();
				}
				else
				{
					//show how to get down when not showing other things 
					if(clickToGetDown && !string.IsNullOrEmpty(getDown) &&!iCursor.active)
						iCursor.ActivateCursor(clickToGetDown, getDown);
				}
			}
			//Make sure to deactive cursor while dialogue-ing 
			else if (_dialogueRunner.IsDialogueRunning)
			{
				iCursor.Deactivate();
			}
		}
	}

	void ReleasePlayer()
	{
		//send player to stand spot 
		if (standSpot)
		{
			_cameraSwitcher.currentPlayer.transform.position = standSpot.position;
		}
		//send player back to pos they were at when they sat
		else
		{
			_cameraSwitcher.currentPlayer.transform.position = playerPosition;
		}
		
		//enable fps and remove listener
		fps.canMove = true;
		fps.beingHeld.RemoveListener(DisableHolding);

		//Return npc back to idle type prev 
		if (setIdleType)
		{
			npc.SetIdleType(origIdleType);
		}
		//todo make npc look back at something else / nothing 
		if (npc.lookAtTransform == _cameraSwitcher.currentCamObj.transform)
		{
			npc.SetLook(null); 
		}
		
		holding = false;
		iCursor.Deactivate();	
	}

	public void SetHolding()
	{
		holding = true;
	}

	//called if parent picks me up while being held 
	public void DisableHolding()
	{
		holding = false;
	}
}

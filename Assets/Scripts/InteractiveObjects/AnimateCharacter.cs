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
    
    [Header("Dialogue on Pickup?")] 
    [SerializeField]
    private bool triggersDialogue;
    [SerializeField] private string dialogueNode;

    [Header("Monologue on Pickup?")] 
    [SerializeField]
    private bool triggersMonologue;
    [SerializeField] private MonologueManager monoMgr;
    [SerializeField] private int monoIndex;
    
    
    
    //TODO add different dialogue AFTER the speech begins 
    
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
		        StartCoroutine(WaitToStartDialogue());
	        }
	        else
	        {
		        _dialogueRunner.StartDialogue(dialogueNode);
	        }
        }
    }

    IEnumerator WaitToStartDialogue()
    {
	    yield return new WaitUntil(() => !monoMgr.inMonologue);
	    
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
				base.SetActive();
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
				//right click to stand up or back button
				if (Input.GetMouseButtonDown(1) || inputDevice.Action2.WasPressed || Input.GetKeyDown(KeyCode.Space))
				{
					ReleasePlayer();
				}
			}
			
			//TODO Left mouse button / main input should be able to trigger Interaction for special cases
			//For Ramin - he will throw us up in the air. 
			//What about for others? 
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
		
		holding = false;
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

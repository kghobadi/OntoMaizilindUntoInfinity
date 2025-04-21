using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

/// <summary>
/// Allows the player to sit on various objects.
/// No movement while sitting, only looking. 
/// </summary>
public class SitOnObject : Interactive
{
	public Transform spotToSit;
	public Transform standSpot;
	public bool sitting;
	private Vector3 playerPosition;
	private FirstPersonController fps;
	private InputDevice inputDevice;

	protected override void SetActive()
	{
		//get fps
		fps = _cameraSwitcher.currentPlayer.GetComponent<FirstPersonController>();
		//check not already sitting && can move
		if (!sitting && fps.canMove)
		{
			base.SetActive();
		}
	}

	protected override void Interact()
	{
		base.Interact();
		
		//grab player pos
		playerPosition = _cameraSwitcher.currentPlayer.transform.position;
		//get fps
		fps = _cameraSwitcher.currentPlayer.GetComponent<FirstPersonController>();

		//only if the player is free to move can they sit 
		if (fps.canMove)
		{
			Sit();
		}
	}

	void Sit()
	{
		//disable movement
		fps.canMove = false;
		//set pos
		_cameraSwitcher.currentPlayer.transform.position = spotToSit.position;
		
		//add event listener for disable sitting 
		fps.beingHeld.AddListener(DisableSitting);
		//set bool
		sitting = true;
		//so its not highlighted anymore 
		SetInactive();
	}

	private void Update()
	{
		if (sitting)
		{
			//get input device.
			inputDevice = InputManager.ActiveDevice;
			
			//check that we are not viewing an obj up close. could also check if we are holding something.
			if (_cameraSwitcher.objViewer.viewing == false)
			{
				//right click to stand up or back button
				if (Input.GetMouseButtonDown(1) || inputDevice.Action2.WasPressed || Input.GetKeyDown(KeyCode.Space))
				{
					ReleasePlayer();
				}
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
		fps.beingHeld.RemoveListener(DisableSitting);
		
		sitting = false;
	}

	//called if parent picks me up while sitting
	public void DisableSitting()
	{
		sitting = false;
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
			//disable movement
			fps.canMove = false;
			//set pos
			_cameraSwitcher.currentPlayer.transform.position = spotToSit.position;
			//so its not highlighted anymore 
			SetInactive();

			sitting = true;
		}
	}

	private void Update()
	{
		if (sitting)
		{
			if (_cameraSwitcher.objViewer.viewing == false)
			{
				//left click to stand up
				if (Input.GetMouseButtonDown(1))
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
		
		fps.canMove = true;

		sitting = false;
	}
}

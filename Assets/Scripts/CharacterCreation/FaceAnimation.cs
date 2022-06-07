using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// The animator of the npc Faces, expressions, and reactions.
/// </summary>
public class FaceAnimation : AnimationHandler
{
	private NPC.Controller npcController;
	public NPC.Controller NpcController 
	{ 
		get
		{
			return npcController;
		}
		set 
		{ 
			npcController = value;
		}
	}
	public GameObject back;

	public UnityEvent onBeginFaceShifting;
	//Should add Reactions to the animator.
	//Will set it up like Idle, except some may not loop. 
	//Pass in an int for the state in a blendTree. 

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Spirit"))
		{
			TriggerFaceShifting();
		}
	}

	void TriggerFaceShifting()
	{
		onBeginFaceShifting.Invoke();
	}
}

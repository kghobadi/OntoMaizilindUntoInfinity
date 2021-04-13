using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows the player to pick up and hold a single object one at a time. 
/// </summary>
public class PickUpObject : Interactive 
{
	//is this really necessary? what if it causes some problems :(
	private Rigidbody _rigidbody;
	private FirstPersonController fpsHolder;
	private bool held;

	private Vector3 originalPos;
	private Transform originalParent;
	
	private Collider[] colliders;

	protected override void Start()
	{
		base.Start();
		_rigidbody = GetComponent<Rigidbody>();
		fpsHolder = _cameraSwitcher.currentPlayer.GetComponent<FirstPersonController>();
		colliders = GetComponentsInChildren<Collider>();
	}

	protected override void SetActive()
	{
		if (fpsHolder.holding == false)
		{
			base.SetActive();
		}
	}

	protected override void Interact()
	{
		if (fpsHolder.holding == false)
		{
			base.Interact();
			
			HoldItem();
		}
	}

	void HoldItem()
	{
		//parent this obj to fps holder
		transform.SetParent(fpsHolder.holdingSpot);
		//zero pos
		transform.localPosition = Vector3.zero;
		//zero rotation
		transform.localRotation = Quaternion.identity;	
		//set holding
		fpsHolder.SetHolding(this);
		//disable rbody
		_rigidbody.velocity = Vector3.zero;
		_rigidbody.isKinematic = true;
		_rigidbody.useGravity = false;
		//disable colliders
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = false;
		}
		//play sound?
		if(interactSound)
			_cameraSwitcher.objViewer.PlaySound(interactSound, 1f);
	}

	private void Update()
	{
		if (fpsHolder)
		{
			if (fpsHolder.holding && fpsHolder.pickUp == this)
			{
				if (Input.GetMouseButtonDown(1))
				{
					DropObject();
				}
			}
		}
	}

	void DropObject()
	{
		//reparent
		transform.SetParent(originalParent);
		//fps drop
		fpsHolder.DropObject();
		//enable rbody
		_rigidbody.isKinematic = false;
		_rigidbody.useGravity = true;
		//enable colliders
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = true;
		}
	}
	
}

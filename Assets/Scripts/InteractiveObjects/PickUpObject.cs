using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.Events;

/// <summary>
/// Allows the player to pick up and hold a single object one at a time. 
/// </summary>
public class PickUpObject : Interactive 
{
	//is this really necessary? what if it causes some problems :(
	private bool init;
	protected Rigidbody _rigidbody;
	public FirstPersonController fpsHolder;
	private bool held;

	private Vector3 originalPos;
	private Transform originalParent;
	
	private Collider[] colliders;
	private InputDevice inputDevice;

	private int holdingCounter;
	private int holdsNecToUse = 10;

	public bool lockInput; //locks input check
	public bool holdOverride; //allows you to take from someone else holding it 
	public bool noDropping; // for things lie the gun in the ending.
	public UnityEvent pickUpEvent;
	public FirstPersonController desiredHolder;
	
	//TODO add picked up object UI 
	//Should just be a single group of fade UIs 
	//with 2 images and 2 tmp texts
	//one for Use object and one for Drop object
	//All the items should recognize both mouse/controller inputs 

	protected override void Start()
	{
		base.Start();
		Init();
	}

	void Init()
	{
		if (init)
			return;
		_rigidbody = GetComponent<Rigidbody>();
		colliders = GetComponentsInChildren<Collider>();
		if (fpsHolder == null)
		{
			fpsHolder = _cameraSwitcher.currentPlayer.GetComponent<FirstPersonController>();
		}

		init = true;
	}

	protected override void SetActive()
	{
		if (fpsHolder.holding == false || holdOverride)
		{
			base.SetActive();
		}
	}

	protected override void Interact()
	{
		//if the fps is not holding anything.
		if (fpsHolder.holding == false || holdOverride)
		{
			base.Interact();
			
			HoldItem();
		}
	}
	
	/// <summary>
	/// allows you to set hold override. 
	/// </summary>
	/// <param name="over"></param>
	public void SetHoldOverride(bool over)
	{
		holdOverride = over;
	}
	
	/// <summary>
	/// allows you to set input lock override. 
	/// </summary>
	public void SetInputLock(bool locked)
	{
		lockInput = locked;
	}

	public virtual void HoldItem()
	{
		Init();
		
		//check for desired holder 
		if (desiredHolder)
		{
			if (transform.parent == fpsHolder.holdingSpot && fpsHolder != desiredHolder)
			{
				fpsHolder = desiredHolder;
			}
		}
		
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
		//reset holding counter
		holdingCounter = 0;
		
		//do we have a desired holder?
		if (desiredHolder != null)
		{
			//only call pickup event if it is desired holder. 
			if (fpsHolder == desiredHolder)
			{
				pickUpEvent?.Invoke();
			}
		}
		//call pickup event
		else
		{
			pickUpEvent?.Invoke();
		}
	}

	protected virtual void Update()
	{
		//take no input.
		if (lockInput)
		{
			return;
		}
		
		//get input device.
		inputDevice = InputManager.ActiveDevice;
		
		//if there is someone holding me.
		if (fpsHolder)
		{
			//if I am being held. 
			if (fpsHolder.holding && fpsHolder.pickUp == this)
			{
				//left click or main action to Use the object. -- must not be first frame of holding...
				if ((Input.GetMouseButtonDown(0)|| inputDevice.Action1.WasPressed) && holdingCounter > holdsNecToUse)
				{
					UseObject();
				}
				
				//right click or back button to Drop the object. 
				if (Input.GetMouseButtonDown(1)|| inputDevice.Action2.WasPressed || Input.GetKeyDown(KeyCode.Space))
				{
					//no drop!
					if (noDropping)
					{
						return;
					}
					
					DropObject();
				}

				//increment hold counter 
				holdingCounter++;
			}
		}
	}

	public virtual void UseObject()
	{
		//this is different depending on the object :)
	}
	
	protected virtual void DropObject()
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
	
	//TODO maybe need a proper Fall state for dropping objects? just add some downward force...
	
}

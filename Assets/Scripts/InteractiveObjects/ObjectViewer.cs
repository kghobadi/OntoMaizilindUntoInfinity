﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using InControl;

/// <summary>
/// Controls behavior of viewing an object up close.
/// Can only view one object at a time. 
/// </summary>
public class ObjectViewer : AudioHandler
{
	private InputDevice inputDevice;
	private CameraSwitcher camSwitcher;
	[Header("Object Viewing")]
	public ViewObject currentViewObj;
	public Transform viewPos;
	private Quaternion origViewPosRot;
	public bool viewing;
	private Camera mainCam;
	private int mainCullingMask;
	public GameObject[] viewObjectSetup;
	private bool couldMove;
	public float mouseRotSpeedX = 30f;
	public float mouseRotSpeedY = 30f;
	public float controllerRotSpeedX = 50f;
	public float controllerRotSpeedY = 50f;
	public bool speechStarted;
	[SerializeField] private Monologue anxietyMonologue;

	public void SetSpeechStarted(bool val)
	{
		speechStarted = val;
	}

	[SerializeField] private MonologueManager playerMonoManager;
	
	public float objectTextOffset = 0.75f;
	public TMP_Text objectDescription;
	private float origTextSize;
	private void Start()
	{
		camSwitcher = GetComponentInParent<CameraSwitcher>();
		camSwitcher.objViewer = this;
		mainCam = Camera.main;
		mainCullingMask = mainCam.cullingMask;
		if (objectDescription)
		{
			origTextSize = objectDescription.fontSize;
		}

		origViewPosRot = viewPos.localRotation;
	}

	//turn on object viewer with specific obj
	public void SetViewObject(ViewObject obj)
	{
		//set view obj
		currentViewObj = obj;
		//play interact sound
		PlaySound(obj.interactSound, 1f);

		//play view audio clip
		if (obj.viewAudioClip)
		{
			myAudioSource.clip = obj.viewAudioClip;
			myAudioSource.Play();
		}
		//disable player movement and camera controls
		couldMove = camSwitcher.CurrentFPC.canMove;
		camSwitcher.CurrentFPC.canMove = false;
		camSwitcher.currentCamObj.camObj.GetComponent<GroundCamera>().canControl = false;
		
		//disable objects colliders
		for (int i = 0; i < obj.colliders.Length; i++)
		{
			obj.colliders[i].enabled = false;
		}

		//reset view pos rotation 
		viewPos.localRotation = origViewPosRot;
		
		//set parent
		obj.transform.SetParent(viewPos);
		
		//set view obj pos to the view pos
		//check null string description of obj
		if (obj.objDescription == null)
		{
			//null -- center object in vew
			obj.transform.position = viewPos.position;
			objectDescription.enabled = false;

			//Set player thoughts monologue 
			if (obj.ObjectMonologue && !speechStarted)
			{
				//Don't proceed if this should not repeat. 
				if (!obj.RepeatsThought && obj.viewCounter > 0)
				{
					return;
				}

				//Can only think thoughts when not already in a player monologue
				//This actually feels like it should just cut off whatever is being thought before. 
				if (playerMonoManager.inMonologue)
				{
					//Prevent insta spam of the same object mono 
					if (playerMonoManager.CurrentMonologueData != obj.ObjectMonologue)
					{
						playerMonoManager.DisableMonologue();
						playerMonoManager.WaitToSetNewMonologue(obj.ObjectMonologue);
					}
				}
				else
				{
					playerMonoManager.SetMonologueSystem(obj.ObjectMonologue);
					playerMonoManager.EnableMonologue();
				}
			}
			//Repeat the anxiety monologue until the player gets the idea. 
			else if (speechStarted)
			{
				playerMonoManager.WaitToSetNewMonologue(anxietyMonologue);
			}
		}
		//there is a text asset 
		else
		{
			//is the text asset empty?
			if (string.IsNullOrEmpty(obj.objDescription.text))
			{
				//null -- center object in vew
				obj.transform.position = viewPos.position + obj.positionOffset;
				objectDescription.enabled = false;
			}
			//there is a string description for the item
			else
			{
				//move slightly to left
				obj.transform.localPosition = new Vector3(objectTextOffset, 0f, 0f) + obj.positionOffset;
				//enable text canvas and set description
				objectDescription.enabled = true;
				objectDescription.text = obj.objDescription.text;
				//set font size
				if (obj.fontSize > 0)
				{
					objectDescription.fontSize = obj.fontSize;
				}
			}
		}
		
		//set rotation
		obj.transform.localRotation = Quaternion.Euler(obj.rotationOffset); // this may be problematic
		//scale
		if (obj.scaleFactor != 1f)
			obj.transform.localScale *= obj.scaleFactor;
		//set obj layer to ObjectView
		obj.gameObject.layer = 16;
		for (int i = 0; i < obj.transform.childCount; i++)
		{
			obj.transform.GetChild(i).gameObject.layer = 16;
		}
		
		//set camera culling mask
		mainCam.cullingMask = 1 << LayerMask.NameToLayer("ObjectView");
		
		//enable object viewer setup
		for (int i = 0; i < viewObjectSetup.Length; i++)
		{
			viewObjectSetup[i].SetActive(true);
		}
		
		//disable Interact cursor
		InteractCursor.Instance.Deactivate();

		obj.viewCounter++;
		viewing = true;
	}

	private void Update()
	{
		//do we have a view obj
		if (viewing)
		{
			//get input device 
			inputDevice = InputManager.ActiveDevice;
			
			RotateObject();
			
			// right click to stop viewing or circle button
			if ( Input.GetMouseButtonDown(1) || inputDevice.Action2.WasPressed || Input.GetKeyDown(KeyCode.Space))
			{
				StopViewing();
			}
		}
	}

	//this allows user to rotate object on X | and Y --
	void RotateObject()
	{
		//get input
		float inputX;
		float inputY;

		Vector3 inputs = Vector3.zero;

		//controller
		if (inputDevice.DeviceClass == InputDeviceClass.Controller)
		{
			inputX = inputDevice.RightStickX;
			inputY = inputDevice.RightStickY;

			inputs = new Vector3(-inputY * mouseRotSpeedX * Time.deltaTime, -inputX * mouseRotSpeedY * Time.deltaTime, 0);
		
		}
		//mouse
		else
		{
			inputX = Input.GetAxis("Mouse X");
			inputY = Input.GetAxis("Mouse Y");
			
			//rotate
			inputs = new Vector3(-inputY * controllerRotSpeedX * Time.deltaTime,
				-inputX * controllerRotSpeedY * Time.deltaTime, 0);
		}
		
		//rotate
		viewPos.transform.Rotate(inputs);
	}

	//turn off object viewer and return obj to its original place. 
	public void StopViewing()
	{
		//reset camera culling mask to original
		mainCam.cullingMask = mainCullingMask;

		//enable player movement and camera controls
		camSwitcher.currentPlayer.GetComponent<FirstPersonController>().canMove = couldMove;
		camSwitcher.currentCamObj.camObj.GetComponent<GroundCamera>().canControl = true;
		
		//reset font size
		objectDescription.fontSize = origTextSize;
		
		//disable object viewer setup
		for (int i = 0; i < viewObjectSetup.Length; i++)
		{
			viewObjectSetup[i].SetActive(false);
		}
		
		//disable audio
		if (myAudioSource.clip != null)
		{
			myAudioSource.Stop();
			myAudioSource.clip = null;
		}
		
		viewing = false;
		
		//reset position
		currentViewObj.ResetViewObject();
		//nullify view obj
		currentViewObj = null;
	}
}

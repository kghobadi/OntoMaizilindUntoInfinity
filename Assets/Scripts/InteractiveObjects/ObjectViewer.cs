﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// Controls behavior of viewing an object up close.
/// Can only view one object at a time. 
/// </summary>
public class ObjectViewer : AudioHandler
{
	private CameraSwitcher camSwitcher;
	[Header("Object Viewing")]
	public ViewObject currentViewObj;
	public Transform viewPos;
	public bool viewing;
	private Camera mainCam;
	private int mainCullingMask;
	public GameObject[] viewObjectSetup;
	private bool couldMove;
	public float mouseRotSpeedX = 1f;
	public float mouseRotSpeedY = 1f;
	
	public float objectTextOffset = 0.75f;
	public TMP_Text objectDescription;
	private void Start()
	{
		camSwitcher = GetComponentInParent<CameraSwitcher>();
		camSwitcher.objViewer = this;
		mainCam = Camera.main;
		mainCullingMask = mainCam.cullingMask;
	}

	//turn on object viewer with specific obj
	public void SetViewObject(ViewObject obj)
	{
		//set view obj
		currentViewObj = obj;
		//play interact sound
		PlaySound(obj.interactSound, 1f);
		//disable player movement and camera controls
		couldMove = camSwitcher.currentPlayer.GetComponent<FirstPersonController>().canMove;
		camSwitcher.currentPlayer.GetComponent<FirstPersonController>().canMove = false;
		camSwitcher.currentCamObj.camObj.GetComponent<GroundCamera>().canControl = false;
		
		//disable objects colliders
		for (int i = 0; i < obj.colliders.Length; i++)
		{
			obj.colliders[i].enabled = false;
		}
		
		//set parent
		obj.transform.SetParent(viewPos);
		
		//set view obj pos to the view pos
		//check null string description of obj
		if (obj.objDescription == null)
		{
			//null -- center object in vew
			obj.transform.position = viewPos.position;
			objectDescription.enabled = false;
		}
		//there is a text asset 
		else
		{
			//is the text asset empty?
			if (String.IsNullOrEmpty(obj.objDescription.text))
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
			}
		}
		
		//set rotation
		obj.transform.localRotation = Quaternion.identity; // this may be problematic
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
		
		viewing = true;
	}

	private void Update()
	{
		//do we have a view obj
		if (viewing)
		{
			MouseRotateObject();
			
			// right click to stop viewing
			if ( Input.GetMouseButtonDown(1))
			{
				StopViewing();
			}
		}
	}

	//this allows user to rotate object on X | and Y --
	void MouseRotateObject()
	{
		//get input
		float inputX = Input.GetAxis("Mouse X");
		float inputY = Input.GetAxis("Mouse Y");
		
		//rotate
		currentViewObj.transform.Rotate(-inputY * mouseRotSpeedX * Time.deltaTime, inputX * mouseRotSpeedY * Time.deltaTime, 0);
	}

	//turn off object viewer and return obj to its original place. 
	public void StopViewing()
	{
		//reset position
		currentViewObj.ResetViewObject();
		//nullify view obj
		currentViewObj = null;
		
		//reset camera culling mask to original
		mainCam.cullingMask = mainCullingMask;

		//enable player movement and camera controls
		camSwitcher.currentPlayer.GetComponent<FirstPersonController>().canMove = couldMove;
		camSwitcher.currentCamObj.camObj.GetComponent<GroundCamera>().canControl = true;
		
		//disable object viewer setup
		for (int i = 0; i < viewObjectSetup.Length; i++)
		{
			viewObjectSetup[i].SetActive(false);
		}
		
		viewing = false;
	}
}
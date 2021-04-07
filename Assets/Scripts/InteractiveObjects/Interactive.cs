using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : AudioHandler
{
	protected CameraSwitcher _cameraSwitcher;
	
	[Header("Interactive Object Settings")]
	public bool active;
	public MeshRenderer _meshRenderer;
	public Material activeMat;
	public Material inactiveMat;
	public AudioClip interactSound;
	public float distNecessary = 7.5f;
	public FadeUI clickerUI;
	public bool hasClicked;
	
	protected virtual void Start ()
	{
		//cam switcher ref
		_cameraSwitcher = FindObjectOfType<CameraSwitcher>();
		
		//get mesh renderer
		if (_meshRenderer == null)
		{
			_meshRenderer = GetComponent<MeshRenderer>();
			if(_meshRenderer == null)
				_meshRenderer = GetComponentInChildren<MeshRenderer>();
		}
	}

	float CheckDistFromPlayer()
	{
		return Vector3.Distance(transform.position, _cameraSwitcher.currentPlayer.transform.position);
	}
	
	void OnMouseEnter()
	{
		if(CheckDistFromPlayer() < distNecessary)
			SetActive();
	}
	
	//for distance check
	void OnMouseOver()
	{
		if (active)
		{
			if (CheckDistFromPlayer() > distNecessary)
			{
				SetInactive();
			}
		}
		else
		{
			if(CheckDistFromPlayer() < distNecessary)
				SetActive();
		}
	}

	protected virtual void SetActive()
	{
		//highlight obj
		_meshRenderer.material = activeMat;
		active = true;

		//in case this object has clicker UI instructions
		if (!hasClicked)
		{
			if(clickerUI)
				clickerUI.FadeIn();
		}
	}

	void OnMouseDown()
	{
		if (active)
		{
			Interact();

			//if we have click UI, disable it and fade out permanently 
			if (clickerUI)
			{
				clickerUI.keepActive = false;
				clickerUI.FadeOut();
			}
				
			hasClicked = true;
		}
	}

	protected virtual void Interact()
	{
		Debug.Log("interacting with " + gameObject.name);
	}

	void OnMouseExit()
	{
		SetInactive();
	}

	protected virtual void SetInactive()
	{
		//unhighlight obj
		_meshRenderer.material = inactiveMat;
		active = false;
		
		//if we have ui to fade out 
		if (clickerUI)
		{
			if(clickerUI.gameObject.activeSelf)
				clickerUI.FadeOut();
		}
	}
}

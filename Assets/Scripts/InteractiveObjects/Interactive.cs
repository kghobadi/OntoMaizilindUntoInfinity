using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : AudioHandler
{
	protected CameraSwitcher _cameraSwitcher;
	protected InteractCursor iCursor;
	
	[Header("Interactive Object Settings")]
	public bool active;
	public MeshRenderer _meshRenderer;
	public SkinnedMeshRenderer _SkinnedMeshRenderer;
	public Sprite iCursorSprite;
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
		//iCursor ref
		iCursor = FindObjectOfType<InteractCursor>();
		
		//get mesh renderer
		if (_meshRenderer == null)
		{
			_meshRenderer = GetComponent<MeshRenderer>();
			if(_meshRenderer == null)
				_meshRenderer = GetComponentInChildren<MeshRenderer>();
		}
		//get skinned mesh renderer
		if (_SkinnedMeshRenderer == null)
		{
			_SkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
			if(_SkinnedMeshRenderer == null)
				_SkinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
		}
	}

	float CheckDistFromPlayer()
	{
		if (_cameraSwitcher == null)
			return 150f;
		
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
		if(_meshRenderer)
			_meshRenderer.material = activeMat;
		if (_SkinnedMeshRenderer)
			_SkinnedMeshRenderer.material = activeMat;
		active = true;
		
		//cursor
		iCursor.ActivateCursor(iCursorSprite);

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
		if(_meshRenderer)
			_meshRenderer.material = inactiveMat;
		if (_SkinnedMeshRenderer)
			_SkinnedMeshRenderer.material = inactiveMat;
		
		//cursor
		iCursor.Deactivate();
		
		active = false;
		
		//if we have ui to fade out 
		if (clickerUI)
		{
			if(clickerUI.gameObject.activeSelf)
				clickerUI.FadeOut();
		}
	}
}

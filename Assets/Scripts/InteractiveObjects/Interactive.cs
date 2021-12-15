using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This script is the base of the Interactive Objects system.
/// Makes any object interactive and connected to audio in relation the current player.
/// //TODO this script currently doesn't work with Controller
/// //Create an InteractiveRaycaster attached to the camera which all Interactives can use to get their raycast data from.
/// Will need to use ScreenPointToRay at the center of the screen. then check if it hits an Interactive object. 
/// </summary>
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

		//set interact hit event listeners
		if (InteractRaycaster.Instance != null)
		{
			InteractRaycaster.Instance.hitInteractiveObjectEvent.AddListener(CheckIfHit);
			InteractRaycaster.Instance.hitNothingEvent.AddListener(NothingHit);
			InteractRaycaster.Instance.interactInput.AddListener(OnInteractInput);
		}
	}

	#region Interact Raycaster Listeners

	/// <summary>
	/// Lets us know based on Raycast from screen if this object is current InteractObj
	/// </summary>
	/// <param name="interactObj"></param>
	void CheckIfHit(GameObject interactObj)
	{
		if (interactObj == gameObject)
		{
			CheckActive();
		}
		else
		{
			SetInactive();
		}
	}

	/// <summary>
	/// When nothing was detected by the Raycaster. 
	/// </summary>
	void NothingHit()
	{
		SetInactive();
	}

	#endregion
	
	float CheckDistFromPlayer()
	{
		if (_cameraSwitcher == null)
			return 150f;
		
		return Vector3.Distance(transform.position, _cameraSwitcher.currentPlayer.transform.position);
	}

	/// <summary>
	/// Called while player is looking directly at this. 
	/// </summary>
	void CheckActive()
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
			if (CheckDistFromPlayer() < distNecessary)
			{
				SetActive();
			}
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
	
	protected virtual void Interact()
	{
		Debug.Log("interacting with " + gameObject.name);
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

	void OnInteractInput()
	{
		//only interact if this obj is active :)
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

	#region Mouse Input
	void OnMouseEnter()
	{
		CheckActive();
	}
	
	//for distance check
	void OnMouseOver()
	{
		CheckActive();
	}
	
	void OnMouseDown()
	{
		OnInteractInput();
	}
	
	void OnMouseExit()
	{
		SetInactive();
	}
	#endregion
	

}

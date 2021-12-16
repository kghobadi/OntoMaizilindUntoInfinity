using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


/// <summary>
/// This script is the base of the Interactive Objects system.
/// Makes any object interactive and connected to audio in relation the current player.
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
	}
	
	//Add event listeners
	private void OnEnable()
	{
		EventManager.StartListening("OnInteractInput", OnInteractInput);
		EventManager.StartListening("OnHitInteractiveObject", CheckIfHit);
		EventManager.StartListening("OnHitNothing", NothingHit);
	}

	//Remove event listeners 
	private void OnDisable()
	{
		EventManager.StopListening("OnInteractInput", OnInteractInput);
		EventManager.StopListening("OnHitInteractiveObject", CheckIfHit);
		EventManager.StopListening("OnHitNothing", NothingHit);
	}

	#region Interact Raycaster Listeners

	/// <summary>
	/// Lets us know based on Raycast from screen if this object is current InteractObj
	/// </summary>
	/// <param name="interactObj"></param>
	void CheckIfHit(GameObject interactObj)
	{
		//check if the name is mine.
		if (interactObj == gameObject) //TODO must be something wrong with the event system. it only ever compares interactObj against 60_Gardin (1) 
		{
			CheckActive();
		}
		//dif name -- im not active.
		else
		{
			SetInactive();
		}
	}

	/// <summary>
	/// When nothing was detected by the Raycaster. 
	/// </summary>
	void NothingHit(GameObject obj)
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
			if (CheckDistFromPlayer() > distNecessary + 1f)
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

	void OnInteractInput(GameObject obj)
	{
		//null check on interact obj
		if (obj == null)
		{
			return;
		}
		
		//only interact if this obj is active :)
		if (active && obj == gameObject)
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
	/*void OnMouseEnter()
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
		OnInteractInput(gameObject);
	}
	
	void OnMouseExit()
	{
		SetInactive();
	}*/
	#endregion
	

}

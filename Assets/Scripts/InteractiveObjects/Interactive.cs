﻿using System;
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
	protected bool init;
	protected CameraSwitcher _cameraSwitcher;
	protected InteractCursor iCursor;
	protected float distFromPlayer;
	protected int interactableLayer = 21;
	
	[Header("Interactive Object Settings")]
	public bool active;
	public MeshRenderer _meshRenderer;
	public SkinnedMeshRenderer _SkinnedMeshRenderer;
	public Sprite iCursorSprite;
	public string interactMessage;
	public Material activeMat;
	private Material[] activeMats;
	public Material inactiveMat;
	private Material[] inactiveMats;
	public AudioClip interactSound;
	public float distNecessary = 7.5f;
	public FadeUI clickerUI;
	public bool hasClicked;
	
	[Header("Interact Event")]
	public bool oneTimeEvent;
	private int interactCount;
	public UnityEvent interactionEvent;

	protected virtual void Start ()
	{
		Init();
	}

	protected virtual void Init()
	{
		if (init)
		{
			return;
		}
		
		//cam switcher ref
		_cameraSwitcher = FindObjectOfType<CameraSwitcher>();
		//iCursor ref
		iCursor = InteractCursor.Instance;
		
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
		
		//generate mat arrays to length of renderer mats 
		if (_meshRenderer != null)
		{
			activeMats = new Material[_meshRenderer.materials.Length];
			inactiveMats = new Material[_meshRenderer.materials.Length];
		}
		if (_SkinnedMeshRenderer != null)
		{
			activeMats = new Material[_SkinnedMeshRenderer.materials.Length];
			inactiveMats = new Material[_SkinnedMeshRenderer.materials.Length];
		}
		
		//set up mat arrays if we have them 
		for (int i = 0; i < activeMats.Length; i++)
		{
			activeMats[i] = activeMat;
		}
		for (int i = 0; i < inactiveMats.Length; i++)
		{
			inactiveMats[i] = inactiveMat;
		}
		
		//set layer to interactable
		gameObject.layer = interactableLayer;

		init = true;
	}
	
	//Add event listeners
	private void OnEnable()
	{
		// subscribe to events
		InteractRaycaster.onHitInteractObj += CheckIfHit;
		InteractRaycaster.onHitNothing += NothingHit;
		InteractRaycaster.onInteractInput += OnInteractInput;
	}

	//Remove event listeners 
	private void OnDisable()
	{
		// unsubscribe to events
		InteractRaycaster.onHitInteractObj -= CheckIfHit;
		InteractRaycaster.onHitNothing -= NothingHit;
		InteractRaycaster.onInteractInput -= OnInteractInput;
	}

	#region Interact Raycaster Listeners

	/// <summary>
	/// Lets us know based on Raycast from screen if this object is current InteractObj
	/// </summary>
	/// <param name="interactObj"></param>
	void CheckIfHit()
	{
		//check if the name is mine.
		if (InteractRaycaster.Instance.currentInteractObject == gameObject) 
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
	void NothingHit()
	{
		SetInactive();
	}

	#endregion
	
	float CheckDistFromPlayer()
	{
		if (_cameraSwitcher == null)
		{
			if (InteractRaycaster.Instance.currentPlayerOverride == null)
			{
				return 150f;
			}
			
			distFromPlayer = Vector3.Distance(transform.position, InteractRaycaster.Instance.currentPlayerOverride.position);
		}
		else
		{
			distFromPlayer = Vector3.Distance(transform.position, _cameraSwitcher.currentPlayer.transform.position);
		}

		return distFromPlayer;
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
		Init();

		//Make sure we can interact before showing the UI 
		if (InteractCursor.Instance.CanInteract)
		{
			ShowAsInteractive();
		}
	}

	protected virtual void ShowAsInteractive()
	{
		SetMaterials(activeMat, activeMats);
			
		active = true;
		
		//cursor
		iCursor.ActivateCursor(iCursorSprite, interactMessage);

		//in case this object has clicker UI instructions
		if (!hasClicked)
		{
			if(clickerUI)
				clickerUI.FadeIn();
		}
	}

	protected void SetMaterials(Material material, Material[] materials)
	{
		//highlight obj
		if (_meshRenderer)
		{
			if (_meshRenderer.materials.Length > 1)
			{
				if (materials[0] == null)
					return;
				_meshRenderer.materials = materials;
			}
			else
			{
				if (material == null)
					return;
				_meshRenderer.material = material;
			}
		}
		if (_SkinnedMeshRenderer)
		{
			if (_SkinnedMeshRenderer.materials.Length > 0)
			{
				if (materials[0] == null)
					return;
				_SkinnedMeshRenderer.materials = materials;
			}
			else
			{
				if (material == null)
					return;
				_SkinnedMeshRenderer.material = material;
			}
		}
	}
	
	protected virtual void Interact()
	{
		Debug.Log("interacting with " + gameObject.name);
	}

	//set any events!
	protected virtual void TriggerInteractEvent()
	{
		if (oneTimeEvent)
		{
			if (interactCount == 0)
			{
				interactionEvent.Invoke();
			}

			interactCount++;
		}
		else
		{
			interactionEvent.Invoke();
		}
	}

	protected virtual void SetInactive()
	{
		Init();

		//already inactive
		if (!active)
		{
			return;
		}

		SetMaterials(inactiveMat, inactiveMats);
		
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
		//must be the interact obj
		if (InteractRaycaster.Instance.currentInteractObject == gameObject)
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
	}
}

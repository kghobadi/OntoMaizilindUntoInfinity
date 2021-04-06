using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactive : AudioHandler
{
	private CameraSwitcher _cameraSwitcher;
	
	[Header("Interactive Object Settings")]
	public bool active;
	public MeshRenderer _meshRenderer;
	public Material activeMat;
	public Material inactiveMat;
	public AudioClip interactSound;
	public float distNecessary = 7.5f;
	
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
	}

	void OnMouseDown()
	{
		if (active)
		{
			Interact();
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
	}
}

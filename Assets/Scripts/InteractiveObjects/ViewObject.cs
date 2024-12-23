﻿
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ViewObject : Interactive
{
	private ObjectViewer _objectViewer;
	private Vector3 originPos;
	private Quaternion originRot;
	private Vector3 originScale;
	private Transform originParent;
	private int sibIndex;
	public Collider[] colliders;

	[Header("View Object Settings")]
	public float scaleFactor = 1f;
	public Material viewMaterial;

	public Vector3 positionOffset = Vector3.zero;
	public Vector3 rotationOffset = Vector3.zero;
	//how to do a rotation offset?
	[SerializeField] private Monologue objMonologue;
	[SerializeField] private bool repeatThought = true;
	public Monologue ObjectMonologue => objMonologue;
	public bool RepeatsThought => repeatThought;
	public int viewCounter = 0;
	public TextAsset objDescription;
	public AudioClip viewAudioClip;
	public float fontSize = 0;
	
	public override void Awake()
	{
		base.Awake();
		_objectViewer = FindObjectOfType<ObjectViewer>();
		originPos = transform.localPosition;
		originRot = transform.rotation; //this may need to be a localized thing, just like with position
		originScale = transform.localScale;
		originParent = transform.parent;
		sibIndex = transform.GetSiblingIndex();
		colliders = GetComponentsInChildren<Collider>();
	}

	protected override void SetActive()
	{
		if (_objectViewer.viewing == false)
		{
			base.SetActive();
		}
		else if(_objectViewer.currentViewObj == this)
		{
			SetMaterials(viewMaterial, new []{viewMaterial});
		}
	}

	protected override void SetInactive()
	{
		if (_objectViewer.viewing == false)
		{
			base.SetInactive();
		}
		else if(_objectViewer.currentViewObj == this)
		{
			SetMaterials(viewMaterial, new []{viewMaterial});
		}
	}

	protected override void Interact()
	{
		if (_objectViewer.viewing == false && Time.timeScale >= 1)
		{
			_objectViewer.SetViewObject(this);
			SetInactive();
			iCursor.Deactivate();
			Debug.Log("Set view object to " + gameObject.name);
		}
		else
		{
			Debug.Log("Already viewing an object!");
		}
	}

	public void ResetViewObject()
	{
		//parent
		transform.SetParent(originParent);
		//sibling index
		transform.SetSiblingIndex(sibIndex);
		//pos
		transform.localPosition = originPos;
		//rotation
		transform.rotation = originRot;
		//scale
		transform.localScale = originScale;
		//layer
		gameObject.layer = interactableLayer;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.layer = 15;
		}
		//enable colliders
		for (int i = 0; i < colliders.Length; i++)
		{
			colliders[i].enabled = true;
		}
		
		TriggerInteractEvent();

		SetInactive();
	}
}

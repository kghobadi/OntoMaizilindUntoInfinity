
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	private Material[] viewMaterials;
	
	public Vector3 positionOffset = Vector3.zero;
	public Vector3 rotationOffset = Vector3.zero;
	//how to do a rotation offset?
	public TextAsset objDescription;
	
	//Properties
	public Material[] ViewMaterials => viewMaterials;
	
	private void Awake()
	{
		_objectViewer = FindObjectOfType<ObjectViewer>();
		originPos = transform.localPosition;
		originRot = transform.rotation; //this may need to be a localized thing, just like with position
		originScale = transform.localScale;
		originParent = transform.parent;
		sibIndex = transform.GetSiblingIndex();
		colliders = GetComponentsInChildren<Collider>();
	}

	protected override void Init()
	{
		base.Init();
		
		//generate view mat array to length of renderer mats 
		if (_meshRenderer != null)
		{
			viewMaterials = new Material[_meshRenderer.materials.Length];
		}
		if (_SkinnedMeshRenderer != null)
		{
			viewMaterials = new Material[_SkinnedMeshRenderer.materials.Length];
		}
		
		//set view mats 
		for (int i = 0; i < viewMaterials.Length; i++)
		{
			viewMaterials[i] = viewMaterial;
		}
	}

	protected override void SetActive()
	{
		if (_objectViewer.viewing == false)
		{
			base.SetActive();
		}
	}

	protected override void SetInactive()
	{
		Init();

		//unhighlight obj only if we are not in viewing mode
		if (_objectViewer.currentViewObj != this)
		{
			SetMaterials(inactiveMat, inactiveMats);
		}

		active = false;
		
		//if we have ui to fade out 
		if (clickerUI)
		{
			if(clickerUI.gameObject.activeSelf)
				clickerUI.FadeOut();
		}
	}

	protected override void Interact()
	{
		if (_objectViewer.viewing == false)
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
		
		//material back to inactive
		SetMaterials(inactiveMat, inactiveMats);
	}
}

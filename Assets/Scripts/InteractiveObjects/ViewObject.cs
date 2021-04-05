
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

	public float scaleFactor = 1f;
	public TextAsset objDescription;
	private void Awake()
	{
		_objectViewer = FindObjectOfType<ObjectViewer>();
		originPos = transform.position;
		originRot = transform.rotation;
		originScale = transform.localScale;
		originParent = transform.parent;
		sibIndex = transform.GetSiblingIndex();
	}

	protected override void SetActive()
	{
		if (_objectViewer.viewing == false)
		{
			base.SetActive();
		}
	}

	protected override void Interact()
	{
		if (_objectViewer.viewing == false)
		{
			_objectViewer.SetViewObject(this);
			SetInactive();
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
		transform.position = originPos;
		//rotation
		transform.rotation = originRot;
		//scale
		transform.localScale = originScale;
		//layer
		gameObject.layer = 15;
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.layer = 15;
		}
	}
}

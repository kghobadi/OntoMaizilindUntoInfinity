using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPosition : MonoBehaviour
{
	private Vector3 origPosLocal;
	private void Start()
	{
		origPosLocal = transform.localPosition;
	}

	public void SetPosOfObject(GameObject gameObject, Vector3 worldPosition)
	{
		gameObject.transform.position = worldPosition;
	}

	public Vector3 worldPosToSet;
	public void SetWorldPosition()
	{
		transform.position = worldPosToSet;
	}
	
	public Vector3 localPosToSet;
	public void SetLocalPosition()
	{
		transform.localPosition = localPosToSet;
	}

	public void SetUpdateLocalOrig(bool state)
	{
		updateLocalOrig = state;
	}

	[SerializeField] private bool updateLocalOrig;
	
	public void SetUpdateLocal(bool state)
	{
		updateLocal = state;
	}

	[SerializeField] private bool updateLocal;
	private void Update()
	{
		if (updateLocalOrig)
		{
			transform.localPosition = origPosLocal;
		}
		if (updateLocal)
		{
			SetLocalPosition();
		}
	}
}

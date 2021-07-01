using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetPosition : MonoBehaviour {

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
}

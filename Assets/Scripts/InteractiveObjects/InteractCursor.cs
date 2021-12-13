using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//The Interact system cursor.
//Referenced by all Interactive scripts and activated from there. 
//TODO SHOULD NOT APPEAR during pause. 
//Probably want to move this based on WorldPosOfObject + offset just like gravecat UI. 
public class InteractCursor : MonoBehaviour
{
	private Image imageHolder;
	public bool active;
	private void Start()
	{
		imageHolder = GetComponent<Image>();
		
		Deactivate();
	}

	public void ActivateCursor(Sprite newSprite)
	{
		if(newSprite)
			imageHolder.sprite = newSprite;
		imageHolder.enabled = true;
		active = true;
		
		//can pass in world pos for assign pos
	}

	public void Deactivate()
	{
		imageHolder.enabled = false;
		active = false;
	}
}

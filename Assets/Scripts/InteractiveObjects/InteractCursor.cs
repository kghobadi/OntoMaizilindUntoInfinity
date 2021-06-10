using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//The Interact system cursor.
//Referenced by all Interactive scripts and activated from there. 
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
	}

	public void Deactivate()
	{
		imageHolder.enabled = false;
		active = false;
	}
}

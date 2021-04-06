using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds off interactive, allows user to turn on or off a set of game objects by interacting with this.
/// Most common use case == light switches / an electronic on /off for radio, etc.
/// </summary>
public class TurnOnOrOff : Interactive
{
	public GameObject[] objects;
	public bool isOn;

	protected override void Interact()
	{
		//on -- turn off
		if (isOn)
		{
			for (int i = 0; i < objects.Length; i++)
			{
				objects[i].SetActive(false);
			}
		}
		//off -- turn on
		else
		{
			for (int i = 0; i < objects.Length; i++)
			{
				objects[i].SetActive(true);
			}
		}

		//play the sound!
		if(interactSound)
			PlaySound(interactSound, 1f);
		
		//switch bool
		isOn = !isOn;
	}
}

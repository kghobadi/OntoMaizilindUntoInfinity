using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds off interactive, allows user to turn on or off a set of game objects by interacting with this.
/// Most common use case == light switches / an electronic on /off for radio, etc.
/// </summary>
public class TurnOnOrOff : Interactive
{
    public bool isOn;

    public AudioClip offSound;
    //any game obj
	public GameObject[] objects;
	//sounds
	public AudioSource[] audioSources;
	public MusicFader[] musicFaders;

	protected override void Interact()
	{
		//on -- turn off
		if (isOn)
		{
			for (int i = 0; i < objects.Length; i++)
			{
				objects[i].SetActive(false);
			}
			
			//play the sound!
			if(offSound)
				_cameraSwitcher.objViewer.PlaySound(offSound, 1f);
			else
			{
				//play the sound!
				if(interactSound)
					_cameraSwitcher.objViewer.PlaySound(interactSound, 1f);
			}

			for (int i = 0; i < audioSources.Length; i++)
			{
				audioSources[i].Stop();
			}
			
			for (int i = 0; i < musicFaders.Length; i++)
			{
				musicFaders[i].FadeOut(0f, musicFaders[i].fadeSpeed);
			}
			
			
		}
		//off -- turn on
		else
		{
			for (int i = 0; i < objects.Length; i++)
			{
				objects[i].SetActive(true);
			}
			
			//play the sound!
			if(interactSound)
				_cameraSwitcher.objViewer.PlaySound(interactSound, 1f);
			
			for (int i = 0; i < audioSources.Length; i++)
			{
				audioSources[i].Play();
			}
			
			for (int i = 0; i < musicFaders.Length; i++)
			{
				musicFaders[i].SetSound(musicFaders[i].musicTrack);
				musicFaders[i].FadeIn(1f, musicFaders[i].fadeSpeed);
			}
		}
		
		//switch bool
		isOn = !isOn;
	}
}

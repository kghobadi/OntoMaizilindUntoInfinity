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
	[Tooltip("These particles will play while the object is set to on state.")]
	[SerializeField] 
	private ParticleSystem[] onFx;

	public override void Awake()
	{
		base.Awake();

		onFx = GetComponentsInChildren<ParticleSystem>();

		if (isOn)
		{
			//play on fx particles
			foreach (var fx in onFx)
			{
				fx.Play();
			}
		}
	}

	protected override void Interact()
	{
		//on -- turn off
		if (isOn)
		{
			TurnOff();
			
			//play the sound!
			if(offSound)
				_cameraSwitcher.objViewer.PlaySound(offSound, 1f);
			else
			{
				//play the sound!
				if(interactSound)
					_cameraSwitcher.objViewer.PlaySound(interactSound, 1f);
			}
		}
		//off -- turn on
		else
		{
			TurnOn();
			
			//play the sound!
			if(interactSound)
				_cameraSwitcher.objViewer.PlaySound(interactSound, 1f);
		}
		
		//switch bool
		isOn = !isOn;
	}

	/// <summary>
	/// Change this object to its on state. 
	/// </summary>
	public void TurnOn()
	{
		//enable any objects
		foreach (var obj in objects)
		{
			obj.SetActive(true);
		}
			
		//play on fx particles
		foreach (var fx in onFx)
		{
			fx.Play();
		}
		
		//play any audio sources
		foreach (var audioSource in audioSources)
		{
			audioSource.Play();
		}
			
		//set and fade in all music faders 
		foreach (var musicFader in musicFaders)
		{
			musicFader.SetSound(musicFader.musicTrack);
			musicFader.FadeIn(1f, musicFader.fadeSpeed);
		}
	}

	/// <summary>
	/// Change this object to its off state. 
	/// </summary>
	public void TurnOff()
	{
		//disable any objects
		foreach (var obj in objects)
		{
			obj.SetActive(false);
		}

		//stop on fx particles
		foreach (var fx in onFx)
		{
			fx.Stop();
		}
		
		//stop the audio source
		foreach (var audioSource in audioSources)
		{
			audioSource.Stop();
		}
			
		//fade out all music faders
		foreach (var musicFader in musicFaders)
		{
			musicFader.FadeOut(0f, musicFader.fadeSpeed);
		}
	}
}

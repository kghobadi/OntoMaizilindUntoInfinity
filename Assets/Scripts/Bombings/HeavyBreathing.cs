using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyBreathing : AudioHandler
{
	[Header("Breathing Sounds")] 
	public AudioClip[] breatheIns;
	public AudioClip[] breathOuts;

	[Header("Breathing Logic")] 
	public bool breathing;
	public bool inOrOut;
	public float breathTimer, breathTime = 1f;
	
	public void StartBreathing()
	{
		breathing = true;
	}

	public void StopBreathing()
	{
		breathing = false;
		breathTimer = breathTime;
	}

	private void Update()
	{
		if (breathing)
		{
			breathTimer -= Time.deltaTime;
			
			//breathe!
			if (breathTimer < 0)
			{
				PlayBreaths();
			}
		}
	}

	public void PlayBreaths()
	{
		//breathe in
		if (inOrOut)
		{
			PlayRandomSoundRandomPitch(breatheIns, 1f);
		}
		// breathe out
		else
		{
			PlayRandomSoundRandomPitch(breathOuts, 1f);
		}

		inOrOut = !inOrOut;
		breathTimer = breathTime;
	}
}

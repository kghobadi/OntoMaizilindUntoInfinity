using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Builds off interactive to allow user to open or close drawers and item doors (like the dresser). 
/// </summary>
public class OpenClose : Interactive
{
	private ObjectAnimator _objectAnimator;
	public bool isOpened;
	public AudioClip closeSound;
	
	protected override void Start()
	{
		base.Start();

		//fetch object animator 
		_objectAnimator = GetComponent<ObjectAnimator>();
		if (_objectAnimator == null)
		{
			_objectAnimator = GetComponentInChildren<ObjectAnimator>();
			if (_objectAnimator == null)
			{
				_objectAnimator = GetComponentInParent<ObjectAnimator>();
			}
		}
	}

	protected override void Interact()
	{
		base.Interact();
		//TODO need a limitation that stops user from doing this mid animation of openclose

		//opened -- close it!
		if (isOpened)
		{
			_objectAnimator.SetAnimator("close");
			
			//play the sound!
			if(closeSound)
				PlaySound(closeSound, 1f);
		}
		//closed -- open it!
		else
		{
			_objectAnimator.SetAnimator("open");
			//play the sound!
			if(interactSound)
				PlaySound(interactSound, 1f);
		}
		
		//switch bool
		isOpened = !isOpened;
	}
}

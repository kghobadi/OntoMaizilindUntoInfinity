using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceAnimationUI : AnimationHandler 
{
	private Image m_Image;
	private FadeUiRevamped faceFade;

	//private values for screen adjustments
	private float currentWidth;
	private float currentHeight;
	private float xMin, xMax;
	private float yMin, yMax;
	public bool active;
	public bool activateOnStart;

	protected override void Awake()
	{
		base.Awake();
		m_Image = GetComponent<Image>();
		faceFade = GetComponent<FadeUiRevamped>();
	}
	
	void Start ()
	{
		if(!activateOnStart)
			Deactivate();
	}

	public void Activate()
	{
		if(m_Image)
			m_Image.enabled = true;

		SetAnimator("talking");
		
		active = true;
	}

	public void SetIdle()
	{
		SetAnimator("idle");
	}

	public void Deactivate()
	{
		if(m_Image)
			m_Image.enabled = false;
		
		SetAnimator("idle");
		
		active = false;
	}

	/// <summary>
	/// Fades in the faces.
	/// </summary>
	public void FadeInFaces()
	{
		faceFade.FadeIn();
	}
	
	/// <summary>
	/// Fades out the faces.
	/// </summary>
	public void FadeOutFaces()
	{
		faceFade.FadeOut();
	}
}

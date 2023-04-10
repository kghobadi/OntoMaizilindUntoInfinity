using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceAnimationUI : AnimationHandler 
{
	private Image m_Image;
	private FadeUI faceFade;

	//private values for screen adjustments
	private float currentWidth;
	private float currentHeight;
	private float xMin, xMax;
	private float yMin, yMax;
	public bool active;

	void Awake()
	{
		m_Image = GetComponent<Image>();
		faceFade = GetComponent<FadeUI>();
	}
	
	void Start ()
	{
		Deactivate();
	}

	public void Activate()
	{
		if(m_Image)
			m_Image.enabled = true;
		
		SetAnimator("talking");
		
		active = true;
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

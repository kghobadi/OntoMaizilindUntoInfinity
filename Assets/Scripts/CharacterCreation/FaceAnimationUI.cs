using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceAnimationUI : AnimationHandler 
{
	private Image m_Image;

	//private values for screen adjustments
	private float currentWidth;
	private float currentHeight;
	private float xMin, xMax;
	private float yMin, yMax;
	public bool active;
	void Start ()
	{
		m_Image = GetComponent<Image>();
		
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
}

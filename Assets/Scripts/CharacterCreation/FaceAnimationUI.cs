using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceAnimationUI : AnimationHandler 
{
	private Image m_Image;
	Color alphaValue;
	//private values for screen adjustments
	private float currentWidth;
	private float currentHeight;
	private float xMin, xMax;
	private float yMin, yMax;
	public bool active;
	public bool activateOnStart;

	[SerializeField] private bool fadesOut;

	protected override void Awake()
	{
		base.Awake();
		m_Image = GetComponent<Image>();
		alphaValue = m_Image.color;
	}

	public void SetTransparent()
	{
		alphaValue.a = 0f;
		m_Image.color = alphaValue;
		active = false;
	}
	
	public void SetOpaque()
	{
		alphaValue.a = 1f;
		m_Image.color = alphaValue;
	}
	
	void Start ()
	{
		if(!activateOnStart)
			Deactivate();
	}

	public void Activate()
	{
		if (m_Image)
		{
			m_Image.enabled = true;
			if (fadesOut)
			{
				FadeInFaces();
			}
		}

		SetAnimator("talking");
		
		active = true;
	}

	public void SetIdle()
	{
		SetAnimator("idle");
	}

	public void Deactivate()
	{
		if (m_Image)
		{
			if (fadesOut)
			{
				FadeOutFaces();
			}
			else
			{
				m_Image.enabled = false;
			}
		}
		
		SetAnimator("idle");
		
		active = false;
	}

	/// <summary>
	/// Fades in the faces.
	/// </summary>
	public void FadeInFaces()
	{
		LeanTween.cancel(m_Image.gameObject);
		LeanTween.alpha(m_Image.rectTransform, 1f, 0.25f);
	}
	
	/// <summary>
	/// Fades out the faces.
	/// </summary>
	public void FadeOutFaces()
	{
		LeanTween.cancel(m_Image.gameObject);
		LeanTween.alpha(m_Image.rectTransform, 0f, 0.25f);
	}
}

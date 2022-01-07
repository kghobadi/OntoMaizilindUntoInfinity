using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceAnimationUI : AnimationHandler 
{
	private Camera mainCam;
	private RectTransform m_rectTransform;
	
	private RectTransform canvasRect;
	private Canvas parentCanvas;
	private Image m_Image;
	private Transform m_speaker;
	private MonologueReader monoReader;

	//private values for screen adjustments
	private float currentWidth;
	private float currentHeight;
	private float xMin, xMax;
	private float yMin, yMax;
	
	public bool active;
	private bool init;
	void Start ()
	{
		m_Image = GetComponent<Image>();
		m_rectTransform = GetComponent<RectTransform>();
		parentCanvas = GetComponentInParent<Canvas>();
		canvasRect = parentCanvas.GetComponent<RectTransform>();
		mainCam = Camera.main;
		
		Deactivate();
	}
	
	public void SetReader(MonologueReader reader, Transform speaker)
	{
		monoReader = reader;
		m_speaker = speaker;
		
		//set name based on the speaker 
		//gameObject.name = gameObject.name + "(" + speaker.parent.parent.name + ")";
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
	
	void Update () 
	{
		if (active)
		{
			RendererExtensions.AdjustScreenPosition(m_speaker.position, mainCam, canvasRect,m_rectTransform);
		}	
	}
}

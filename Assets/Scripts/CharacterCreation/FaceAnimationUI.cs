using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FaceAnimationUI : AnimationHandler {

	private Camera mainCam;
	private RectTransform m_rectTransform;
	private RectTransform canvasRect;
	private Image m_Image;
	private Transform m_speaker;
	private MonologueReader monoReader;

	[Tooltip("For adjusting the reader within screen bounds")]
	public Vector2 screenBoundsX = new Vector2(-400, 400);
	public Vector2 screenBoundsY = new Vector2(-175, 175);
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
		canvasRect = transform.parent.parent.GetComponent<RectTransform>();
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
			AdjustScreenPosition();
		}	
	}

	/// <summary>
	/// Move towards the closest point where to portray the location of the character which is speaking in world space. Stay on Screen. 
	/// </summary>
	public void AdjustScreenPosition()
	{
		//then you calculate the position of the UI element
		//0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0.
		//Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
		Vector2 ViewportPosition =mainCam.WorldToViewportPoint(m_speaker.position);
		Vector2 screenPos = new Vector2(
			((ViewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
			((ViewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

		//get current width 
		currentWidth = m_rectTransform.sizeDelta.x;
		//change x min/max based on current width
		xMin = screenBoundsX.x + (currentWidth / 2);
		xMax = screenBoundsX.y - (currentWidth / 2);
		//check screen bounds X min
		if (screenPos.x < xMin)
			screenPos = new Vector2(xMin, screenPos.y); 
		//check screen bounds X max
		if(screenPos.x > xMax)
			screenPos = new Vector2(xMax, screenPos.y);

		//get current height
		currentHeight = m_rectTransform.sizeDelta.y;
		//change y min/max based on current width
		yMin = screenBoundsY.x + (currentHeight / 2);
		yMax = screenBoundsY.y - (currentHeight / 2);
		//check screen bounds Y min
		if (screenPos.y < yMin)
			screenPos = new Vector2(screenPos.x, yMin); 
		//check screen bounds Y max
		if (screenPos.y > yMax)
			screenPos = new Vector2(screenPos.x, yMax); 
		
		//now you can set the position of the ui element
		m_rectTransform.anchoredPosition = screenPos;
	}
}

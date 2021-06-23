using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls behavior (movement) of main canvas readers.
/// </summary>
public class ScreenReader : MonoBehaviour
{
	private Camera mainCam;
	private TMP_Text m_text;
	private RectTransform m_rectTransform;
	private RectTransform canvasRect;
	private Image m_Image;
	private Transform m_speaker;
	
	[Tooltip("Offset for face icon")]
	public float iconAdjust;
	[Tooltip("Max width of text back")]
	public float maxWidth = 750f;
	[Tooltip("Offset to image behind text")]
	public float sideOffset = 25f;

	[Tooltip("For adjusting the reader within screen bounds")]
	public Vector2 screenBoundsX = new Vector2(-400, 400);
	public Vector2 screenBoundsY = new Vector2(-175, 175);
	
	public MonologueReader monoReader;
	public bool active;
	private bool init;
	void Start ()
	{
		Init();
	}
	void Init()
	{
		if(init)
			return;

		m_text = GetComponentInChildren<TMP_Text>();
		m_Image = GetComponent<Image>();
		m_rectTransform = GetComponent<RectTransform>();
		canvasRect = transform.parent.parent.GetComponent<RectTransform>();
		Debug.Log(transform.parent.parent.name);
		mainCam = Camera.main;

		init = true;
	}
	public void SetReader(MonologueReader reader, Transform speaker)
	{
		monoReader = reader;
		m_speaker = speaker;
		//set name based on the speaker 
		gameObject.name = gameObject.name + "(" + speaker.parent.parent.name + ")";
	}
	public void SetText(string mono)
	{
		Init();
		
		m_text.text = mono;
		ChangeWidthOfObject();
	}
	
	public void Activate()
	{
		Init();
		
		m_text.enabled = true;
		m_Image.enabled = true;
		active = true;
	}

	public void Deactivate()
	{
		m_text.enabled = false;
		m_Image.enabled = false;
		active = false;
	}
	
	void Update () 
	{
		if (active)
		{
			AdjustScreenPosition();
			
			AdjustScale();
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
			((ViewportPosition.x*canvasRect.sizeDelta.x)-(canvasRect.sizeDelta.x*0.5f)),
			((ViewportPosition.y*canvasRect.sizeDelta.y)-(canvasRect.sizeDelta.y*0.5f)));

		//get current width 
		float currentWidth = m_rectTransform.sizeDelta.x + sideOffset;
		//change x min/max based on current width
		float xMin = screenBoundsX.x + (currentWidth / 2);
		float xMax = screenBoundsX.y - (currentWidth / 2);
		//check screen bounds X min
		if (screenPos.x < xMin)
			screenPos = new Vector2(xMin, screenPos.y); 
		//check screen bounds X max
		if(screenPos.x > xMax)
			screenPos = new Vector2(xMax, screenPos.y); 
		//check screen bounds Y min
		if (screenPos.y < screenBoundsY.x)
			screenPos = new Vector2(screenPos.x, screenBoundsY.x); 
		//check screen bounds Y max
		if (screenPos.y > screenBoundsY.y)
			screenPos = new Vector2(screenPos.x, screenBoundsY.y); 
		
		//now you can set the position of the ui element
		m_rectTransform.anchoredPosition = screenPos;
		//Debug.Log("Adjusting pos to " + screenPos);
	}

	/// <summary>
	/// Adjusts scale of the screen reader depending on how player's distance from the character who is speaking. 
	/// </summary>
	public void AdjustScale()
	{
		//get distance between speaker and player
		//float dist = Vector3.Distance(m_speaker.position, )
		
		//adjust scale...
	}

	/// <summary>
	/// Set the face icon to the correct face & animate it when necessary. 
	/// </summary>
	public void ShowFace()
	{
		
	}

	private float width;
	private void ChangeWidthOfObject()
	{
		width = m_text.preferredWidth + iconAdjust;
		//set to width if it is less than max
		if (width < maxWidth)
		{
			m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width + sideOffset);
			m_text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		}
		//set to max width 
		else
		{
			m_rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth + sideOffset);
			m_text.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
		}
	}
}

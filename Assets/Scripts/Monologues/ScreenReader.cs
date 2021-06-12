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
	private Image m_Image;
	private Transform m_speaker;
	
	[Tooltip("Offset for face icon")]
	public float iconAdjust;
	[Tooltip("Max width of text back")]
	public float maxWidth = 750f;
	[Tooltip("Offset to image behind text")]
	public float sideOffset = 25f;

	[Tooltip("For adjusting the reader within screen bounds")]
	public Vector2 screenBoundsX = new Vector2(-250, 250);
	public Vector2 screenBoundsY = new Vector2(-175, 175);
	public float adjustSpeed = 1f;
	
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
		mainCam = Camera.main;

		init = true;
	}

	public void SetReader(MonologueReader reader, Transform speaker)
	{
		monoReader = reader;
		m_speaker = speaker;
	}
	
	void Update () 
	{
		if (active)
		{
			AdjustScreenPosition();
			
			AdjustScale();
		}	
	}

	public void SetText(string mono)
	{
		Init();
		
		m_text.text = mono;
		ChangeWidthOfObject();
	}

	/// <summary>
	/// Move towards the closest point where to portray the location of the character which is speaking in world space. Stay on Screen. 
	/// </summary>
	public void AdjustScreenPosition()
	{
		//convert position of the speaker (from world to screen)
		Vector3 screenPos = mainCam.WorldToScreenPoint(m_speaker.position);
		//check screen bounds X min
		if (screenPos.x < screenBoundsX.x)
			screenPos = new Vector3(screenBoundsX.x, screenPos.y, screenPos.z); 
		//check screen bounds X max
		if(screenPos.x > screenBoundsX.y)
			screenPos = new Vector3(screenBoundsX.y, screenPos.y, screenPos.z); 
		//check screen bounds Y min
		if (screenPos.y < screenBoundsY.x)
			screenPos = new Vector3(screenPos.x, screenBoundsY.x, screenPos.z); 
		//check screen bounds Y max
		if (screenPos.y > screenBoundsY.y)
			screenPos = new Vector3(screenPos.x, screenBoundsY.y, screenPos.z); 
			
		//use move towards to move rect transform towards screen pos as long as my current pos is within bounds. 
		m_rectTransform.position = Vector3.MoveTowards(m_rectTransform.position, screenPos, adjustSpeed * Time.deltaTime);
		Debug.Log("Adjusting pos from " + screenPos+ " pixels from the left " + screenPos);
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
	
	private void ChangeWidthOfObject()
	{
		var width = m_text.preferredWidth + iconAdjust;
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

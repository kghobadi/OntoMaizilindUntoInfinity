using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controls behavior (movement) of main canvas readers.
/// TODO could build a UIBase from this script and create coroutines for running the screen adjustment & other visual methods run in Update from RenderExtensions.cs 
/// </summary>
public class ScreenReader : MonoBehaviour
{
	private Camera mainCam;
	private TMP_Text m_text;
	private RectTransform m_rectTransform;
	private Canvas parentCanvas;
	private CanvasScaler canvasScaler;
	private RectTransform canvasRect;
	private Image m_Image;
	private Transform m_speaker;
	
	[Tooltip("Offset for face icon")]
	public float iconAdjust;
	[Tooltip("Max width of text back")]
	public float maxWidth = 750f;
	[Tooltip("Offset to image behind text")]
	public float sideOffset = 25f;

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
		parentCanvas = GetComponentInParent<Canvas>();
		canvasRect = parentCanvas.GetComponent<RectTransform>();
		canvasScaler = parentCanvas.GetComponent<CanvasScaler>();
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
		
		RendererExtensions.ChangeWidthOfObject(m_rectTransform, m_text, maxWidth, sideOffset);
	}
	
	public void Activate()
	{
		Init();
		
		if(m_text)
			m_text.enabled = true;
		if(m_Image)
			m_Image.enabled = true;
		active = true;
	}

	public void Deactivate()
	{
		if(m_text)
			m_text.enabled = false;
		if(m_Image)
			m_Image.enabled = false;
		active = false;
	}
	
	void Update () 
	{
		if (active)
		{
		  	RendererExtensions.AdjustScreenPosition(m_speaker.position, mainCam, canvasRect, m_rectTransform,canvasScaler);
			
			RendererExtensions.AdjustScale();	
		}	
	}

	/// <summary>
	/// Set the face icon to the correct face & animate it when necessary. 
	/// </summary>
	public void ShowFace()
	{
		
	}
}

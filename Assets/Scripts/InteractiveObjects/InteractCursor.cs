using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//The Interact system cursor.
//Referenced by all Interactive scripts and activated from there. 
//TODO SHOULD NOT APPEAR during pause. 
public class InteractCursor : MonoBehaviour
{  
	//private refs to the UI components.
	private Camera mainCam;
	private Canvas parentCanvas;
	private RectTransform canvasRect;
	private CanvasScaler canvasScaler;
	private RectTransform m_rectTransform;
	private Image imageHolder;
	public bool active;
	private void Start()
	{
		//get Ui components
		mainCam = Camera.main;
		parentCanvas = GetComponentInParent<Canvas>();
		canvasRect = parentCanvas.GetComponent<RectTransform>();
		canvasScaler = parentCanvas.GetComponent<CanvasScaler>();
		m_rectTransform = GetComponent<RectTransform>();
		imageHolder = GetComponent<Image>();
		
		Deactivate();
	}

	public void ActivateCursor(Sprite newSprite)
	{
		if(newSprite)
			imageHolder.sprite = newSprite;
		imageHolder.enabled = true;
		active = true;
		
		//can pass in world pos for assign pos
		//AdjustScreenPosition(worldPosition);
	}
	
	public void Deactivate()
	{
		imageHolder.enabled = false;
		active = false;
	}
	
	/// <summary>
	/// Set UI screen position based on world pos. Stay on Screen. 
	/// </summary>
	public void AdjustScreenPosition(Vector3 worldPos)
	{
		//then you calculate the position of the UI element
		//0,0 for the canvas is at the center of the screen, whereas WorldToViewPortPoint treats the lower left corner as 0,0.
		//Because of this, you need to subtract the height / width of the canvas * 0.5 to get the correct position.
		Vector2 ViewportPosition = mainCam.WorldToViewportPoint(worldPos);
		Vector2 screenPos = new Vector2(
			((ViewportPosition.x*canvasRect.sizeDelta.x)-(canvasRect.sizeDelta.x*0.5f)),
			((ViewportPosition.y*canvasRect.sizeDelta.y)-(canvasRect.sizeDelta.y*0.5f)));

		//get current width & height
		float currentWidth = m_rectTransform.sizeDelta.x;
		float currentHeight = m_rectTransform.sizeDelta.y;
		//change x min/max based on current width
		float xMin = -canvasScaler.referenceResolution.x / 2 + (currentWidth / 2);
		float xMax = canvasScaler.referenceResolution.x / 2 - (currentWidth / 2);
		//get y min / max
		float yMin = -canvasScaler.referenceResolution.y / 2 + (currentHeight / 2);
		float yMax = canvasScaler.referenceResolution.y / 2 - (currentHeight / 2);
		//check screen bounds X min
		if (screenPos.x < xMin)
			screenPos = new Vector2(xMin, screenPos.y); 
		//check screen bounds X max
		if(screenPos.x > xMax)
			screenPos = new Vector2(xMax, screenPos.y); 
		//check screen bounds Y min
		if (screenPos.y < yMin)
			screenPos = new Vector2(screenPos.x, yMin); 
		//check screen bounds Y max
		if (screenPos.y > yMax)
			screenPos = new Vector2(screenPos.x, yMax); 
		
		//now you can set the position of the ui element
		m_rectTransform.anchoredPosition = screenPos;
		//Debug.Log("Adjusting pos to " + screenPos);
	}
}

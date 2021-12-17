using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//The Interact system cursor.
//Referenced by all Interactive scripts and activated from there. 
//TODO SHOULD NOT APPEAR during pause. 
public class InteractCursor : NonInstantiatingSingleton<InteractCursor>
{
	protected override InteractCursor GetInstance () { return this; }
	private bool init;
	//private refs to the UI components.
	private Camera mainCam;
	private Canvas parentCanvas;
	private RectTransform canvasRect;
	private CanvasScaler canvasScaler;
	private RectTransform m_rectTransform;
	private Image imageHolder;
	public bool active;

	protected override void OnAwake()
	{
		base.OnAwake();
		
		Init();
	}
	
	void Init()
	{
		if (init)
		{
			return;
		}
		
		//get Ui components
		mainCam = Camera.main;
		parentCanvas = GetComponentInParent<Canvas>();
		canvasRect = parentCanvas.GetComponent<RectTransform>();
		canvasScaler = parentCanvas.GetComponent<CanvasScaler>();
		m_rectTransform = GetComponent<RectTransform>();
		imageHolder = GetComponent<Image>();

		init = true;
	}

	private void Start()
	{
		Deactivate();
	}

	private void OnEnable()
	{
		//events
		InteractRaycaster.onHitNothing += Deactivate;
	}

	private void OnDisable()
	{
		//events
		InteractRaycaster.onHitNothing -= Deactivate;
	}

	public void ActivateCursor(Sprite newSprite)
	{
		Init();

		if (newSprite != null)
		{
			imageHolder.sprite = newSprite;
		}
			
		imageHolder.enabled = true;
		active = true;
		
		//can pass in world pos for assign pos from the Interactive obj
		//RenderExtensions.AdjustScreenPosition(worldPosition);
	}
	
	public void Deactivate()
	{
		Init();
		
		imageHolder.enabled = false;
		active = false;
	}
}

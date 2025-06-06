﻿using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
	private CameraSwitcher camSwitcher;
	private PauseMenu pauseMenu;
	private Canvas parentCanvas;
	private RectTransform canvasRect;
	private CanvasScaler canvasScaler;
	private RectTransform m_rectTransform;
	private Image imageHolder;
	private Sprite origSprite;
	public Sprite CurrentSprite => imageHolder.sprite;
	public string CurrentText => interactTexts[0].text;
	private TMP_Text[] interactTexts;
	private ObjectViewer objectViewer;
	public ObjectViewer ObjectViewer => objectViewer;
	public bool active;
	
	public bool CanInteract
	{
		get
		{
			
			bool canInteract = !((camSwitcher && camSwitcher.CurrentFPC.holding) || (objectViewer && objectViewer.viewing));

			return canInteract;
		}
	}

	public float CurrentPlayerSpeed => camSwitcher.CurrentFPC.currentSpeed;

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
		camSwitcher = FindObjectOfType<CameraSwitcher>();
		pauseMenu = FindObjectOfType<PauseMenu>();
		parentCanvas = GetComponentInParent<Canvas>();
		canvasRect = parentCanvas.GetComponent<RectTransform>();
		canvasScaler = parentCanvas.GetComponent<CanvasScaler>();
		m_rectTransform = GetComponent<RectTransform>();
		imageHolder = GetComponent<Image>();
		origSprite = imageHolder.sprite;
		interactTexts = GetComponentsInChildren<TMP_Text>();
		objectViewer = FindObjectOfType<ObjectViewer>();

		init = true;
	}

	private void Start()
	{
		Deactivate();
	}

	private void OnEnable()
	{
		//events
		InteractRaycaster.onHitNothing += OnHitNothing;
	}

	private void OnDisable()
	{
		//events
		InteractRaycaster.onHitNothing -= OnHitNothing;
	}

	public void ActivateCursor(Sprite newSprite, string message)
	{
		Init();

		//Object viewer active check
		if (objectViewer && objectViewer.viewing) 
		{
			return;
		}

		//do we have a unique interact sprite?
		if (newSprite != null)
		{
			imageHolder.sprite = newSprite;
		}
		
		SetInteractTexts(message);
		//Check if the speech started 
		if (objectViewer &&  objectViewer.speechStarted) 
		{
			//IF you just want to pass in the new text for other checks but don't enable. 
			// foreach (var interactText in interactTexts)
			// {
			// 	interactText.text = message;
			// }
			return;
		}
	
		Cursor.visible = true;
		imageHolder.enabled = true;
		active = true;
		
		//can pass in world pos for assign pos from the Interactive obj
		//RenderExtensions.AdjustScreenPosition(worldPosition);
	}

	void SetInteractTexts(string message)
	{
		//do we have a message?
		if (string.IsNullOrEmpty(message) == false)
		{
			foreach (var interactText in interactTexts)
			{
				interactText.text = message;
				interactText.enabled = true;
			}
		}
		//no message, text disabled
		else
		{
			DisableInteractTexts();
		}
	}

	/// <summary>
	/// Only call deactivate when player is not holding anything. 
	/// </summary>
	void OnHitNothing()
	{
		if (camSwitcher) //TODO can add other conditions if necessary. 
		{
			if (!camSwitcher.CurrentFPC.holding && camSwitcher.CurrentFPC.canMove)
			{
				Deactivate();
			}
			else if (camSwitcher.CurrentFPC.holding)
			{
				if (interactTexts[0].text.Contains("Go to"))
				{
					Deactivate();
				}
			}
			else if (!camSwitcher.CurrentFPC.canMove)
			{
				if (interactTexts[0].text != "Get Up" && interactTexts[0].text != "Get Down")
				{
					Deactivate();
				}
			}
		}
		else
		{
			Deactivate();
		}
	}
	
	public void Deactivate()
	{
		Init();

		if (!pauseMenu.paused)
		{
			Cursor.visible = false;
		}
	
		imageHolder.enabled = false;
		DisableInteractTexts();
		active = false;
	}

	void DisableInteractTexts()
	{
		foreach (var interactText in interactTexts)
		{
			interactText.enabled = false;
		}
	}
}

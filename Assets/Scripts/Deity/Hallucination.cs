using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.PostProcessing;

/// <summary>
/// Holds Hallucination data and can be played after Ramin kills specific Deity.
/// </summary>
public class Hallucination : MonoBehaviour
{
	private ThePilot pilot;
	private PilotView pilotCam;
	public DeityManager deityMan;
	
	private Camera mainCam;
	private PostProcessingBehaviour camBehavior;
	public PostProcessingProfile hallucProfile;
	public PostProcessingProfile normalProfile;
	public float hallucinationEndWait = 3f;
	public GameObject[] hallucObjects;
	public bool hallucinating;

	[Header("Camera Transitions")] 
	public bool playOnStart;
	public Camera renderCam;
	private PostProcessingBehaviour hallucCamBehavior;
	private GroundCamera camMover;
	public Transform cameraPos;
	public Transform camLookAt;
	public Transform playerToParent;
	public Vector3 camLocalPos;
	[Tooltip("The Hallucination view raw image")]
	public FadeUI imageFade;
	[Tooltip("Air player in nuclearity.")]
	public camMouseLook airPlayer;

	[Header("Monologues")] 
	public MonologueManager[] monoManagers;
	public int[] monoIndeces;
	
	[Header("Events")] 
	public UnityEvent[] events;
	public UnityEvent[] endEvents;

	[Header("Yarn Variable")] 
	public string variableName;
	public string yarnValue;
	private VariableInterface _variableInterface;
	
	void Start()
	{
		//pilot refs
		pilot = FindObjectOfType<ThePilot>();
		if (pilot)
		{
			pilotCam = pilot.fpCam.GetComponent<PilotView>();
		}
		
		//deity refs
		if (deityMan == null)
		{
			deityMan = FindObjectOfType<DeityManager>();
		}
		
		//cam refs
		mainCam = Camera.main;
		camBehavior = mainCam.GetComponent<PostProcessingBehaviour>();
		hallucCamBehavior = renderCam.GetComponent<PostProcessingBehaviour>();

		//check for cam movement
		if (renderCam)
			camMover = renderCam.GetComponent<GroundCamera>();

		//variable interface ref
		_variableInterface = FindObjectOfType<VariableInterface>();
		
		//disable hallucination objects
		for (int i = 0; i < hallucObjects.Length; i++)
		{
			hallucObjects[i].SetActive(false);
		}

		//play on start 
		if (playOnStart)
		{
			PlayHallucination();
		}
	}

	public void PlayHallucination()
	{
		StartCoroutine(WaitToStartHallucination());
	}

	IEnumerator WaitToStartHallucination()
	{
		yield return new WaitForSeconds(1f);

		//start it 
		StartHallucination();
	}

	public void StartHallucination()
	{
		//pilot stuff
		if (pilot)
		{
			//disable controls 
			pilot.DisableControls();
			//freeze movements
			pilot.FreezeMovement();
		}
		//air player
		if (airPlayer)
		{
			airPlayer.Deactivate();
		}
		
		//deity freeze
		if (deityMan != null)
		{
			deityMan.FreezeDeities();
		}
		
		//enable hallucination objects
		for (int i = 0; i < hallucObjects.Length; i++)
		{
			hallucObjects[i].SetActive(true);
		}

		//play monologues 
		for (int i = 0; i < monoManagers.Length; i++)
		{
			monoManagers[i].WaitToSetNewMonologue(monoIndeces[i]);
		}
		
		//trigger events
		for (int i = 0; i < events.Length; i++)
		{
			events[i].Invoke();
		}
		
		//set up halluc camera with player parent 
		if (playerToParent)
		{
			//parent
			renderCam.transform.SetParent(playerToParent);
			//set local pos
			renderCam.transform.localPosition = camLocalPos;
			//enable camera movement
			camMover.GetRefs();
		}
		
		//set camera TODO may need more ways to orient camera at the beginning
		if(cameraPos)
			renderCam.transform.position = cameraPos.position;
		if(camLookAt)
			renderCam.transform.LookAt(camLookAt);

		//fade in
		imageFade.FadeIn();
		
		//set profiles
		camBehavior.profile = hallucProfile;
		hallucCamBehavior.profile = hallucProfile;

		hallucinating = true;
	}

	public void EndHallucination()
	{
		//if halluc camera was playable on something 
		if (playerToParent)
		{
			//parent
			renderCam.transform.SetParent(null);
			//disable camera movement
			camMover.canControl = false;
		}
		
		//disable hallucination objects
		for (int i = 0; i < hallucObjects.Length; i++)
		{
			hallucObjects[i].SetActive(false);
		}
		
		//trigger events
		for (int i = 0; i < endEvents.Length; i++)
		{
			endEvents[i].Invoke();
		}
		
		//pilot stuff
		if (pilot)
		{
			//enable controls
			pilot.EnableControls();
			//resume movements
			pilot.ResumeMovement();
		}
		//air player
		if (airPlayer)
		{
			airPlayer.Activate();
		}
		
		//deity resume
		if (deityMan != null)
		{
			deityMan.ResumeDeities();
		}
		
		//yarn variable to save?
		if (!string.IsNullOrEmpty(variableName))
		{
			_variableInterface.SetValue(variableName, yarnValue);	
		}
		
		//fade out
		imageFade.FadeOut();
		
		//set profiles
		camBehavior.profile = normalProfile;

		hallucinating = false;
	}

	public void WaitToEnd(float wait)
	{
		StartCoroutine(WaitToEndHalluc(wait));
	}

	IEnumerator WaitToEndHalluc(float time)
	{
		yield return new WaitForSeconds(time);
		
		EndHallucination();
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
	public float hallucinationLength = 5f;
	public GameObject[] hallucObjects;
	public bool hallucinating;

    [Header("Camera Transitions")]
	public Camera renderCam;
	private GroundCamera camMover;
	public Transform cameraPos;
	public Transform camLookAt;
	public Transform playerToParent;
	public Vector3 camLocalPos;
	public FadeUI imageFade;

	[Header("Monologues")] 
	public MonologueManager[] monoManagers;
	public int[] monoIndeces;
	
	void Start()
	{
		//pilot refs
		pilot = FindObjectOfType<ThePilot>();
		pilotCam = pilot.fpCam.GetComponent<PilotView>();
		
		//deity refs
		if(deityMan == null)
			deityMan = FindObjectOfType<DeityManager>();
		
		//cam refs
		mainCam = Camera.main;
		camBehavior = mainCam.GetComponent<PostProcessingBehaviour>();

		//check for cam movement
		if (renderCam)
			camMover = renderCam.GetComponent<GroundCamera>();
		
		//disable hallucination objects
		for (int i = 0; i < hallucObjects.Length; i++)
		{
			hallucObjects[i].SetActive(false);
		}
	}

	public void PlayHallucination()
	{
		StartCoroutine(WaitToStartHallucination());
	}

	IEnumerator WaitToStartHallucination()
	{
		//go to first person
		pilot.SetFPView();
		
		yield return new WaitForSeconds(3f);

		//start it 
		StartHallucination();

		//wait for halluc
		yield return new WaitForSeconds(hallucinationLength);
		
		//end it
		EndHallucination();
	}

	public void StartHallucination()
	{
		//disable controls 
		pilot.DisableControls();
		//freeze movements
		pilot.FreezeMovement();
		deityMan.FreezeDeities();

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

		//set camera
		if(cameraPos)
			renderCam.transform.position = cameraPos.position;
		if(camLookAt)
			renderCam.transform.LookAt(camLookAt);
		if (playerToParent)
		{
			//parent
			renderCam.transform.SetParent(playerToParent);
			//set local pos
			renderCam.transform.localPosition = camLocalPos;
			//enable camera movement
			camMover.GetRefs();
			//disable pilot cam 
			pilotCam.isActive = false;
		}

		//fade in
		imageFade.FadeIn();
		
		//set profiles
		camBehavior.profile = hallucProfile;

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
			//enable pilot cam 
			pilotCam.isActive = true;
		}
		
		//disable hallucination objects
		for (int i = 0; i < hallucObjects.Length; i++)
		{
			hallucObjects[i].SetActive(false);
		}
		
		//return to 3p view
		pilot.SetTPView();
		
		//enable controls
		pilot.EnableControls();
		//resume movements
		pilot.ResumeMovement();
		deityMan.ResumeDeities();
		
		//fade out
		imageFade.FadeOut();
		
		//set profiles
		camBehavior.profile = normalProfile;

		hallucinating = false;
	}

	private void OnDisable()
	{
		// if (hallucinating)
		// {
		// 	EndHallucination();
		// }
	}
}

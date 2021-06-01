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
	//render texture cam?
	//video?
	//post processing effect (vignette?)
	private Camera mainCam;
	private PostProcessingBehaviour camBehavior;
	public PostProcessingProfile hallucProfile;
	public PostProcessingProfile normalProfile;
	public float hallucinationLength = 5f;

	public Camera renderCam;
	public Transform cameraPos;
	public Transform camLookAt;
	public FadeUI imageFade;
	
	private void Start()
	{
		pilot = FindObjectOfType<ThePilot>();
		mainCam = Camera.main;
		camBehavior = mainCam.GetComponent<PostProcessingBehaviour>();
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
		
		//set camera
		if(cameraPos)
			renderCam.transform.position = cameraPos.position;
		if(camLookAt)
			renderCam.transform.LookAt(camLookAt);
		
		//fade in
		imageFade.FadeIn();
		
		//set profiles
		camBehavior.profile = hallucProfile;
	}

	public void EndHallucination()
	{
		//enable controls
		pilot.EnableControls();
		
		//fade out
		imageFade.FadeOut();
		
		//set profiles
		camBehavior.profile = normalProfile;
	}
}

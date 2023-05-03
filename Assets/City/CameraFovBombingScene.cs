using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DG.Tweening;

/// <summary>
/// Handles FOV behavior in the city bombing scene. 
/// </summary>
public class CameraFovBombingScene : MonoBehaviour
{
	public bool GetRotationValues = true;
	public float MinFOV = 100;
	public float MaxFOV = 130;
	public float updateRot;
	public float transitionFOV = 65f;
	public float transitionDuration = 2.5f;
	private float initialMin;
	private float initialMax;
	public int keepOriginalFovCount = 2;
	private int transitionCount = 0;
	
	public float UpdateT;
	public float roundedRotation;

	private Transform cam;
	CinemachineVirtualCamera vCam;
	CinemachineBrain brain;
	private GameObject lastCamObj;

	//public float normal;
	public float remapValue;

	void Start()
	{
		cam = Camera.main.transform;

		initialMin = MinFOV;
		initialMax = MaxFOV;
		brain = CinemachineCore.Instance.GetActiveBrain(0);
	}

	void Update()
	{
		if (!brain.IsBlending)
		{
			//update vcam and last cam obj if they don't match what's set in the brain
			if (brain.ActiveVirtualCamera.VirtualCameraGameObject != lastCamObj)
			{
				vCam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
				lastCamObj = brain.ActiveVirtualCamera.VirtualCameraGameObject;
				FadeToTransitionValues();
				transitionCount++;
			}
			
			roundedRotation = vCam.transform.rotation.eulerAngles.x;

			//roundedRotation = cam.rotation.eulerAngles.x + InitialDegree;

			UpdateT = ((1 - (Mathf.Cos(roundedRotation * Mathf.Deg2Rad))) / 2);
			if (roundedRotation < 200)
			{
				UpdateT = -UpdateT;
			}

			remapValue = Mathf.InverseLerp(-0.27f, 0.27f, UpdateT);

			updateRot = Mathf.Lerp(MinFOV, MaxFOV, remapValue);

			vCam.m_Lens.FieldOfView = updateRot;
		}
	}
	
	/// <summary>
	/// Normalize FOV values. 
	/// </summary>
	void FadeToTransitionValues()
    {
		DOTween.To(() => MinFOV, x => MinFOV = x, transitionFOV, transitionDuration);
		DOTween.To(() => MaxFOV, x => MaxFOV = x, transitionFOV, transitionDuration);

		if (transitionCount < keepOriginalFovCount)
		{
			StopAllCoroutines();
			StartCoroutine(WaitToResetMinMax());
		}
    }

	IEnumerator WaitToResetMinMax()
	{
		yield return new WaitForSeconds(transitionDuration);
		
		FadeToOriginalValues();
	}
	
	/// <summary>
	/// Returns to original FOV values 
	/// </summary>
	void FadeToOriginalValues()
	{
		DOTween.To(() => MinFOV, x => MinFOV = x, initialMin, transitionDuration);
		DOTween.To(() => MaxFOV, x => MaxFOV = x, initialMax, transitionDuration);
	}
}
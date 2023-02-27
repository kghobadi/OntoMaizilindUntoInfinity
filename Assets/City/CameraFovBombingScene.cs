using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraFovBombingScene : MonoBehaviour
{
	public bool GetRotationValues = true;
	public float MinFOV = 100;
	public float MaxFOV = 130;
	public float updateRot;

	public float InitialDegree;

	public float UpdateT;
	public float roundedRotation;

	private Transform cam;
	CinemachineVirtualCamera vCam;
	CinemachineBrain brain;

	//public float normal;
	public float remapValue;

	void Start()
	{
		cam = Camera.main.transform;

		brain = CinemachineCore.Instance.GetActiveBrain(0);
	}

	void Update()
	{
		vCam = brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
		roundedRotation = vCam.transform.rotation.eulerAngles.x;

		//roundedRotation = cam.rotation.eulerAngles.x + InitialDegree;

		UpdateT = ((1 - (Mathf.Cos(roundedRotation * Mathf.Deg2Rad))) / 2);
		if (roundedRotation < 200)
        {
			UpdateT = -UpdateT;
        }

		remapValue = Mathf.InverseLerp(-0.2f, 0.3f, UpdateT);

		updateRot = Mathf.Lerp(MinFOV, MaxFOV, remapValue);

		vCam.m_Lens.FieldOfView = updateRot;
	}
}
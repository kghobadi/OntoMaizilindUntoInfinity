// original by asteins
// adapted by @torahhorse
// http://wiki.unity3d.com/index.php/SmoothMouseLook

// Instructions:
// There should be one MouseLook script on the Player itself, and another on the camera
// player's MouseLook should use MouseX, camera's MouseLook should use MouseY

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseLook : MonoBehaviour
{
 
	public enum RotationAxes { MouseX = 1, MouseY = 2 }
	public RotationAxes axes = RotationAxes.MouseX;
	public bool invertY = false;
	
	public float sensitivityX = 2f;
	public float sensitivityY = 2F;
 
	public float minimumX = -360F;
	public float maximumX = 360F;
 
	public float minimumY = -80F;
	public float maximumY = 80F;

	private float originalY;
 
	float rotationX = 0F;
	float rotationY = 0F;
 
	private List<float> rotArrayX = new List<float>();
	float rotAverageX = 0F;	
 
	private List<float> rotArrayY = new List<float>();
	float rotAverageY = 0F;
 
	public float framesOfSmoothing = 5;
 
	Quaternion originalRotation;

	private bool FadeValues = false;
	private bool StartFade = false;
	public float FreezeSeconds = 3f; 
	private float timePassed = 0.0f;
	private float OriginalSensibilityX;
	private float OriginalSensibilityY;
	private float LerpSensX;
	private float LerpSensY;

	void Awake (){
		originalY = gameObject.transform.localEulerAngles.x;
		//originalY = GameObject.FindGameObjectWithTag ("MainCamera").transform.localEulerAngles.x;
		originalY = (originalY > 180) ? originalY - 360 : originalY;
		OriginalSensibilityX = sensitivityX;
		OriginalSensibilityY = sensitivityY;
	}
	
	void Start ()
	{	
		if (FreezeSeconds == 0) {
			FadeValues = false;
		} else {
			FadeValues = true;
			sensitivityX = 0;
			sensitivityY = 0;
		}
		if (GetComponent<Rigidbody>())
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}
		
		originalRotation = transform.localRotation;
	}
 
	void Update ()
	{
		if (FadeValues) {
			timePassed += Time.deltaTime;
			if (timePassed > 1 && sensitivityX == 0){
				StartFade = true;
				timePassed = 0;
			}
			if (StartFade){
				float LerpFloat = timePassed / FreezeSeconds;
				sensitivityX = Mathf.Lerp(0, (OriginalSensibilityX), LerpFloat);
				sensitivityY = Mathf.Lerp(0, (OriginalSensibilityY), LerpFloat);
				if (sensitivityX == OriginalSensibilityX) {
					sensitivityX = OriginalSensibilityX;
					sensitivityY = OriginalSensibilityY;
					FadeValues = false;
				}
			}
		}
		if (axes == RotationAxes.MouseX)
		{			
			rotAverageX = 0f;
 
			rotationX += Input.GetAxis("Mouse X") * sensitivityX * Time.timeScale;

			// STRINGA AGGIUNTA DA ME in caso di camera bloccata entro un certo angolo
			if (minimumX != -360f && maximumX != 360f){
				rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);
			}
 
			rotArrayX.Add(rotationX);
 
			if (rotArrayX.Count >= framesOfSmoothing)
			{
				rotArrayX.RemoveAt(0);
			}
			for(int i = 0; i < rotArrayX.Count; i++)
			{
				rotAverageX += rotArrayX[i];
			}
			rotAverageX /= rotArrayX.Count;

			if (minimumX == -360f && maximumX == 360f){
				rotAverageX = ClampAngle(rotAverageX, minimumX, maximumX);
			}
 
			Quaternion xQuaternion = Quaternion.AngleAxis (rotAverageX, Vector3.up);
			transform.localRotation = originalRotation * xQuaternion;
		}
		else
		{			
			rotAverageY = 0f;
 
 			float invertFlag = 1f;
 			if( invertY )
 			{
 				invertFlag = -1f;
 			}
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY * invertFlag * Time.timeScale;
			
			rotationY = Mathf.Clamp(rotationY, (minimumY+originalY), (maximumY+originalY));
 	
			rotArrayY.Add(rotationY);
 
			if (rotArrayY.Count >= framesOfSmoothing)
			{
				rotArrayY.RemoveAt(0);
			}
			for(int j = 0; j < rotArrayY.Count; j++)
			{
				rotAverageY += rotArrayY[j];
			}
			rotAverageY /= rotArrayY.Count;
 
			Quaternion yQuaternion = Quaternion.AngleAxis (rotAverageY, Vector3.left);
			transform.localRotation = originalRotation * yQuaternion;
		}
	}
	
	public void SetSensitivity(float s)
	{
		if (!FadeValues) {
			sensitivityX = OriginalSensibilityX * s;
			sensitivityY = OriginalSensibilityY * s;
			//print ("sensitivityX =" + sensitivityX);
		}
	}

	public void ResetSensitivity(float passedFloat)
	{
		if (!FadeValues) {
			OriginalSensibilityX = passedFloat;
			OriginalSensibilityY = passedFloat;
			sensitivityX = OriginalSensibilityX;
			sensitivityY = OriginalSensibilityY;
		}
	}

	public void FadeSensitivity(float newVal)
    {
		StartCoroutine(FadeSensitivityTo(2, sensitivityX, newVal));
    }

	public void ClearArrays()
    {
		rotArrayX.Clear();
		rotArrayY.Clear();
	}


	private float t;
	private float UpdateLerp;
	IEnumerator FadeSensitivityTo(float seconds, float CurrentSensitivity, float NewSensitivity)
	{
		while (true)
		{
			t += Time.deltaTime;

			UpdateLerp = t / seconds;

			UpdateLerp = UpdateLerp * UpdateLerp;
			sensitivityX = Mathf.Lerp(CurrentSensitivity, NewSensitivity, UpdateLerp);
			sensitivityY = Mathf.Lerp(CurrentSensitivity, NewSensitivity, UpdateLerp);

			if (t >= seconds)
			{
				sensitivityX = NewSensitivity;
				sensitivityY = NewSensitivity;
				t = 0;
				yield break;
			}

			yield return null;
		}
	}

	public static float ClampAngle (float angle, float min, float max)
	{
		angle = angle % 360;
		if ((angle >= -360F) && (angle <= 360F)) {
			if (angle < -360F) {
				angle += 360F;
			}
			if (angle > 360F) {
				angle -= 360F;
			}			
		}
		return Mathf.Clamp (angle, min, max);
	}
}
using UnityEngine;
using System.Collections;

public class ConstantRotation : MonoBehaviour {

	void Start () {
	
	}
	public bool X;
	public bool Y;
	public bool Z;


	public float Xvalue;
	public float Yvalue;
	public float Zvalue;

	void Update () {
		if (Time.timeScale == 0) { return; }
		if (X) {
			transform.Rotate (Xvalue * Time.deltaTime, 0, 0); //rotates 50 degrees per second around z axis
		}
		if (Y) {
			transform.Rotate ( 0, Yvalue * Time.deltaTime, 0); //rotates 50 degrees per second around z axis
		}
		if (Z) {
			transform.Rotate (0, 0, Zvalue * Time.deltaTime); //rotates 50 degrees per second around z axis
		}
	}
}

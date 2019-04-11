using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour {
    CharacterController charBody;
    public float currentSpeed, rotationSpeed;
    Vector3 movement;


	void Awake () {
        charBody = GetComponent<CharacterController>();

        currentSpeed = Random.Range(10, 25);

        rotationSpeed = Random.Range(-30, 30);
	}
	
	void Update () {
        movement = new Vector3(0, 0, currentSpeed);
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        movement = transform.rotation * movement;

        charBody.Move(movement * Time.deltaTime);

        charBody.Move(new Vector3(0, -0.5f, 0));
    }

    void SetMove()
    {

    }
}

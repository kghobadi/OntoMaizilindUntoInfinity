using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Citizen : MonoBehaviour {
    CharacterController charBody;
    public float currentSpeed, rotationSpeed;
    Vector3 movement;
    [HideInInspector]
    public CitizenAudio citizenSounds;

    public SpriteRenderer face;
    public Sprite normalFace, screaming;

	void Awake () {
        charBody = GetComponent<CharacterController>();
        citizenSounds = GetComponent<CitizenAudio>();
        SetSpeeds();
	}
	
	void Update () {
        Move();

        //swaps face sprite for screaming
        if (citizenSounds.myAudioSource.isPlaying)
        {
            face.sprite = screaming;
        }
        else
        {
            face.sprite = normalFace;
        }
    }

    private void Move()
    {
        movement = new Vector3(0, 0, currentSpeed);
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        movement = transform.rotation * movement;

        charBody.Move(movement * Time.deltaTime);

        charBody.Move(new Vector3(0, -0.5f, 0));
    }

    void SetMove()
    {

    }

    void SetSpeeds()
    {
        currentSpeed = Random.Range(10, 25);

        rotationSpeed = Random.Range(-30, 30);
    }
}

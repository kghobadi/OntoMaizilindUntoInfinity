using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class FirstPersonControllerVR : MonoBehaviour
{
    //timers and values for speed
    public float currentSpeed, walkSpeed, sprintSpeed;
    public float scrollSpeed = 2.0f;
    float sprintTimer = 0;
    public float sprintTimerMax = 1;

    float footStepTimer = 0;
    public float footStepTimerTotal = 0.5f;

    CharacterController player;
    GroundCamera mouseLook;
    Vector3 movement;

    //for footstep sounds
    public AudioClip[] currentFootsteps/*, indoorFootsteps, gardenFootsteps, pathFootsteps*/;
    AudioSource playerAudSource;

    public bool moving;

    Vector3 lastPosition;

    void Start()
    {
        player = GetComponent<CharacterController>();
        playerAudSource = GetComponent<AudioSource>();
        mouseLook = GetComponentInChildren<GroundCamera>();

    }

    void Update()
    {
        OVRInput.Update();

        //when hold mouse 1, you begin to move in that direction
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            Debug.Log("move forward");
            moving = true;

            movement = new Vector3(0, 0, currentSpeed);

            SprintSpeed();
        }
        //move backwards
        else if (OVRInput.Get(OVRInput.Button.One))
        {
            Debug.Log("move backward");
            moving = true;

            movement = new Vector3(0, 0, -currentSpeed);

            SprintSpeed();
        }
        
        //when not moving
        else
        {
            moving = false;
            movement = Vector3.zero;
            currentSpeed = walkSpeed;

            Debug.Log("not moving");
        }

        movement = transform.GetChild(0).rotation * movement;
        player.Move(movement * Time.deltaTime);

        player.Move(new Vector3(0, -0.5f, 0));
    }

    //increases move speed while player is moving over time
    public void SprintSpeed()
    {
        //increment and play footstep sounds
        footStepTimer -= Time.deltaTime;
        if (footStepTimer < 0)
        {
            PlayFootStepAudio();
            footStepTimer = footStepTimerTotal;
        }

        sprintTimer += Time.deltaTime;
        //while speed is less than sprint, autoAdd
        if (sprintTimer > sprintTimerMax && currentSpeed < sprintSpeed)
        {
            currentSpeed += Time.deltaTime;
        }
    }

    private void PlayFootStepAudio()
    {
        int n = Random.Range(1, currentFootsteps.Length);
        playerAudSource.clip = currentFootsteps[n];
        playerAudSource.PlayOneShot(playerAudSource.clip, 1f);
        // move picked sound to index 0 so it's not picked next time
        currentFootsteps[n] = currentFootsteps[0];
        currentFootsteps[0] = playerAudSource.clip;
    }

    

}

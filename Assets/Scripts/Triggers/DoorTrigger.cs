﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : AudioHandler {

    Animator doorAnimator;
    public bool locked;

    [Header("Sounds")]
    public AudioClip doorClose;
    public AudioClip doorOpen;

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    public void EnableDoor()
    {
        locked = false;
    }

    public void DisableDoor()
    {
        locked = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!locked)
        {
            if (other.gameObject.CompareTag("Human") || other.gameObject.CompareTag("Player"))
            {
                OpenDoor();
            }
        }
    }

    public void OpenDoor()
    {
        if (doorAnimator.GetBool("opened") == false)
        {
            doorAnimator.SetBool("opened", true);

            PlaySoundRandomPitch(doorOpen, 1f);

            //Debug.Log("person entered " + gameObject.name);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Human") || other.gameObject.CompareTag("Player"))
        {
            CloseDoor();
        }
    }

    public void CloseDoor()
    {
        if (doorAnimator.GetBool("opened") == true)
        {
            doorAnimator.SetBool("opened", false);

            PlaySoundRandomPitch(doorClose, 1f);

            //Debug.Log("person exited " + gameObject.name);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : AudioHandler {

    Animator doorAnimator;

    [Header("Sounds")]
    public AudioClip doorClose;
    public AudioClip doorOpen;

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Human")
        {
            Debug.Log("person entered " + gameObject.name);
            if(doorAnimator.GetBool("opened") == false)
            {
                doorAnimator.SetBool("opened", true);

                PlaySoundRandomPitch(doorOpen, 1f);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Human")
        {
            if (doorAnimator.GetBool("opened") == true)
            {
                doorAnimator.SetBool("opened", false);

                PlaySoundRandomPitch(doorClose, 1f);
            }
            Debug.Log("person exited " + gameObject.name);
        }
    }
}

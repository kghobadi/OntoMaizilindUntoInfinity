using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : AudioHandler {

    Animator doorAnimator;
    private BoxCollider doorCollider;
    
    [Tooltip("Check this and set the specific game object if there is only one very specific object which can trigger this.")]
    public bool specificObject;
    [Tooltip("Specific object this trigger is waiting for.")]
    public GameObject specificObj;
   
    public bool locked;

    [Header("Sounds")]
    public AudioClip doorClose;
    public AudioClip doorOpen;

    void Start()
    {
        doorAnimator = GetComponent<Animator>();
        doorCollider = GetComponent<BoxCollider>();
    }

    public void EnableDoor()
    {
        locked = false;
        //doorCollider.isTrigger = true; //triggerable
    }

    public void DisableDoor()
    {
        locked = true;
        //doorCollider.isTrigger = false;
    }

    public void SetSpecificObj(GameObject obj)
    {
        specificObj = obj;
        specificObject = true;
    }

    public void DisableSpecificObj()
    {
        specificObject = false;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!locked)
        {
            //check for a specific obj to trigger this. 
            if (specificObject)
            {
                if (other.gameObject == specificObj)
                {
                    OpenDoor();
                }
            }
            else if (other.gameObject.CompareTag("Human") || other.gameObject.CompareTag("Player"))
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

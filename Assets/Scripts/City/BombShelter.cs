using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombShelter : MonoBehaviour {

    CameraSwitcher camSwitcher;

    private void Awake()
    {
        camSwitcher = FindObjectOfType<CameraSwitcher>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Human")
        {
            EnterShelter(other.gameObject.GetComponent<CamObject>());
        }
    }

    void EnterShelter(CamObject person)
    {
        if(person == camSwitcher.currentCamObj)
        {
            camSwitcher.SwitchCam(true, -1);
        }

        person.gameObject.SetActive(false);
        camSwitcher.cameraObjects.Remove(person);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivationTrigger : MonoBehaviour {

    public bool hasTriggered;

    public GameObject[] objectsToActivate;
    public GameObject[] objectsToDeactivate;

    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered)
        {
            if (other.gameObject.tag == "Human" || other.gameObject.tag == "Player")
            {
                SetTrigger();

                Debug.Log(other.gameObject.name + " triggered " + gameObject.name);
            }
        }
        
    }

    void SetTrigger()
    {
        //activate stuff
        for(int i = 0; i < objectsToActivate.Length; i++)
        {
            objectsToActivate[i].SetActive(true);
        }

        //deactivate stuff
        for (int i = 0; i < objectsToDeactivate.Length; i++)
        {
            objectsToDeactivate[i].SetActive(false);
        }

        hasTriggered = true;
    }
}

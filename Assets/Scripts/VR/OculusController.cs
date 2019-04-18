using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//controls the motion sensor hand
public class OculusController : MonoBehaviour {
    public GameObject go;

	void Start () {
        go = new GameObject();
	}

    void Update()
    {
        OVRInput.Update();
        //set rotation to remote rotation
        transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTrackedRemote);

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit.collider != null)
            {
                if (go != hit.collider.gameObject)
                {
                    go = hit.transform.gameObject;
                    //go.SendMessage("OnVREnter");
                    Debug.Log("Raycasting at " + go.name);
                }
            }
        }
        else
        {
            if (go != null)
            {
                //go.SendMessage("OnVRExit");
                go = null;
            }
        }

        if (OVRInput.Get(OVRInput.Button.Two))
        {
            Debug.Log("vr input");
        }
    }
}

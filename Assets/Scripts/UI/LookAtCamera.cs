using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    Transform cammy;
    public bool looksAtMainCam = true;
    public Transform otherCam;

    private void Start()
    {
        if (looksAtMainCam)
        {
            cammy = Camera.main.transform;
        }
        else
        {
            cammy = otherCam;
        }
    }
    
    void Update () {
        transform.LookAt(new Vector3(cammy.position.x, transform.position.y, cammy.position.z));
	}
}

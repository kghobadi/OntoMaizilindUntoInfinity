using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour {

    public List<GameObject> cameraObjects = new List<GameObject>();
    public int currentCam = 0;

	void Start () {
		
	}
	
	void Update () {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SwitchCam(false);
        }	
	}

    public void SwitchCam(bool upOrDown)
    {
        cameraObjects[currentCam].SetActive(false);
        
        //count up
        if (upOrDown)
        {
            if (currentCam < cameraObjects.Count - 1)
            {
                currentCam++;
            }
            else
            {
                currentCam = 0;
            }
        }
        //count down
        else
        {
            if (currentCam > 0)
            {
                currentCam--;
            }
            else
            {
                currentCam = cameraObjects.Count - 1;
            }
        }

        cameraObjects[currentCam].SetActive(true);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour {
    //camera objects list, current obj, and int to count them
    public List<GameObject> cameraObjects = new List<GameObject>();
    GameObject currentCamObj;
    public int currentCam = 0;

	void Start () {

        //collect all humans and add them to cameraObjects list if they are not in it already
        GameObject[] humans = GameObject.FindGameObjectsWithTag("Human");

        //go through and add them
        for(int i = 0; i < humans.Length; i++)
        {
            if (!cameraObjects.Contains(humans[i]))
            {
                cameraObjects.Add(humans[i]);
            }
        }
        
        //loop through the cam objects list and set start settings for objects
        for (int i = 1; i < cameraObjects.Count; i++)
        {
            //do this to human cameras only
            if (cameraObjects[i].GetComponent<CamObject>().myCamType == CamObject.CamType.HUMAN)
            {
                //set the body's parent to the host game obj
                cameraObjects[i].GetComponent<CamObject>().myBody.transform.SetParent(cameraObjects[i].transform);
                //turn on that persons Citizen Ai
                cameraObjects[i].GetComponent<Citizen>().enabled = true;
                //turn off that persons FPC
                cameraObjects[i].GetComponent<FirstPersonController>().enabled = false;
                //turn off the person's camera
                cameraObjects[i].GetComponent<CamObject>().camObj.enabled = false;
                cameraObjects[i].GetComponent<CamObject>().camObj.GetComponent<AudioListener>().enabled = false;
            }
        }

        //set current cam obj at start
        currentCamObj = cameraObjects[currentCam];
	}
	
	void Update () {

        //switch through cam objects down
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SwitchCam(false, -1);
        }
        //switch through cam objects up
        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            SwitchCam(true, -1);
        }

        //directly switch to planes
        if (Input.GetKeyDown(KeyCode.Alpha0) && currentCam != 0)
        {
            SwitchCam(false, 0);
        }
	}

    public void SwitchCam(bool upOrDown, int num)
    {
        //deal with current cam object
        if(currentCamObj.GetComponent<CamObject>().myCamType == CamObject.CamType.HUMAN)
        {
            //set the body's parent to the host game obj
            currentCamObj.GetComponent<CamObject>().myBody.transform.SetParent(currentCamObj.transform);
            //turn on that persons Citizen Ai
            currentCamObj.GetComponent<Citizen>().enabled = true;
            //turn off that persons FPC
            currentCamObj.GetComponent<FirstPersonController>().enabled = false;
            //turn off the person's camera
            currentCamObj.GetComponent<CamObject>().camObj.enabled = false;
            currentCamObj.GetComponent<CamObject>().camObj.GetComponent<AudioListener>().enabled = false;
        }
        else
        {
            currentCamObj.SetActive(false);
        }

        //increment currentCam

        //use the passed int
        if (num >= 0)
        {
            currentCam = num;
        }
        //count up or down based on bool
        else
        {
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
                    if(currentCam < cameraObjects.Count - 1)
                    {
                        currentCam--;
                    }
                    else
                    {
                        currentCam = cameraObjects.Count - 2;
                    }
                }
                else
                {
                    currentCam = cameraObjects.Count - 1;
                }
            }
        }
        
        //turn on new cam obj
        if (cameraObjects[currentCam].GetComponent<CamObject>().myCamType == CamObject.CamType.HUMAN)
        {
            //set the body's parent to its camera
            cameraObjects[currentCam].GetComponent<CamObject>().myBody.transform.SetParent(cameraObjects[currentCam].GetComponent<CamObject>().camObj.transform);
            //turn off that persons Citizen Ai
            cameraObjects[currentCam].GetComponent<Citizen>().enabled = false;
            //turn on that persons FPC
            cameraObjects[currentCam].GetComponent<FirstPersonController>().enabled = true;
            //turn on the person's camera
            cameraObjects[currentCam].GetComponent<CamObject>().camObj.enabled = true;
            cameraObjects[currentCam].GetComponent<CamObject>().camObj.GetComponent<AudioListener>().enabled = true;
        }
        else
        {
            cameraObjects[currentCam].SetActive(true);
        }

        //reset current cam obj
        currentCamObj = cameraObjects[currentCam];
    }
}

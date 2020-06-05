using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class CameraSwitcher : MonoBehaviour {
    //camera objects list, current obj, and int to count them
    public List<CamObject> cameraObjects = new List<CamObject>();
    CamObject currentCamObj;
    public int currentCam = 0;
    public GameObject citizensParent;
    public FadeUI shiftPress;
    public bool canShift;
    public GameObject radioRoom;

    [Header("Transition")]
    public int transitionAmount = 3;
    AdvanceScene advance;

    void Awake()
    {
        //find all CamObjects in scene 
        CamObject[] cams = FindObjectsOfType<CamObject>();
        for(int i = 0; i < cams.Length; i++)
        {
            cameraObjects.Add(cams[i]);
        }

        //get advance scene comp
        advance = FindObjectOfType<AdvanceScene>();
        if(advance == null)
        {
            advance = gameObject.AddComponent<AdvanceScene>();
        }
    }

    void Start ()
    {
        //loop through the cam objects list and set start settings for objects
        for (int i = 1; i < cameraObjects.Count; i++)
        {
            DisableCamObj(cameraObjects[i]);
        }

        //turn off citizens for now
        citizensParent.SetActive(false);

        //set current cam obj at start
        currentCamObj = cameraObjects[currentCam];

        //cant shift yet
        canShift = false;
	}
	
	void Update ()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //resets current cam when people are destroyed
        if (currentCam > cameraObjects.Count - 1)
            currentCam = cameraObjects.IndexOf(currentCamObj);

        //only while shift ability active 
        if (canShift)
        {
            //switch through cam objects down
            if (Input.GetKeyDown(KeyCode.LeftShift) || inputDevice.DPadLeft.WasPressed || inputDevice.DPadDown.WasPressed)
            {
                SwitchCam(false, -1);
            }
            //switch through cam objects up
            if (Input.GetKeyDown(KeyCode.RightShift) || inputDevice.DPadRight.WasPressed || inputDevice.DPadUp.WasPressed)
            {
                SwitchCam(true, -1);
            }

            //directly switch to planes
            if (Input.GetKeyDown(KeyCode.Alpha0) && currentCam != 0)
            {
                SwitchCam(false, 0);
            }
        }

        //when there is all but one camera left, advance scene 
        if(cameraObjects.Count <= transitionAmount)
        {
            advance.LoadNextScene();
        }
	}

    public void SwitchCam(bool upOrDown, int num)
    {
        DisableCamObj(cameraObjects[currentCam]);

        //fade out shift press UI
        if (shiftPress.gameObject.activeSelf)
            shiftPress.FadeOut();
        //disable radio room 
        if (radioRoom.activeSelf)
            radioRoom.SetActive(false);

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

        EnableCamObj(cameraObjects[currentCam]);
    }

    //enables a camObj as current cam obj
    void EnableCamObj(CamObject cam)
    {
        //turn on new cam obj
        if (cam.myCamType == CamObject.CamType.HUMAN)
        {
            //set the body's parent to its camera
            cam.myBody.transform.SetParent(cam.camObj.transform);
            //enable ground cam script
            cam.camObj.GetComponent<GroundCamera>().enabled = true;
            //turn off that persons Citizen Ai
            cam.GetComponent<Citizen>().enabled = false;
            //turn on that persons FPC
            cam.GetComponent<FirstPersonController>().enabled = true;
            //turn on the person's camera
            cam.camObj.enabled = true;
            cam.camObj.GetComponent<AudioListener>().enabled = true;
        }
        else
        {
            cam.gameObject.SetActive(true);
        }

        //reset current cam obj
        currentCamObj = cameraObjects[currentCam];
    }

    //disables a cam obj
    void DisableCamObj(CamObject cam)
    {
        //deal with current cam object
        if (cam.myCamType == CamObject.CamType.HUMAN)
        {
            //set the body's parent to the host game obj
            cam.myBody.transform.SetParent(cam.transform);
            //disable ground cam script
            cam.camObj.GetComponent<GroundCamera>().enabled = false;
            //turn on that persons Citizen Ai
            cam.GetComponent<Citizen>().enabled = true;
            //turn off that persons FPC
            cam.GetComponent<FirstPersonController>().enabled = false;
            //turn off the person's camera
            cam.camObj.enabled = false;
            cam.camObj.GetComponent<AudioListener>().enabled = false;
        }
        else
        {
            cam.gameObject.SetActive(false);
        }
    }
}

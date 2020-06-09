using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using Cameras;
using UnityEngine.AI;
using NPC;

public class CameraSwitcher : MonoBehaviour {
    CameraManager camManager;

    //camera objects list, current obj, and int to count them
    public List<CamObject> cameraObjects = new List<CamObject>();
    [HideInInspector] public CamObject currentCamObj;
    public int currentCam = 0;
    public GameObject citizensParent;
    public FadeUI shiftPress;
    public bool canShift;
    public bool debug;
    public GameObject radioRoom;

    [Header("Transition")]
    public int transitionAmount = 3;
    AdvanceScene advance;

    void Awake()
    {
        //camera manager ref 
        camManager = FindObjectOfType<CameraManager>();

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
        
        //set current cam obj at start
        currentCamObj = cameraObjects[currentCam];

        if (!debug)
        {
            //turn off citizens for now
            citizensParent.SetActive(false);

            //cant shift yet
            canShift = false;
        }
        else
        {
            canShift = true;
        }
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
        if (shiftPress)
        {
            if (shiftPress.gameObject.activeSelf)
                shiftPress.FadeOut();
        }

        //disable radio room 
        if (radioRoom)
        {
            if (radioRoom.activeSelf)
                radioRoom.SetActive(false);
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
            //set new cam
            camManager.Set(cam.camObj);
            //enable ground cam script
            cam.camObj.GetComponent<GroundCamera>().enabled = true;
            //turn off that persons Citizen Ai
            cam.GetComponent<NavMeshAgent>().enabled = false;
            //turn off that persons Citizen Ai
            cam.GetComponent<Movement>().AIenabled = false;
            //turn on that persons FPC
            cam.GetComponent<FirstPersonController>().enabled = true;
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
            cam.GetComponent<NavMeshAgent>().enabled = true;
            //turn off that persons Citizen Ai
            cam.GetComponent<Movement>().AIenabled = true;
            //turn off that persons FPC
            cam.GetComponent<FirstPersonController>().enabled = false;
        }
        else
        {
            cam.gameObject.SetActive(false);
        }
    }
}

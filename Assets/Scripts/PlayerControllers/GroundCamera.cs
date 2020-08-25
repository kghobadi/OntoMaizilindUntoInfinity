using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class GroundCamera : MonoBehaviour
{
    PauseMenu pauseMenu;
    CameraSwitcher camSwitcher;
    Transform mainCam;
    GameObject character;
    Transform player;
    FirstPersonController fpc;
    Vector2 mouseLook;
    Vector2 smoothV;

    float hRot, vRot;
    public float sensitivityX = 1f;
    public float sensitivityY = 1f;
    public bool invertX, invertY;

    [Header("Clamp Rotation")]
    public bool clamps;
    public float minX, maxX;
    public float minY, maxY;
    //public float smoothing = 2.0f;

    void Awake()
    {
        character = transform.parent.gameObject;
        player = transform.parent;
        fpc = player.GetComponent<FirstPersonController>();
        mainCam = Camera.main.transform;
        pauseMenu = FindObjectOfType<PauseMenu>();
        camSwitcher = FindObjectOfType<CameraSwitcher>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (pauseMenu)
        {
            if (pauseMenu.paused == false)
                CameraRotation();
        }
        else
        {
            CameraRotation();
        }
    }

    //only camera rotation 
    void CameraRotation()
    { 
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        if (clamps)
        {
            //controller 
            if (inputDevice.DeviceClass == InputDeviceClass.Controller)
            {
                hRot += inputDevice.RightStickX * sensitivityX;
                vRot += inputDevice.RightStickY * sensitivityY;
            }
            //mouse
            else
            {
                hRot += Input.GetAxis("Mouse X") * sensitivityX;
                vRot += Input.GetAxis("Mouse Y") * sensitivityY;
            }
            //only clamp if both values are not set to 0
            if (minY != 0 && maxY != 0)
            {
                //clamp Y - horizontal
                hRot = Mathf.Clamp(hRot, minY, maxY);
            }
            //only clamp if both values are not set to 0
            if(minX != 0 && maxX != 0)
            {
                //clamp X - vertical
                vRot = Mathf.Clamp(vRot, minX, maxX);
            }
            
            //horizontal parent axis  - Y
            transform.parent.rotation = Quaternion.Euler(0f, hRot, 0f);
            //vertical camera axis - X
            transform.localRotation = Quaternion.Euler(-vRot, 0f, 0f);
        }
        //free rotations without clamp
        else
        {
            //controller 
            if (inputDevice.DeviceClass == InputDeviceClass.Controller)
            {
                hRot = sensitivityX * inputDevice.RightStickX;
                vRot = sensitivityY * inputDevice.RightStickY;
            }
            //mouse
            else
            {
                hRot = sensitivityX * Input.GetAxis("Mouse X");
                vRot = sensitivityY * Input.GetAxis("Mouse Y");
            }

            //neg value 
            if (invertX)
                hRot *= -1f;
            //neg value 
            if (invertY)
                vRot *= -1f;

            //Rotates Player on "X" Axis Acording to Mouse Input
            transform.parent.Rotate(0, hRot, 0);
            //Rotates Player on "Y" Axis Acording to Mouse Input
            transform.Rotate(vRot, 0, 0);
        }
    }
    
    void ShiftCheck()
    {
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && camSwitcher.canShift)
        {
            //only switch if there is a current cam obj 
            if (currentCamObj != null)
            {
                int index = camSwitcher.cameraObjects.IndexOf(currentCamObj);
                camSwitcher.SwitchCam(true, index);
                currentCamObj.shiftUI.FadeOut();
            }
        }
    }

    [Header("Gaze Settings")]
    public CamObject currentCamObj;
    public LayerMask shiftable;
    
    //sends a raycast forward
    void RaycastForward()
    {
        //forward
        Vector3 dir = mainCam.transform.TransformDirection(Vector3.forward);

        RaycastHit hit; 
        //raycast   
        if (Physics.Raycast(mainCam.transform.position, dir, out hit, camSwitcher.shiftDistLimit, shiftable))
        {
            //check if it has gaze tp
            if (hit.transform.gameObject.GetComponent<CamObject>())
            {
                //get comp
                CamObject camObj = hit.transform.gameObject.GetComponent<CamObject>();
                //check that it is not already the currentTP
                if (camObj != currentCamObj)
                {
                    //already have a currentCamObj
                    if (currentCamObj != null)
                    {
                        currentCamObj.shiftUI.FadeOut();
                        currentCamObj = camObj;
                        currentCamObj.shiftUI.FadeIn();
                    }
                    //no current TP, just set it to this one 
                    else
                    {
                        currentCamObj = camObj;
                        currentCamObj.shiftUI.FadeIn();
                    }
                }
            }
        }
    }
}




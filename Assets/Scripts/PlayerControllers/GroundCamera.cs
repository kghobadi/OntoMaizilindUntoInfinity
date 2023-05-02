using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using InControl;
using UnityEngine.SceneManagement;

public class GroundCamera : MonoBehaviour
{
    private CinemachineVirtualCamera cvc;
    PauseMenu pauseMenu;
    CameraSwitcher camSwitcher; 
    ObjectViewer _objectViewer;
    Transform mainCam;
    GameObject character;
    Transform player;
    FirstPersonController fpc;
    Vector2 mouseLook;
    Vector2 smoothV;

    float hRot, vRot;
    public float sensitivityX = 1f;
    public float sensitivityY = 1f;
    public float controllerSensitivityX = 3f;
    public float controllerSensitivityY = 3f;
    public bool invertX, invertY;
    public bool canControl = true;

    [Header("Clamp Rotation")]
    public bool clamps;
    public float minX, maxX;
    public float minY, maxY;

    void Awake()
    {
        if (transform.parent)
        {
            GetRefs();
            
            //set starting values for rotations. 
            if (clamps)
            {
                hRot = transform.parent.eulerAngles.y;
                vRot = transform.eulerAngles.x;
            }
        }
    }

    private void OnEnable()
    {
        if (SceneManager.GetActiveScene().name == "Bombing of a City")
        {
            if (cvc == null)
            {
                cvc = GetComponent<CinemachineVirtualCamera>();
            }
            cvc.enabled = true;
        }
    }

    private void OnDisable()
    {
        if (SceneManager.GetActiveScene().name == "Bombing of a City")
        {
            cvc.enabled = false; 
        }
    }

    public void GetRefs()
    {
        character = transform.parent.gameObject;
        player = transform.parent;
        fpc = player.GetComponent<FirstPersonController>();
        cvc = GetComponent<CinemachineVirtualCamera>();
        mainCam = Camera.main.transform;
        pauseMenu = FindObjectOfType<PauseMenu>();
        pauseMenu.toggledPause.AddListener(CheckCanControl);
        camSwitcher = FindObjectOfType<CameraSwitcher>();
        _objectViewer = FindObjectOfType<ObjectViewer>();
        if (currentCamObj == null)
        {
            currentCamObj = GetComponentInParent<CamObject>();
        }

        Cursor.lockState = CursorLockMode.Locked;
        canControl = true;
    }
    
    void CheckCanControl()
    {
        if (pauseMenu.paused)
        {
            canControl = false;
        }
        else if(pauseMenu.paused == false )
        {
            if (_objectViewer)
            {
                if(_objectViewer.viewing == false)
                    canControl = true;
            }
            else
            {
                canControl = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (canControl)
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
                hRot += inputDevice.RightStickX * controllerSensitivityX;
                vRot += inputDevice.RightStickY * controllerSensitivityY;
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
                hRot = controllerSensitivityX * inputDevice.RightStickX;
                vRot = controllerSensitivityY * inputDevice.RightStickY;
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

    /// <summary>
    /// Public call for clamp bool.
    /// </summary>
    /// <param name="clamp"></param>
    public void SetClamp(bool clamp)
    {
        clamps = clamp;
    }
    
    void ShiftCheck()
    {
        if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && camSwitcher.canShift)
        {
            //only switch if there is a current cam obj 
            if (currentCamObj != null)
            {
                int index = camSwitcher.cameraObjects.IndexOf(currentCamObj);
                camSwitcher.SetCam(index);
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




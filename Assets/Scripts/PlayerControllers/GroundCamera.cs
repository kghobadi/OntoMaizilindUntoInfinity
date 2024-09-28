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
    private float MouseSensitivity
    {
        get
        {
            float MouseSense = 1f;
            if (PlayerPrefs.HasKey("MouseSensitivity"))
            {
                MouseSense =  PlayerPrefs.GetFloat("MouseSensitivity");
            }
            else
            {
                PlayerPrefs.SetFloat("MouseSensitivity", MouseSense);
            }

            return MouseSense;
        }
    }
    private float ControllerSensitivity
    {
        get
        {
            float controllerSense = 3f;
            if (PlayerPrefs.HasKey("ControllerSensitivity"))
            {
                controllerSense =  PlayerPrefs.GetFloat("ControllerSensitivity");
            }
            else
            {
                PlayerPrefs.SetFloat("ControllerSensitivity", controllerSense);
            }

            return controllerSense;
        }
    }
    
    public bool invertX, invertY;
    public bool canControl = true;

    [Header("Clamp Rotation")]
    public bool clamps;
    public float minX, maxX;
    public float minY, maxY;

    private const string bombingScene = "2_Bombing of a City";

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
        if (SceneManager.GetActiveScene().name == bombingScene)
        {
            if (cvc == null)
            {
                cvc = currentCamObj.GetCinemachineCam();
            }
            cvc.enabled = true;
        }
    }

    private void OnDisable()
    {
        if (SceneManager.GetActiveScene().name == bombingScene)
        {
            cvc.enabled = false; 
        }
    }

    public void GetRefs()
    {
        if (currentCamObj)
        {
            character = currentCamObj.gameObject;
            player = currentCamObj.transform;
        }
        else
        {
            character = transform.parent.gameObject;
            player = transform.parent;
        }
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
        if(SceneManager.GetActiveScene().name != bombingScene)
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

    void Update()
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
                hRot += inputDevice.RightStickX * ControllerSensitivity;
                vRot += inputDevice.RightStickY * ControllerSensitivity;
            }
            //mouse
            else
            {
                hRot += Input.GetAxis("Mouse X") * MouseSensitivity;
                vRot += Input.GetAxis("Mouse Y") * MouseSensitivity;
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
            if(currentCamObj)
                player.rotation = Quaternion.Euler(0f, hRot, 0f);
            else 
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
                hRot = ControllerSensitivity * inputDevice.RightStickX;
                vRot = ControllerSensitivity * inputDevice.RightStickY;
            }
            //mouse
            else
            {
                hRot = MouseSensitivity * Input.GetAxis("Mouse X");
                vRot = MouseSensitivity * Input.GetAxis("Mouse Y");
            }

            //neg value 
            if (invertX)
                hRot *= -1f;
            //neg value 
            if (invertY)
                vRot *= -1f;
        }
    }

    private void FixedUpdate()
    {
        if (canControl)
        {
            Rotations();
        }
    }

    void Rotations()
    {
        //Rotates Player on "X" Axis Acording to Mouse Input
        if(currentCamObj)
            player.Rotate(0, hRot, 0);
        else 
            transform.parent.rotation = Quaternion.Euler(0, hRot, 0);
           
        //Rotates Player on "Y" Axis Acording to Mouse Input
        transform.Rotate(vRot, 0, 0);
    }

    /// <summary>
    /// Public call for clamp bool.
    /// </summary>
    /// <param name="clamp"></param>
    public void SetClamp(bool clamp)
    {
        clamps = clamp;
    }

    /// <summary>
    /// Can we control this?
    /// </summary>
    /// <param name="controls"></param>
    public void SetControls(bool controls)
    {
        canControl = controls;
    }

    /// <summary>
    /// Toggles the clamp.
    /// </summary>
    public void ToggleClamp()
    {
        clamps = !clamps;
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




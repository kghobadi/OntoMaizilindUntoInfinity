using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class GroundCamera : MonoBehaviour
{
    CameraSwitcher camSwitcher;
    Transform mainCam;
    GameObject character;
    Transform player;
    FirstPersonController fpc;
    Vector2 mouseLook;
    Vector2 smoothV;

    public float sensitivityX;
    public float sensitivityY;
    public float smoothing = 2.0f;

    void Awake()
    {
        character = transform.parent.gameObject;
        player = transform.parent;
        fpc = player.GetComponent<FirstPersonController>();
        mainCam = Camera.main.transform;
        camSwitcher = FindObjectOfType<CameraSwitcher>();
    }

    void Update()
    {
        CameraMovement();

        if (camSwitcher.canShift)
            RaycastForward();

        ShiftCheck();
    }

    void CameraMovement()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        Cursor.lockState = CursorLockMode.Locked;

        var newRotate = new Vector2(0, 0);

        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            newRotate = new Vector2(inputDevice.RightStickX, inputDevice.RightStickY);
        }
        //mouse
        else
        {
            newRotate = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        }

        newRotate = Vector2.Scale(newRotate, new Vector2(sensitivityX * smoothing, sensitivityY * smoothing));
        smoothV.x = Mathf.Lerp(smoothV.x, newRotate.x, 1f / smoothing);
        smoothV.y = Mathf.Lerp(smoothV.y, newRotate.y, 1f / smoothing);
        mouseLook += smoothV;

        mouseLook.y = Mathf.Clamp(mouseLook.y, -90f, 90f);

        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);
        character.transform.localRotation = Quaternion.AngleAxis(mouseLook.x, character.transform.up);
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




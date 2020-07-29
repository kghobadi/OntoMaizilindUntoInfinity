using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using InControl;

public class PilotView : MonoBehaviour
{
    Camera mainCam;

    [Header("FPS Camera Controls")]
    public bool isActive;
    float hRot, vRot;
    public float sensitivityX = 1f;
    public float sensitivityY = 1f;
    public bool invertX, invertY;

    void Awake()
    {
        mainCam = Camera.main;

        isActive = true;
    }

    void Update()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //for viewing with cam
        if (isActive)
        {
            CameraRotation();
        }
    }

    void CameraRotation()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

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

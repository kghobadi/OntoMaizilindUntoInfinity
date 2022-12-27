﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using InControl;

public class PilotView : MonoBehaviour
{
    PauseMenu pauseMenu;
    Camera mainCam;
    private InputDevice inputDevice;

    [Header("FPS Camera Controls")]
    public bool isActive;
    private bool activeAtStart;
    float hRot, vRot;
    public float sensitivityX = 1f;
    public float sensitivityY = 1f;
    public bool invertX, invertY;

    [Header("Clamp Rotation")]
    public bool clamps;
    public float minX, maxX;
    public float minY, maxY;

    void Awake()
    {
        mainCam = Camera.main;
        pauseMenu = FindObjectOfType<PauseMenu>();
        activeAtStart = isActive;
    }

    void Update()
    {
        //get input device 
        inputDevice = InputManager.ActiveDevice;

        //for viewing with cam
        if (isActive && pauseMenu.paused == false)
        {
            CameraRotation();
        }
    }

    void CameraRotation()
    {
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

            //clamp Y - horizontal
            hRot = Mathf.Clamp(hRot, minY, maxY);
            //clamp X - vertical
            vRot = Mathf.Clamp(vRot, minX, maxX);
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

    // private void OnDisable()
    // {
    //     isActive = activeAtStart;
    // }
}

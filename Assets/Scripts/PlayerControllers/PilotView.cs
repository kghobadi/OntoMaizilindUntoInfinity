using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using InControl;

public class PilotView : MonoBehaviour
{
    Camera mainCam;

    //for viewing
    Vector2 mouseLook;
    Vector2 smoothV;
    public float sensitivityX;
    public float sensitivityY;
    public float smoothing = 2.0f;
    public bool isActive;

    void Awake()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //for viewing with cam
        if (isActive)
        {
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
            transform.localRotation = Quaternion.AngleAxis(mouseLook.x, transform.up);
        }
    }
    
}

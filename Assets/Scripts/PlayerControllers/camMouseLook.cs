using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
using InControl;
using Cameras;

public class camMouseLook : MonoBehaviour
{
    //player and maincam
    Transform player;
    Camera mainCam;
    CharacterController astralBody;
    DebugTime timeDebug;

    //for viewing
    Vector2 mouseLook;
    Vector2 smoothV;
    
    [Header("Old FPS movement")]
    public bool isActive;
    float hRot, vRot;
    public float sensitivityX = 1f;
    public float sensitivityY = 1f;
    public bool invertX, invertY;

    [Header("Astral Body Movement")]
    public float moveSpeed = 10f;
    public float fovSpeed = 1f;
    public float speedOverTime = 0.05f;

    void Awake()
    {
        mainCam = Camera.main;
        astralBody = transform.parent.GetComponent<CharacterController>();
        timeDebug = FindObjectOfType<DebugTime>();

        isActive = true;
    }

    void Update()
    {
        if (isActive)
        {
            CameraRotation();

            WASDmovement();

            ClickMovement();

            FovControls();

            moveSpeed += (timeDebug.gameTime * Time.deltaTime) * speedOverTime;
        }
    }

    public void Activate()
    {
        isActive = true;
    }

    public void Deactivate()
    {
        isActive = false;
    }
    
    void WASDmovement()
    {
        //create empty force vector for this frame 
        Vector3 force = Vector3.zero;
        float horizontalMovement;
        float forwardMovement;

        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //controller 
        if (inputDevice.DeviceClass == InputDeviceClass.Controller)
        {
            // 3 axes 
            horizontalMovement = inputDevice.LeftStickX;
            forwardMovement = inputDevice.LeftStickY;
        }
        //mouse & keyboard
        else
        {
            // 3 axes 
            horizontalMovement = Input.GetAxis("Horizontal");
            forwardMovement = Input.GetAxis("Vertical");
        }

        //add forward force 
        force += transform.forward * forwardMovement;
        //add x force 
        force += transform.right * horizontalMovement;

        //actual move command 
        if (astralBody.enabled)
            astralBody.Move(force * moveSpeed);
    }

    void ClickMovement()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //left click to float camera forward through space
        if (Input.GetMouseButton(0) || inputDevice.DPadY > 0)
        {
            Vector3 forward = transform.forward * moveSpeed;
            astralBody.Move(forward);
        }
        //right click to float camera backward through space
        if (Input.GetMouseButton(1) || inputDevice.DPadY < 0)
        {
            Vector3 backward = -transform.forward * moveSpeed;
            astralBody.Move(backward);
        }
    }

    void FovControls()
    {
        //scroll mousewheel to adjust FOV
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            mainCam.fieldOfView += fovSpeed;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            mainCam.fieldOfView -= fovSpeed;
        }
    }

    //only camera rotation 
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

    private void OnDisable()
    {

    }
}

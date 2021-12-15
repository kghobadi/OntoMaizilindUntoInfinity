using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using InControl;

public class InteractRaycaster : NonInstantiatingSingleton<InteractRaycaster>
{
    protected override InteractRaycaster GetInstance () { return this; }
    private Camera mainCam;
    public float interactDistanceMax;
    public LayerMask interactiveLayer;

    public bool active = true;
    //let all interactives know I hit something. 
    public UnityEvent<GameObject> hitInteractiveObjectEvent;
    public UnityEvent hitNothingEvent;
    public UnityEvent interactInput;
    InputDevice inputDevice;
    private void Awake()
    {
        //get main cam
        mainCam = Camera.main;
    }

    private void Start()
    {
        StartCoroutine(RaycastToWorld());
    }

    IEnumerator RaycastToWorld()
    {
        active = true;
        while (active)
        {
            //get input device 
            inputDevice = InputManager.ActiveDevice;
            
            RaycastFromScreenToWorld();
            //input event check.
            if (inputDevice.Action1.WasPressed)
            {
                if (interactInput != null)
                {
                    interactInput.Invoke();
                }
            }
            yield return new WaitForEndOfFrame();
        }
    }
    
    //Moves 3d cursor in world space 
    public void RaycastFromScreenToWorld()
    {
        // Convert to world space
        Ray ray = mainCam.ScreenPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 dir = Vector3.forward;
        RaycastHit hit;

        // Find our ray's intersection through the selected layer
        if ( Physics.Raycast(ray, out hit, interactDistanceMax, interactiveLayer))
        {
            //send hit event 
            if (hitInteractiveObjectEvent != null)
            {
                hitInteractiveObjectEvent.Invoke(hit.transform.gameObject);
            }

            Debug.Log("Hit object: " + hit.transform.gameObject.name);
            //could set something to this position:
            //transform.position = hit.point +new Vector3(0f, heightOffset, 0f);
            //get pos in viewport
            //Vector3 viewPos = mainCam.WorldToViewportPoint(threeDCursor.transform.position);
        }
        //Didn't hit anything on the Interactive layer in distance.
        else
        {
            if (hitNothingEvent != null)
            {
                hitNothingEvent.Invoke();
            }
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}

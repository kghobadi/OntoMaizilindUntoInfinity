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
    private InputDevice inputDevice;
    private GameObject currentInteractObject;

    protected override void OnAwake()
    {
        base.OnAwake();
        
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
            
            //raycast
            RaycastFromScreenToWorld();
            
            //input event check.
            if (inputDevice.Action1.WasPressed || Input.GetMouseButtonDown(0))
            {
                EventManager.TriggerEvent("OnInteractInput", currentInteractObject);
            }
            
            //wait for end of frame
            yield return new WaitForEndOfFrame();
        }
    }
    
    //Moves 3d cursor in world space 
    public void RaycastFromScreenToWorld()
    {
        // Convert to world space
        Ray ray = mainCam.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, mainCam.nearClipPlane));
        Vector3 dir = Vector3.forward;
        RaycastHit hit;

        // Find our ray's intersection through the selected layer
        if ( Physics.Raycast(ray, out hit, interactDistanceMax, interactiveLayer))
        {
            //set current interact obj
            currentInteractObject = hit.transform.gameObject;
            //send hit event 
            EventManager.TriggerEvent("OnHitInteractiveObject", currentInteractObject);

            Debug.Log("Hit object: " + currentInteractObject.name);
            //could set something to this position:
            //transform.position = hit.point +new Vector3(0f, heightOffset, 0f);
            //get pos in viewport
            //Vector3 viewPos = mainCam.WorldToViewportPoint(threeDCursor.transform.position);
        }
        //Didn't hit anything on the Interactive layer in distance.
        else
        {
            EventManager.TriggerEvent("OnHitNothing", gameObject);
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}

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
    public LayerMask interactableLayer;
    public bool usesOtherCamera;
    public Transform currentPlayerOverride;

    public bool active = true;
    private InputDevice inputDevice;
    public GameObject currentInteractObject;
    private GameObject lastInteractObject;

    public bool enableOnStart = true;
    
    //delegate/events
    public delegate void OnHitInteractObj ();
    public static event OnHitInteractObj onHitInteractObj;
    public delegate void OnHitNothing ();
    public static event OnHitNothing onHitNothing;
    public delegate void OnInteractInput ();
    public static event OnInteractInput onInteractInput;

    protected override void OnAwake()
    {
        base.OnAwake();

        if (usesOtherCamera)
        {
            //get cam from this obj
            mainCam = GetComponent<Camera>();
        }
        else
        {
            //get main cam
            mainCam = Camera.main;
        }
    }

    private void Start()
    {
        if (enableOnStart)
        {
            ActivateRaycasts();
        }
    }

    private void Update()
    {
        if (active == false)
        {
            return;
        }
        
        //get input device 
        inputDevice = InputManager.ActiveDevice;
        
        //input event check.
        if (inputDevice.Action1.WasPressed || Input.GetMouseButtonDown(0))
        {
            onInteractInput();
        }
    }

    public void ActivateRaycasts()
    {
        StartCoroutine(RaycastToWorld());
    }

    IEnumerator RaycastToWorld()
    {
        active = true;
        
        while (active)
        {
            //raycast
            RaycastFromScreenToWorld();
            
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
        if ( Physics.Raycast(ray, out hit, interactDistanceMax, interactableLayer))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Interactable"))
            {
                //set current interact obj
                currentInteractObject = hit.transform.gameObject;
                //send hit event
                onHitInteractObj();
                //set last interact obj
                lastInteractObject = currentInteractObject;

                //Debug.Log("Hit object: " + currentInteractObject.name);
            }
            else
            {
                currentInteractObject = null;
                onHitNothing();
            }
        }
        //Didn't hit anything on the Interactive layer in distance.
        else
        {
            currentInteractObject = null;
            onHitNothing();
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}

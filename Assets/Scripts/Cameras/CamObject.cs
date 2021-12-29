using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cameras;
using Cinemachine;
using NPC;
using UnityEngine.AI;

//this scripts stores info about what kind of camera object we are switching too
//will allow me to access various kinds of movement scripts, camera, and body of obj
public class CamObject : MonoBehaviour 
{
    public CamType myCamType;
    public GameCamera camObj;
    public GameObject headset, myBody;
    public FadeUI shiftUI;
    private Controller myController;
    private Movement myMover;
    private FirstPersonController myFPS;
    private GroundCamera myGroundCam;
    private CinemachineVirtualCamera myVirtualCam;
    private NavMeshAgent myNMA;
    
    public enum CamType
    {
        HUMAN, BOMBER,
    }
    
    public Controller GetController()
    {
        if (myController == null)
        {
            myController = GetComponent<Controller>();
        }

        return myController;
    }

    public Movement GetMovement()
    {
        if (myMover == null)
        {
            myMover = GetComponent<Movement>();
        }

        return myMover;
    }
    
    public FirstPersonController GetFPS()
    {
        if (myFPS == null)
        {
            myFPS = GetComponent<FirstPersonController>();
        }

        return myFPS;
    }

    public CinemachineVirtualCamera GetCinemachineCam()
    {
        if (myVirtualCam == null)
        {
            myVirtualCam = camObj.GetComponent<CinemachineVirtualCamera>();
        }

        return myVirtualCam;
    }
    
    public GroundCamera GetGroundCam()
    {
        if (myGroundCam == null)
        {
            myGroundCam = camObj.GetComponent<GroundCamera>();
        }

        return myGroundCam;
    }
    
    
    public NavMeshAgent GetNMA()
    {
        if (myNMA == null)
        {
            myNMA = GetComponent<NavMeshAgent>();
        }

        return myNMA;
    }
}

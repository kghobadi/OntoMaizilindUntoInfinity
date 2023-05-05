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
    public enum CamType
    {
        HUMAN, BOMBER, MAINPLAYER,
    }
    //All Cam objs
    public CamType myCamType;
    public GameCamera camObj;
    private CinemachineVirtualCamera myVirtualCam;
    public FadeUI shiftUI;
    
    //Human/player
    public GameObject headset, myBody;
    private Controller myController;
    private Movement myMover;
    private FirstPersonController myFPS;
    private GroundCamera myGroundCam;
    private NavMeshAgent myNMA;
    
    //bomber only
    private camMouseLook _camMouseLook;
    private BombSquadron bombSquadron;
    
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
    
    public camMouseLook GetCamMouseLook()
    {
        if (_camMouseLook == null)
        {
            _camMouseLook = camObj.GetComponent<camMouseLook>();
        }

        return _camMouseLook;
    }
    
    public NavMeshAgent GetNMA()
    {
        if (myNMA == null)
        {
            myNMA = GetComponent<NavMeshAgent>();
        }

        return myNMA;
    }
    
    public BombSquadron BombSquadron
    {
        get
        {
            if (bombSquadron == null)
            {
                bombSquadron = GetComponentInParent<BombSquadron>();
            }
            return bombSquadron;
        }
        set => bombSquadron = value;
    }

    /// <summary>
    /// Makes a human pray. 
    /// </summary>
    public void Pray()
    {
        myController.Animation.SetAnimator("idle");
        //set to Prayer idle.
        myController.Animation.Animator.SetFloat("IdleType", 0.666667f);
        //disable FPS 
        myFPS.enabled = false;
        //set cam view -- currently we are seeing weird angles of the npc, want to move it a bit or change fov
        myVirtualCam.m_Lens.FieldOfView = 80f;
    }
}

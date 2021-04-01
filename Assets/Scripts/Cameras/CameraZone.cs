using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Cameras
{
    //This script allows you to trigger cam Manager to enable a specific camera while player is in a zone.
    //Useful for buildings, caves, etc. 
    public class CameraZone : MonoBehaviour
    {
        CameraManager camManager;
        CameraSwitcher camSwitcher;
        Transform currentPlayer;

        public bool active;
        public GameCamera desiredCam, lastCam, exitCam;

        [Tooltip("Player cannot sprint")]
        public bool limitPlayerMovement;

        void Awake()
        {
            camManager = FindObjectOfType<CameraManager>();
            camSwitcher = FindObjectOfType<CameraSwitcher>();
        }

        void OnTriggerEnter(Collider other)
        {
            currentPlayer = camSwitcher.currentPlayer.transform;

            if(other.gameObject == currentPlayer.gameObject && !active)
            {
                Activate();
            }
        }
        
        void Activate()
        {
            lastCam = camManager.currentCamera;
            camManager.Set(desiredCam);

            camSwitcher.canShift = false;
            active = true;
        }

        void OnTriggerExit(Collider other)
        {
            currentPlayer = camSwitcher.currentPlayer.transform;

            if (other.gameObject == currentPlayer.gameObject && active)
            {
                Deactivate();
            }
        }

        void Deactivate()
        {
            if (exitCam)
                camManager.Set(exitCam);
            else
                camManager.Set(lastCam);

            camSwitcher.canShift = true;

            active = false;
        }
    }
}


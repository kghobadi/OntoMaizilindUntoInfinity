using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the Prayer Wall Interaction. 
/// </summary>
public class PrayerWall : Interactive
{
    CameraSwitcher camSwitcher;
    public BombShelter bombShelter;
    
    public override void Awake()
    {
        base.Awake();
        
        camSwitcher = FindObjectOfType<CameraSwitcher>();
    }

    protected override void SetActive()
    {
        if (camSwitcher.currentCamObj.GetFPS().enabled && (int)bombShelter.transitionState < 2)
        {
            base.SetActive();
        }
    }

    protected override void Interact()
    {
        base.Interact();

        //check FPS enabled - only do this if we have not transitioned already
        if (camSwitcher.currentCamObj.GetFPS().enabled && (int)bombShelter.transitionState < 2)
        {
            //set human to pray
            camSwitcher.currentCamObj.Pray();
            //lock dist nec
            distNecessary = 0f;
        }
        
        //disable 
        SetInactive();
        
        //deactivate cursor
        iCursor.Deactivate();
    }
}

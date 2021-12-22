using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (camSwitcher.currentCamObj.GetFPS().enabled)
        {
            base.SetActive();
        }
    }

    protected override void Interact()
    {
        base.Interact();

        //check FPS enabled
        if (camSwitcher.currentCamObj.GetFPS().enabled)
        {
            //disable it and start prayer behavior. 
            camSwitcher.currentCamObj.GetFPS().enabled = false;
            camSwitcher.currentCamObj.GetNMA().enabled = true;
            //turn on AI movement and reset movement 
            camSwitcher.currentCamObj.GetMovement().AIenabled = true;
            //turn on AI to go pray 
            bombShelter.SetAIPosition(camSwitcher.currentCamObj);
            
            SetInactive();
        }
    }
    //TODO need this to communicate better with BombShelter transition system. 
    // could also just move the camera a bit on the NPC so it looks less weird when they go to pray, but may need to fiddle with it a bit. 
}

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
            //set to idle anim
            camSwitcher.currentCamObj.GetController().Animation.SetAnimator("idle");
            //set to Prayer idle.
            camSwitcher.currentCamObj.GetController().Animation.Animator.SetFloat("IdleType", 0.666667f);
            //disable FPS
            camSwitcher.currentCamObj.GetFPS().enabled = false;
        }
        
        SetInactive();
    }
    //TODO need this to communicate better with BombShelter transition system. 
    // could also just move the camera a bit on the NPC so it looks less weird when they go to pray, but may need to fiddle with it a bit. 
    
    //another option would be that we could just Trigger the prayer animation while keeping control of the FPS.
}

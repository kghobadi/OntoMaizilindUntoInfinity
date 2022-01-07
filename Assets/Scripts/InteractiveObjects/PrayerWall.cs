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
        if (camSwitcher.currentCamObj.GetFPS().enabled && (int)bombShelter.transitionState < 2)
        {
            base.SetActive();
        }
    }

    protected override void Interact()
    {
        base.Interact();

        //check FPS enabled
        if (camSwitcher.currentCamObj.GetFPS().enabled && (int)bombShelter.transitionState < 2)
        {
            //set to idle anim
            camSwitcher.currentCamObj.GetController().Animation.SetAnimator("idle");
            //set to Prayer idle.
            camSwitcher.currentCamObj.GetController().Animation.Animator.SetFloat("IdleType", 0.666667f);
            //disable FPS 
            camSwitcher.currentCamObj.GetFPS().enabled = false;
            //set cam view -- currently we are seeing weird angles of the npc, want to move it a bit or change fov
            camSwitcher.currentCamObj.GetCinemachineCam().m_Lens.FieldOfView = 80f;
            //lock dist nec
            distNecessary = 0f;
        }
        
        //disable 
        SetInactive();
        
        //deactivate cursor
        iCursor.Deactivate();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVclicker : Interactive {

    Television tv;
    
    [Header("Sounds")]
    public AudioClip[] staticClicks;

    protected override void Start()
    {
        base.Start();
        
        tv = FindObjectOfType<Television>();
    }

    protected override void SetActive()
    {
        //never set active for interaction if the speech stuff has started 
        if (tv.speechStarted == false && tv.waitingForStatic == false)
        {
            base.SetActive();
        }
    }

    protected override void Interact()
    {
        base.Interact();
        
        if(tv.speechStarted == false && tv.waitingForStatic == false)
        {
            tv.SwitchChannel();

            PlayRandomSoundRandomPitch(staticClicks, 1f);

            if(clickerUI)
                clickerUI.FadeOut();

            hasClicked = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVclicker : Interactive {

    Television tv;
    
    [Header("Sounds")]
    public AudioClip[] staticClicks;


    private int channelChanges;
    [Header("Adult Reactions")] 
    [SerializeField]
    private MonologueManager[] speakers;
    [SerializeField]
    private int[] monoIndexes;
    
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
        //disable cursor during speech stuff 
        else if (tv.waitingForStatic || tv.speechStarted)
        {
            iCursor.Deactivate();
        }
    }

    protected override void Interact()
    {
        base.Interact();
        
        //This only proceeds if the speech has not started. 
        if(tv.speechStarted == false && tv.waitingForStatic == false)
        {
            //trigger reaction monologue 
            if (tv.CurrentClip == 0)
            {
                if (channelChanges < speakers.Length)
                {
                    speakers[channelChanges].WaitToSetNewMonologue(monoIndexes[channelChanges]);
                }
                channelChanges++;
            }
            tv.SwitchChannel();

            PlayRandomSoundRandomPitch(staticClicks, 1f);
            
            if(clickerUI)
                clickerUI.FadeOut();

            hasClicked = true;
        }
    }
}

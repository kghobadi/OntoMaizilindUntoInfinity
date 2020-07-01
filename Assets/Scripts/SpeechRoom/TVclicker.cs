using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVclicker : AudioHandler {

    Television tv;

    [Header("Sounds")]
    public AudioClip[] staticClicks;

    public FadeUI clicker;
    public bool hasClicked;

    void Start()
    {
        tv = FindObjectOfType<Television>();
    }

    private void OnMouseEnter()
    {
        if(!hasClicked && tv.speechStarted == false)
            clicker.FadeIn();
    }

    private void OnMouseDown()
    {
        if(tv.speechStarted == false)
        {
            tv.SwitchChannel();

            PlayRandomSoundRandomPitch(staticClicks, 1f);

            clicker.FadeOut();

            hasClicked = true;
        }
    }

    private void OnMouseExit()
    {
        if(clicker)
            clicker.FadeOut();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVclicker : AudioHandler {

    Television tv;
    float dist;
    Transform player;

    [Header("Sounds")]
    public AudioClip[] staticClicks;

    public FadeUI clicker;
    public bool hasClicked;
    public float necDist = 7f;

    void Start()
    {
        tv = FindObjectOfType<Television>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        dist = Vector3.Distance(transform.position, player.position);
    }

    private void OnMouseEnter()
    {
        if(!hasClicked && tv.speechStarted == false && tv.waitingForStatic == false)
            clicker.FadeIn();
    }

    private void OnMouseOver()
    {
        if (tv.waitingForStatic)
        {
            if(clicker.fadingOut == false)
            {
                clicker.keepActive = false;
                clicker.FadeOut();
            }
        }
    }

    private void OnMouseDown()
    {
        if(tv.speechStarted == false && tv.waitingForStatic == false)
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

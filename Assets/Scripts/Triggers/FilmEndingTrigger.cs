using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Video;

public class FilmEndingTrigger : MonoBehaviour
{
    private VideoPlayer myVideoPlayer;
    public float endingOffset = 30f;
    public float volFadeInAt = 150f;
    public float volAddAmount = 0.35f;
    public float desiredVolume = 7f;
    public AudioMixerGroup filmGroup;
    
    public UnityEvent endingEvent;
    public bool ended;
    
    void Start()
    {
        myVideoPlayer = GetComponent<VideoPlayer>();
    }

    void Update()
    {
        CheckIsEnded();
    }

    void CheckIsEnded()
    {
        if (ended)
        {
            return;
        }

        // fade volume up once we hit correct point in the film 
        if (myVideoPlayer.isPlaying)
        {
            //are we at the time for the fade?
            if (myVideoPlayer.time > volFadeInAt)
            {
                //get current volume
                float currentVol = 0f;
                filmGroup.audioMixer.GetFloat("filmVol", out currentVol);
                
                //if it is less than desired...
                if (currentVol < desiredVolume)
                {
                    //move it up vol add amount 
                    filmGroup.audioMixer.SetFloat("filmVol", currentVol + volAddAmount);
                }
            }
        }
        
        if (myVideoPlayer.isPlaying)
        {
            //invoke our ending events 
            if (myVideoPlayer.frame >= myVideoPlayer.frameCount - endingOffset)
            {
                endingEvent?.Invoke();
                ended = true;
            }
        }
    }
}

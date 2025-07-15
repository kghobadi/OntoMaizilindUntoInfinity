using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Transition audio source when it finishes something. 
/// </summary>
public class OnAudioClipFinish : AudioHandler
{
    [SerializeField] 
    private bool setNewClipOnFinish;
    [SerializeField] 
    private AudioClip newClip;
    [SerializeField] 
    private bool setLoop;
    
    void Start()
    {
        InvokeRepeating("CheckFinished", 1f, 1f);
    }


    void CheckFinished()
    {
        if ( myAudioSource.isPlaying == false)
        {
            SetFinishedBehaviors();
        }
    }

    void SetFinishedBehaviors()
    {
        if (setNewClipOnFinish)
        {
            myAudioSource.clip = newClip;
        }

        if (setLoop)
        {
            myAudioSource.loop = true;
        }
        myAudioSource.Play();
    }
}

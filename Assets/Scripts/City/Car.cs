using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : AudioHandler {

    public AudioClip[] carAlarms;

    void Update()
    {
        if(myAudioSource.isPlaying == false)
        {
            //PlayRandomSoundRandomPitch(carAlarms, myAudioSource.volume);
        }
    }
}

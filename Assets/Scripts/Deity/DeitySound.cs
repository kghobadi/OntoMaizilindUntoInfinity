using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeitySound : AudioHandler {

    public AudioClip[] explosionSounds;
    public AudioClip[] alienWails;
    public AudioClip[] deathSounds;

    private void OnEnable()
    {
        PlayRandomSoundRandomPitch(alienWails, myAudioSource.volume);
    }
}

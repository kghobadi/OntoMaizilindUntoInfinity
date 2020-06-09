using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPC;

public class CitizenVision : MonoBehaviour {

    public Transform citizenParent;

    Sounds sounds;

    void Awake()
    {
        sounds = citizenParent.GetComponent<Sounds>();   
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bomb" || other.gameObject.tag == "Explosion")
        {
            if(sounds.myAudioSource.isPlaying == false)
                sounds.PlayRandomSoundRandomPitch(sounds.screams, 1f);
        }
    }
}

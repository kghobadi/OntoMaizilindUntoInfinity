using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CitizenVision : MonoBehaviour {

    public Transform citizenParent;

    Citizen citizenScript;

    void Awake()
    {
        citizenScript = citizenParent.GetComponent<Citizen>();   
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Bomb" || other.gameObject.tag == "Explosion")
        {
            if(citizenScript.citizenSounds.myAudioSource.isPlaying == false)
                citizenScript.citizenSounds.PlayRandomSoundRandomPitch(citizenScript.citizenSounds.screams, 1f);
        }
    }
}

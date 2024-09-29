using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPC;

public class CitizenVision : MonoBehaviour 
{
    public Transform citizenParent;
    public float screamChance = 25f;

    Sounds sounds;

    void Awake()
    {
        sounds = citizenParent.GetComponent<Sounds>();   
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Bomb") || other.gameObject.CompareTag("Explosion"))
        {
            Scream();
        }
    }

    void Scream()
    {
        if (sounds.myAudioSource.isPlaying == false)
        {
            float screamCheck = Random.Range(0f, 100f);
            if (screamCheck <= screamChance)
            {
                sounds.PlayRandomSoundRandomPitch(sounds.screams, 1f);
            }
        }
    }
}

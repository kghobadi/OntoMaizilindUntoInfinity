using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeityHealth : MonoBehaviour {
    DeitySound _Sounds;

    public int healthPoints = 133;
    public ObjectPooler splosionPooler;

    private void Awake()
    {
        _Sounds = GetComponentInParent<DeitySound>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            //self destruct
        }

        if(other.tag == "Bullet")
        {
            //take damage
            TakeDamage(other.gameObject);
        }
    }

    void TakeDamage(GameObject bull)
    {
        //get bullet
        Bullet bullet = bull.GetComponent<Bullet>();
        //spawn splosion
        GameObject splosion = splosionPooler.GrabObject();
        splosion.transform.position = bull.transform.position;
        //particle system
        ParticleSystem sParticles = splosion.GetComponent<ParticleSystem>();
        sParticles.Play();
        //reset bullet
        bullet.ResetBullet();
        //sub health
        healthPoints--;
        //explosion sound 
        _Sounds.PlayRandomSound(_Sounds.explosionSounds, _Sounds.myAudioSource.volume);

        
        if(healthPoints <= 0)
        {
            //Fall
        }
    }

    void Fall()
    {
        //find spot on ground in front of me to move towards at fall speed
    }

    void Crash()
    {
        //when i hit the ground and explode 
    }
}

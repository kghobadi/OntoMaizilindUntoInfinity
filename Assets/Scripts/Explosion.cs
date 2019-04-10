using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
    //audio vars
    AudioSource explosionAudio;
    public AudioClip[] explosions;

    //particles
    ParticleSystem explosionParts;
    ParticleSystem.MainModule eMain;
    
    void Start () {
        explosionAudio = GetComponent<AudioSource>();
        explosionParts = GetComponent<ParticleSystem>();
        eMain = explosionParts.main;

        //set explode sound
        int randomFall = Random.Range(0, explosions.Length);
        explosionAudio.clip = explosions[randomFall];
        explosionAudio.Play();

        //duration of particle efx should be audio length * speed of simulation (0.5f)
        eMain.duration = explosionAudio.clip.length * eMain.simulationSpeed;
        explosionParts.Play();
    }
	
	void Update () {
		if(explosionAudio.isPlaying == false)
        {
            //Destroy(gameObject);
            Debug.Log("it is in our house now");
        }
	}
}

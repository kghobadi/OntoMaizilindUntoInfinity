using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//explosion is produced by the bomb class
public class Explosion : MonoBehaviour {
    //world manager ref
    WorldManager worldMan;
    CameraSwitcher camSwitcher;

    //audio vars
    AudioSource explosionAudio;
    public AudioClip[] explosions;

    //particles
    ParticleSystem explosionParts;
    ParticleSystem.MainModule eMain;
    
    void Start () {
        //world man and add to list
        worldMan = GameObject.FindGameObjectWithTag("WorldManager").GetComponent<WorldManager>();
        worldMan.explosionsToDelete.Add(gameObject);
        camSwitcher = worldMan.GetComponent<CameraSwitcher>();

        //component refs
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
        if (explosionAudio.isPlaying == false)
        {
            //Destroy(gameObject);
            Debug.Log("it is in our house now");
        }

        //could add a collider to this so that when it overlaps with other explosion fire, they combine into one thing
	}

    private void OnTriggerEnter(Collider other)
    {
        //kill a human
        if (other.gameObject.tag == "Human")
        {
            //if this is the human currently being played
            if (camSwitcher.cameraObjects[camSwitcher.currentCam] == other.gameObject)
            {
                //switch to next viewer
                camSwitcher.SwitchCam(true, -1);
            }

            //remove this human from cam objects list
            camSwitcher.cameraObjects.Remove(other.gameObject);

            //destroy the human
            Destroy(other.gameObject);
        }
    }
}

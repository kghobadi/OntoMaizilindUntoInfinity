using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//explosion is produced by the bomb class
public class Explosion : MonoBehaviour {
    //world manager ref
    WorldManager worldMan;
    CameraSwitcher camSwitcher;

    //audio vars
    AudioSource explosionAudio;
    public AudioClip[] explosions;
    public AudioClip fireBurning;
    public AudioMixerGroup fireGroup;
    int randomFall;

    //particles
    ParticleSystem explosionParts;
    ParticleSystem.MainModule eMain;

    void Awake()
    {
        //world man and add to list
        worldMan = FindObjectOfType<WorldManager>();
        camSwitcher = FindObjectOfType<CameraSwitcher>();

        //component refs
        explosionAudio = GetComponent<AudioSource>();
        explosionParts = GetComponent<ParticleSystem>();
    }
    
    void Start () {
        //set particles 
        worldMan.explosionsToDelete.Add(gameObject);
        eMain = explosionParts.main;

        //set explode sound
        randomFall = Random.Range(0, explosions.Length);
        explosionAudio.clip = explosions[randomFall];
        explosionAudio.Play();

        //duration of particle efx should be audio length * speed of simulation (0.5f)
        eMain.duration = explosionAudio.clip.length * eMain.simulationSpeed;
        explosionParts.Play();
    }
	
	void Update ()
    {
        //audio stopped playing after explosion 
        if (explosionAudio.isPlaying == false && explosionAudio.clip == explosions[randomFall])
        {
            //Debug.Log("just a fire burning///");
            explosionAudio.Stop();
            explosionAudio.clip = fireBurning;
            explosionAudio.outputAudioMixerGroup = fireGroup;
            explosionAudio.loop = true;
        }

        //on fire buring 
        if(explosionAudio.clip == fireBurning)
        {
            //float dist = Vector3.Distance(transform.position, )
        }
	}

    //could add to this so that when it overlaps with other explosion fire, they combine into one thing
    private void OnTriggerEnter(Collider other)
    {
        //kill a human
        if (other.gameObject.tag == "Human")
        {
            Debug.Log("human burnssss");

            //if this is the human currently being played
            if (other.gameObject.GetComponent<FirstPersonController>().enabled)
            {
                //switch to next viewer
                camSwitcher.SwitchCam(false, 0);
                Debug.Log("it was you who died");
            }

            //remove this human from cam objects list
            camSwitcher.cameraObjects.Remove(other.gameObject.GetComponent<CamObject>());

            //destroy the human
            Destroy(other.gameObject);

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

//explosion is produced by the bomb class
public class Explosion : AudioHandler {
    //world manager ref
    WorldManager worldMan;
    CameraSwitcher camSwitcher;

    //audio vars
    AudioSource explosionAudio;
    [Header("Sounds")]
    public AudioClip[] explosions;
    public AudioClip fireBurning;
    public AudioMixerGroup fireGroup;
    int randomFall;

    //particles
    ParticleSystem explosionParts;
    ParticleSystem.MainModule eMain;

    public override void Awake()
    {
        base.Awake();

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
        
        //play particles 
        explosionParts.Play();

        //audio 
        StartCoroutine(ExplodeThenFireSounds());
    }

    IEnumerator ExplodeThenFireSounds()
    {
        ExplosionSound();

        yield return new WaitForSeconds(explosionAudio.clip.length);

        FireSound();
    }
    
	void Update ()
    {
        //on fire buring 
        if(explosionAudio.clip == fireBurning)
        {
            AudioCheck();
        }
	}

    void ExplosionSound()
    {
        //set explode sound
        randomFall = Random.Range(0, explosions.Length);
        explosionAudio.clip = explosions[randomFall];
        explosionAudio.PlayOneShot(explosions[randomFall]);
    }

    //stop audio source and set fire sound to loop 
    void FireSound()
    {
        explosionAudio.Stop();
        explosionAudio.clip = fireBurning;
        explosionAudio.outputAudioMixerGroup = fireGroup;
        explosionAudio.loop = true;
    }

    //checks whether to play looping audio based on distance from current player 
    void AudioCheck()
    {
        //grab current player 
        Transform currentPlayer = camSwitcher.cameraObjects[camSwitcher.currentCam].transform;
        //dist check
        float dist = Vector3.Distance(transform.position, currentPlayer.position);

        //close enough -- play audio 
        if (dist < 150f)
        {
            if (explosionAudio.isPlaying == false)
                explosionAudio.Play();
        }
        //disable audio
        else
        {
            if (explosionAudio.isPlaying)
                explosionAudio.Stop();
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

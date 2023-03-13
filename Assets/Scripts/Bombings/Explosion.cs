using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using NPC;
using UnityEngine;
using UnityEngine.Audio;
using Debug = UnityEngine.Debug;
using DG.Tweening;

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
    
    [Header("Parents Death")]
    public Transform spiritWritingSpot;
    public Transform momDead;
    public Transform dadDead;
    public Transform playerSpot;
    public override void Awake()
    {
        base.Awake();

        //world man and add to list
        worldMan = FindObjectOfType<WorldManager>();
        camSwitcher = worldMan.GetComponent<CameraSwitcher>();

        //component refs
        explosionAudio = GetComponent<AudioSource>();
        explosionParts = GetComponent<ParticleSystem>();
    }
    
    void Start () 
    {
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
    
	/*void Update ()
    {
        //on fire buring 
        if(explosionAudio.clip == fireBurning)
        {
            AudioCheck();
        }
	}*/

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

        InvokeRepeating("AudioCheck", Random.value, 1);
    }

    //checks whether to play looping audio based on distance from current player 
    void AudioCheck()
    {
        //grab current player 
        Transform currentPlayer = camSwitcher.currentPlayer.transform;
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
        //kill a human/
        if (other.gameObject.tag == "Human" )
        {
            //Debug.Log("human burnssss");

            //if this is the human currently being played
            if (other.gameObject.GetComponent<FirstPersonController>().enabled)
            {
                //switch to next viewer
                camSwitcher.SetCam(0);
                //kill the human you were playing
                KillHuman(other.gameObject);
                Debug.Log("it was you who died");
            }
            //not you 
            else
            {
                //kill the human
                KillHuman(other.gameObject);
            }
        }

        //player
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("player is in explosion!");
            //do nothing for now? 
        }
    }

    //kills a human
    void KillHuman(GameObject humanObj)
    {
        //remove this human from cam objects list
        camSwitcher.cameraObjects.Remove(humanObj.GetComponent<CamObject>());

        //destroy the human
        Movement npc = humanObj.GetComponent<Movement>();
        if (npc)
        {
            npc.ResetMovement(camSwitcher.death);
        }
    }
}

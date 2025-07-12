using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Lightning : AudioHandler {
    Transform pilot;
    ThePilot the_Pilot;
    float distFromPlayer;

    ParticleSystem lightningParticles;
    public AudioClip[] thunderStrikes;

    public float lightningTimer;
    public Vector2 lightningFreqRange = new Vector2(5, 15);
    public bool lightningCloud;
    public float lightningChance = 33f;

    public bool alwaysFaceGround; 

    public override void Awake()
    {
        base.Awake();

        the_Pilot = FindObjectOfType<ThePilot>();
        if (the_Pilot)
            pilot = the_Pilot.transform;

        LightningCheck();
    }

    //decide whether this cloud will produce lightning 
    void LightningCheck()
    {
        Random.InitState((int)System.DateTime.Now.Ticks);
        lightningParticles = GetComponent<ParticleSystem>();

        float randomChance = Random.Range(0f, 100f);

        if (randomChance < lightningChance)
        {
            lightningCloud = true;
        }
    }

    void Start()
    {
        lightningTimer = Random.Range(lightningFreqRange.x, lightningFreqRange.y);
    }

    void Update()
    {
        //only some clouds are chosen && MUST BE ACTIVE GAMEOBJ
        if (lightningCloud && gameObject.activeSelf)
        {
            //look at ground/straight down 
            if (alwaysFaceGround)
            {
                transform.LookAt(new Vector3(transform.position.x, transform.position.y - 1000f, transform.position.z));
            }
            //tick thunderstrike time 
            lightningTimer -= Time.deltaTime;
            if (lightningTimer < 0)
            {
                Thunderstrike();
            }
        }
    }

    //lightning and thunder all in one 
    void Thunderstrike()
    {
        //particles
        lightningParticles.Play();
        //sound
        PlayRandomSoundRandomPitch(thunderStrikes, 1f);
        //reset timer
        lightningTimer = Random.Range(lightningFreqRange.x, lightningFreqRange.y);
    }

    private void OnTriggerEnter(Collider other)
    {
        //Is the lightning active?
        if (gameObject.activeSelf && (myAudioSource.isPlaying || lightningParticles.isPlaying))
        {
            //ZAP method #1
            if (other.gameObject.CompareTag("Player"))
            {
                the_Pilot.InitiateZap();
            }
        }
    }
    
    private void OnTriggerStay(Collider other)
    {
        //Is the lightning active?
        if (gameObject.activeSelf && (myAudioSource.isPlaying || lightningParticles.isPlaying))
        {
            //ZAP method #2
            if (other.gameObject.CompareTag("Player"))
            {
                the_Pilot.InitiateZap();
            }
        }
    }

    private void OnDisable()
    {
        myAudioSource.Stop();
        lightningParticles.Stop();
    }
}

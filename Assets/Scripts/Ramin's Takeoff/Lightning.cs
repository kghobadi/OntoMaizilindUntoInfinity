using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : AudioHandler {
    Transform pilot;
    ThePilot the_Pilot;
    float distFromPlayer;

    ParticleSystem lightningParticles;
    public AudioClip[] thunderStrikes;
    public float zappingDist = 15f;

    public float lightningTimer;
    public Vector2 lightningFreqRange = new Vector2(5, 15);
    public bool lightningCloud;
    public float lightningChance = 33f;

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
        //only some clouds are chosen 
        if (lightningCloud)
        {
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

        //check dist from player
        if (pilot)
        {
            distFromPlayer = Vector3.Distance(transform.position, pilot.position);

            //ZAP method #1
            //disable player controls when close
            if (distFromPlayer < zappingDist)
            {
                the_Pilot.InitiateZap();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (myAudioSource.isPlaying || lightningParticles.isPlaying)
        {
            //ZAP method #2
            if (other.gameObject.CompareTag("Player"))
            {
                the_Pilot.InitiateZap();
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lightning : AudioHandler {

    ParticleSystem lightningParticles;
    public AudioClip[] thunderStrikes;

    public float lightningTimer, lightningFreq = 15f;
    public bool lightningCloud;
    public float lightningChance = 33f;

    public override void Awake()
    {
        base.Awake();
        Random.InitState((int)System.DateTime.Now.Ticks);
        lightningParticles = GetComponent<ParticleSystem>();

        float randomChance = Random.Range(0f, 100f);

        if(randomChance < lightningChance)
        {
            lightningCloud = true;
        }
    }

    void Start () {
        lightningTimer = lightningFreq + Random.Range(-10, 10);
    }
	
	void Update () {
        if (lightningCloud)
        {
            lightningTimer -= Time.deltaTime;
            if (lightningTimer < 0)
            {
                Thunderstrike();
            }
        }
	}

    void Thunderstrike()
    {
        lightningParticles.Play();
        PlayRandomSoundRandomPitch(thunderStrikes, 1f);
        lightningTimer = lightningFreq + Random.Range(-10, 10);
    }
}

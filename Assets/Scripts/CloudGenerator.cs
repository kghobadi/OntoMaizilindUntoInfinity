using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
    public ObjectPooler cloudPooler;
    GameObject cloudClone;

    public float cloudSpeed;
    public float speedMin, speedMax;
    public float distanceToDestroy;
    //spawn times
    public float spawnTimer, spawnInterval;

    void Awake()
    {
        spawnTimer = spawnInterval + Random.Range(-1f, 1f);
    }

    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if(spawnTimer < 0)
        {
            SpawnCloud();
            spawnTimer = spawnInterval + Random.Range(-1f, 1f);
        }
        
    }

    //spawn normal cloud
    void SpawnCloud()
    {
        //randomize spawn pos a bit
        Vector3 spawnPos = transform.position + Random.insideUnitSphere * 150f;
        spawnPos = new Vector3(spawnPos.x, transform.position.y, spawnPos.z);
        //grab obj from pool and set pos
        cloudClone = cloudPooler.GrabObject();
        cloudClone.transform.SetParent(null);
        cloudClone.transform.position = spawnPos;
        cloudClone.transform.rotation = Quaternion.Euler(transform.eulerAngles);
        //assign refs to rain cloud script
        cloudClone.GetComponent<Cloud>()._cloudGen = this;

        //grab all the particle mod refs
        ParticleSystem cloudSystem = cloudClone.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule cloudMain = cloudSystem.main;
        ParticleSystem.ShapeModule cloudShape = cloudSystem.shape;
        ParticleSystem.EmissionModule cloudEmis = cloudSystem.emission;

        //random max particles
        cloudMain.maxParticles += Random.Range(-15, 15);

        //randomize shape 
        float xScale = cloudShape.scale.x + Random.Range(-5f, 5f);
        float yScale = cloudShape.scale.y + Random.Range(-2.5f, 2.5f);
        float zScale = cloudShape.scale.z + Random.Range(-5f, 5f);
        cloudShape.scale = new Vector3(xScale, yScale, zScale);
    }
    
    public void RandomizeCloudSpeed()
    {
        cloudSpeed = Random.Range(speedMin, speedMax);
    }
}

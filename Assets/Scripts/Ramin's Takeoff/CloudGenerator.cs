using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudGenerator : MonoBehaviour
{
    Vector3 origPos;
    public ObjectPooler cloudPooler;
    GameObject cloudClone;
    public GameObject[] generatedObjs;
    public GenerationType generationType;
    public enum GenerationType
    {
        RANDOM, SQUARE, 
    }
    //for RANDOM
    [Header("RANDOM")]
    public int generationAmount;
    public float generationRadius;

    //for SQUARE
    [Header("SQUARE")]
    public int gridSizeX;
    public int gridSizeY;
    public float distBetweenX, distBetweenY;

    //cloud settings 
    [Header("Cloud Settings")]
    public float cloudSpeed;
    public float speedMin, speedMax;
    public float distanceToDestroy;
    //spawn times
    public float spawnTimer, spawnIntervalMin = 3f, spawnIntervalMax = 5f;
    [Tooltip("Lowest Height on Y axis to spawn")]
    public float minHeight = 250f;

    public float scaleMin = 0.5f, scaleMax = 2f;

    void Awake()
    {
        //random y
        transform.position = new Vector3(transform.position.x,
            transform.position.y + Random.Range(-15f, 75f), transform.position.z);
        //set orig pos 
        origPos = transform.position;
        //randomize spawn timer 
        spawnTimer = Random.Range(spawnIntervalMin, spawnIntervalMax);
    }

    //so cloud spawning is frame dependent huh
    void Update()
    {
        spawnTimer -= Time.deltaTime;
        if(spawnTimer < 0)
        {
            //randomize spawn center on x axis 
            transform.position = new Vector3(transform.position.x + Random.Range(-15f, 15f),
                transform.position.y, transform.position.z);

            //generation patterns 
            if(generationType == GenerationType.RANDOM)
            {
                GenerateRandom();
            }
            if (generationType == GenerationType.SQUARE)
            {
                GenerateSquare();
            }

            //randomize spawn timer 
            spawnTimer = Random.Range(spawnIntervalMin, spawnIntervalMax);
        }
    }

    //spawn normal cloud
    void SpawnCloud(Vector3 spawnPos)
    {
        //grab obj from pool and set pos
        cloudClone = cloudPooler.GrabObject();
        cloudClone.transform.SetParent(null);
        cloudClone.transform.position = spawnPos;
        cloudClone.transform.rotation = Quaternion.Euler(transform.eulerAngles);
        //assign refs to rain cloud script
        cloudClone.GetComponent<Cloud>()._cloudGen = this;

        //randomize cloud scale 
        float randomScale = Random.Range(scaleMin, scaleMax);
        cloudClone.transform.localScale *= randomScale;
    }

    //generate objects in a random unit circle 
    void GenerateRandom()
    {
        for (int i = 0; i < generationAmount; i++)
        {
            Vector2 xz = Random.insideUnitCircle * generationRadius;

            Vector3 spawnPos = transform.position + new Vector3(xz.x, 0, xz.y);

            SpawnCloud(spawnPos);
        }
    }

    //generate objects in a square grid pattern 
    void GenerateSquare()
    {
        //set to size of the grid we will be making 
        generatedObjs = new GameObject[(gridSizeX + 1) * (gridSizeY + 1)];

        for (int i = 0, y = 0; y <= gridSizeY; y++)
        {
            for (int x = 0; x <= gridSizeX; x++, i++)
            {
                Vector3 spawnPos = new Vector3(x * distBetweenX, transform.position.y, y * distBetweenY) + transform.position;

                SpawnCloud(spawnPos);
            }
        }
    }

    public void RandomizeCloudSpeed()
    {
        cloudSpeed = Random.Range(speedMin, speedMax);
    }
}

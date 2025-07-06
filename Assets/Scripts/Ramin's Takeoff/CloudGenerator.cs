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
        RANDOM, 
        SQUARE,
        SQUID,
    }
    
    //for RANDOM
    [Header("RANDOM")]
    public int generationAmount;
    public float generationRadius;
    [SerializeField] private bool includeHeight;

    //for SQUARE
    [Header("SQUARE")]
    public int gridSizeX;
    public int gridSizeY;
    public float distBetweenX, distBetweenY;

    //cloud settings 
    [Header("Cloud Travel Speed & Lifetime")]
    public float cloudSpeed;
    public float speedMin, speedMax;
    public float distanceToDestroy;
    
    [Header("Spawn Timing")]
    [Tooltip("Check true to use another method to spawn clouds - most likely animation events.")]
    [SerializeField] private bool disableSpawnTimers;
    public float spawnTimer, spawnIntervalMin = 3f, spawnIntervalMax = 5f;

    [Header("Extra Randomization")]
    [SerializeField] private bool randomYAtStart;
    [SerializeField] private Vector2 randomYRange = new Vector2(-15f, 75f);

    [SerializeField] private bool randomXAtGenerate;
    [SerializeField] private Vector2 randomXRange = new Vector2(-15f, 15);

    [Header("Scaling")] 
    public float scaleMin = 0.5f;
    public float scaleMax = 2f;
    public bool tweenScale;
    public Vector3 startScale = new Vector3(0.1f, 0.1f, 0.1f);
    public float scaleTime = 1f;

    void Awake()
    {
        //TODO why set a random y like this? 
        //random y
        if (randomYAtStart)
        {
            transform.position = new Vector3(transform.position.x,
                transform.position.y + Random.Range(randomYRange.x, randomYRange.y), transform.position.z);
        }

        //set orig pos 
        origPos = transform.position;
        //randomize spawn timer 
        spawnTimer = Random.Range(spawnIntervalMin, spawnIntervalMax);
    }

    //so cloud spawning is frame dependent huh
    void Update()
    {
        if (!disableSpawnTimers)
        {
            spawnTimer -= Time.deltaTime;
            if(spawnTimer < 0)
            {
                Generate();
                //randomize spawn timer 
                spawnTimer = Random.Range(spawnIntervalMin, spawnIntervalMax);
            }
        }
    }

    public void Generate()
    {
        //TODO why do all generate behaviors do this? 
        //randomize spawn center on x axis 
        if (randomXAtGenerate)
        {
            transform.position = new Vector3(transform.position.x + Random.Range(randomXRange.x, randomXRange.y),
                transform.position.y, transform.position.z);
        }

        //generation patterns 
        if(generationType == GenerationType.RANDOM)
        {
            GenerateRandom();
        }
        else if (generationType == GenerationType.SQUARE)
        {
            GenerateSquare();
        }
        else if (generationType == GenerationType.SQUID)
        {
            GenerateSquid();
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

        //randomize cloud scale  - tween 
        if (tweenScale)
        {
            float randomScale = Random.Range(scaleMin, scaleMax);
            Vector3 finalScale = cloudPooler.ObjPrefab.transform.localScale * randomScale;
            cloudClone.transform.localScale = startScale;
            LeanTween.scale(cloudClone, finalScale, scaleTime);
        }
        //Instantly set scale
        else
        {
            float randomScale = Random.Range(scaleMin, scaleMax);
            cloudClone.transform.localScale = randomScale * cloudPooler.ObjPrefab.transform.localScale;
        }
    }

    //generate objects in a random unit circle 
    void GenerateRandom()
    {
        for (int i = 0; i < generationAmount; i++)
        {
            Vector2 xz = Random.insideUnitCircle * generationRadius;

            //todo this yields 0 randomization on Y 
            float y = 0;
            if (includeHeight)
            {
                y = Random.Range(-generationRadius, generationRadius);
            }
            Vector3 spawnPos = transform.position + new Vector3(xz.x, y, xz.y);

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
    
    //generate objects in a square grid pattern 
    void GenerateSquid()
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

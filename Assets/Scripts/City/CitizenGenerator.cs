using System.Collections;
using System.Collections.Generic;
using NPC;
using UnityEngine;

/// <summary>
/// Handles the generation of Citizens at different locations on the City map during the bombing sequence. 
/// </summary>
public class CitizenGenerator : MonoBehaviour
{
    private bool init;
    private CameraSwitcher camSwitcher;
    Vector3 origPos;
    [SerializeField] ObjectPooler citizenPooler;
    GameObject citizenClone;
    [SerializeField] GameObject[] generatedObjs;
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

    [Header("Spawn Positioning")] 
    public Transform currentSpawnNexus;
    public LayerMask grounded;
    public int groundLayer;
    public Vector2 xRange = new Vector2(-15f, 15f);
    public Vector2 yRange = new Vector2(-15f, 15f);
    public Vector2 zRange = new Vector2(-15f, 15f);

    [Header("Spawn Timing")]
    //spawn times
    public bool usesSpawnTiming;
    public float spawnTimer;
    public float spawnIntervalMin = 3f, spawnIntervalMax = 5f;

    //Citizen settings 
    [Header("Citizen Settings")] 
    public bool useRandomSpeed;
    public float citizenSpeed;
    public float speedMin, speedMax;
    public float distanceToDestroy;

    [Header("Scale")]
    public bool useRandomScale;
    public float scaleMin = 0.5f, scaleMax = 2f;
    private Vector3 origObjScale;

    void Awake()
    {
        Init();
    }

    void Init()
    {
        if (!init)
        {
            //get cam switcher 
            camSwitcher = FindObjectOfType<CameraSwitcher>();
            //randomize spawn timer 
            spawnTimer = Random.Range(spawnIntervalMin, spawnIntervalMax);
            if (currentSpawnNexus == null)
            {
                currentSpawnNexus = transform;
            }

            //store orig scale of the pooler prefab
            if (useRandomScale)
            {
                origObjScale = citizenPooler.ObjPrefab.transform.localScale;
            }
            init = true;
        }
    }

    //so cloud spawning is frame dependent huh
    void FixedUpdate()
    {
        if (usesSpawnTiming)
        {
            //Tick spawn time 
            spawnTimer -= Time.fixedDeltaTime;
            //Time to spawn!
            if(spawnTimer < 0)
            {
                SpawnCitizens();

                //randomize spawn timer 
                spawnTimer = Random.Range(spawnIntervalMin, spawnIntervalMax);
            }
        }
    }
    
    public void SpawnCitizens()
    {
        Init();
        
        GetRandomSpawnPosition();
            
        //generation patterns 
        if(generationType == GenerationType.RANDOM)
        {
            GenerateRandom();
        }
        if (generationType == GenerationType.SQUARE)
        {
            GenerateSquare();
        }
    }

    /// <summary>
    /// Overrides current spawn nexus. 
    /// </summary>
    /// <param name="nexus"></param>
    public void SetSpawnNexus(Transform nexus)
    {
        currentSpawnNexus = nexus;
    }

    void GetRandomSpawnPosition()
    {
        //get spawn nexus
        Vector3 spawnNexus = currentSpawnNexus.position;
        //get random positions 
        float randomX = spawnNexus.x + Random.Range(xRange.x, xRange.y);
        float randomY = spawnNexus.y + Random.Range(yRange.x, yRange.y);
        float randomZ = spawnNexus.z + Random.Range(zRange.x, zRange.y);
        //randomize spawn center on x axis 
        transform.position = new Vector3(randomX, randomY, randomZ);
    }

    //spawn normal cloud
    void SpawnCitizen(Vector3 spawnPos)
    {
        //grab obj from pool and set pos
        citizenClone = citizenPooler.GrabObject();
        citizenClone.transform.SetParent(currentSpawnNexus.transform);
        citizenClone.transform.position = spawnPos;
        citizenClone.transform.rotation = Quaternion.Euler(transform.eulerAngles);
        //get citizen cam obj
        CamObject citizenCam = citizenClone.GetComponent<CamObject>();
        if (citizenCam)
        {
            camSwitcher.AddCamObject(citizenCam);
        }

        //get npc and ground it 
        Movement npcMove = citizenClone.GetComponent<Movement>();
        if (npcMove)
        {
            npcMove.SnapToGroundPoint();
        }
        
        //randomize citizen speed 
        if (useRandomSpeed)
        {
            citizenSpeed = Random.Range(speedMin, speedMax);
            //apply to nav mesh agent/Movement.cs
        }

        //randomize citizen scale 
        if (useRandomScale)
        {
            float randomScale = Random.Range(scaleMin, scaleMax);
            citizenClone.transform.localScale = origObjScale * randomScale;   
        }
    }

    //generate objects in a random unit circle 
    void GenerateRandom()
    {
        for (int i = 0; i < generationAmount; i++)
        {
            Vector2 xz = Random.insideUnitCircle * generationRadius;

            Vector3 spawnPos = transform.position + new Vector3(xz.x, 0, xz.y);
            
            //make sure we get a spawn pos that can get a grounded point
            while (!IsGroundPoint(spawnPos))
            {
                xz = Random.insideUnitCircle * generationRadius;

                spawnPos = transform.position + new Vector3(xz.x, 0, xz.y);
            }

            SpawnCitizen(spawnPos);
        }
    }

    public void GenerateRandomOverTime()
    {
        StartCoroutine(RandomOverTime());
    }

    /// <summary>
    /// Spawns citizens over time. 
    /// </summary>
    /// <returns></returns>
    IEnumerator RandomOverTime()
    {
        for (int i = 0; i < generationAmount; i++)
        {
            Vector2 xz = Random.insideUnitCircle * generationRadius;

            Vector3 spawnPos = transform.position + new Vector3(xz.x, 0, xz.y);
            
            //make sure we get a spawn pos that can get a grounded point
            while (!IsGroundPoint(spawnPos))
            {
                xz = Random.insideUnitCircle * generationRadius;

                spawnPos = transform.position + new Vector3(xz.x, 0, xz.y);
            }

            SpawnCitizen(spawnPos);

            yield return new WaitForSeconds(spawnTimer);
        }
    }
    
    /// <summary>
    /// Teleports AI to Ground Pos. 
    /// </summary>
    public bool IsGroundPoint(Vector3 point)
    {
        bool isGrounded = false;
        RaycastHit hit;
        GameObject hitGameObj;
        
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(point, Vector3.down, out hit, 1500f, grounded))
        {
            hitGameObj = hit.collider.gameObject;
            isGrounded = hitGameObj.layer ==  groundLayer;
        }
        // Try up
        else if (Physics.Raycast(transform.position, Vector3.up, out hit, 1500f, grounded))
        {
            hitGameObj = hit.collider.gameObject;
            isGrounded = hitGameObj.layer ==  groundLayer;
        }

        return isGrounded;
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

                SpawnCitizen(spawnPos);
            }
        }
    }
}
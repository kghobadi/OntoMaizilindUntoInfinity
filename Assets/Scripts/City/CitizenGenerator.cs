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
    [SerializeField] GameObject citizenPrefab;
    [SerializeField] ObjectPooler citizenPooler;
    GameObject citizenClone;
    [SerializeField] GameObject[] generatedObjs;
    
    [Header("RANDOM Gen Settings")]
    public int generationAmount;
    public float generationRadius;
    
    [Header("Spawn Positioning")] 
    public Transform currentSpawnNexus;
    private Vector3 spawnNexus;
    private Vector3 currentSpawnPos;
    public LayerMask grounded;
    public int groundLayer;
    public float sphereCastRadius = 1.5f;
    
    [Header("Spawn Timing")]
    public bool usesSpawnTiming;
    public float spawnTimer;
    public float spawnIntervalMin = 3f, spawnIntervalMax = 5f;

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
          
            if (currentSpawnNexus == null)
            {
                currentSpawnNexus = transform;
            }

            if (usesSpawnTiming)
            {
                //randomize spawn timer 
                spawnTimer = Random.Range(spawnIntervalMin, spawnIntervalMax);
            }

            //store orig scale of the pooler prefab
            if (useRandomScale)
            {
                if (citizenPooler)
                {
                    origObjScale = citizenPooler.ObjPrefab.transform.localScale;
                }
                else
                {
                    origObjScale = citizenPrefab.transform.localScale;
                }
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
    
    /// <summary>
    /// Overrides current spawn nexus. 
    /// </summary>
    /// <param name="nexus"></param>
    public void SetSpawnNexus(Transform nexus)
    {
        currentSpawnNexus = nexus;
    }
    
    public void SpawnCitizens()
    {
        Init();
        
        GetRandomSpawnPosition();
            
        GenerateRandom();
    }

    /// <summary>
    /// Returns random spawn nexus within gen radius of the Current Spawn Nexus. 
    /// </summary>
    void GetRandomSpawnPosition()
    {
        //get random point in generation radius 
        Vector2 xz = Random.insideUnitCircle * generationRadius;
        //get spawn nexus by adding xz point to current spawn nexus pos 
        spawnNexus =  currentSpawnNexus.position + new Vector3(xz.x, 0, xz.y);
    }
    
    /// <summary>
    /// Generates designated objects in a random unit circle 
    /// </summary>
    void GenerateRandom()
    {
        //set to size of the grid we will be making 
        generatedObjs = new GameObject[generationAmount];
        
        for (int i = 0; i < generationAmount; i++)
        {
            GetRandomSpawnPosition();
            
            //make sure we get a spawn pos that can get a grounded point
            while (!IsGroundPoint(spawnNexus))
            {
                GetRandomSpawnPosition();
            }

            //set array element
            generatedObjs[i] = SpawnCitizen(currentSpawnPos);
        }
    }

    /// <summary>
    /// Generates citizens over time. 
    /// </summary>
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
        //set to size of the grid we will be making 
        generatedObjs = new GameObject[generationAmount];
        
        for (int i = 0; i < generationAmount; i++)
        {
            GetRandomSpawnPosition();

            //make sure we get a spawn pos that can get a grounded point
            while (!IsGroundPoint(spawnNexus))
            {
                GetRandomSpawnPosition();

                yield return null;
            }

            //set array element
            generatedObjs[i] = SpawnCitizen(currentSpawnPos);

            yield return new WaitForSeconds(spawnTimer);
        }
    }

    //spawn normal cloud
    GameObject SpawnCitizen(Vector3 spawnPos)
    {
        //use pooler
        if (citizenPooler)
        {
            //grab obj from pool and set pos
            citizenClone = citizenPooler.GrabObject();
            citizenClone.transform.position = spawnPos;
            citizenClone.transform.SetParent(transform);
        }
        //fresh instantiate
        else
        {
            citizenClone = Instantiate(citizenPrefab, spawnPos, Quaternion.identity, transform);
        }

        //get citizen cam obj
        CamObject citizenCam = citizenClone.GetComponent<CamObject>();
        if (citizenCam)
        {
            camSwitcher.AddCamObject(citizenCam);
            //only disable cam while in play mode 
            if (Application.isPlaying)
            {
                camSwitcher.DisableCamObj(citizenCam);
            }
        }

        //randomize citizen scale 
        if (useRandomScale)
        {
            float randomScale = Random.Range(scaleMin, scaleMax);
            citizenClone.transform.localScale = origObjScale * randomScale;   
        }

        return citizenClone;
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
        if (Physics.SphereCast(point, sphereCastRadius, Vector3.down, out hit, 1500f, grounded))
        {
            hitGameObj = hit.collider.gameObject;
            isGrounded = hitGameObj.layer ==  groundLayer;
            if (isGrounded)
            {
                currentSpawnPos = hit.point;
            }
        }
        // Try up
        else if (Physics.SphereCast(point, sphereCastRadius, Vector3.up, out hit, 1500f, grounded))
        {
            hitGameObj = hit.collider.gameObject;
            isGrounded = hitGameObj.layer ==  groundLayer;
            if (isGrounded)
            {
                currentSpawnPos = hit.point;
            }
        }

        return isGrounded;
    }

    public void DestroyAllGeneratedObjs()
    {
        foreach (var obj in generatedObjs)
        {
            if (obj)
            {
                DestroyImmediate(obj);
            }   
        }

        generatedObjs = null;
    }
}
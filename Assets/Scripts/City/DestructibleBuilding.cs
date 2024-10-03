using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//TODO rework this script so it doesn't use Update
//This will probably only be used for the Pilot scene. 
public class DestructibleBuilding : MonoBehaviour {
    ThePilot the_pilot;
    private Vector3 origPos;
    
    public int health;
    public int healthMultiplier;

    public int segments;

    public float totalHeight;

    public bool falling = true;

    Vector3 nextPos;
    public float fallSpeed;

    MeshRenderer buildingMesh;

    [Header("Deity Reactions")]
    EffectsManager effectsMan;
    private DeityManager _deityManager;
    public GameObject explosionPrefab;
    ParticleSystem explosionParticles;
    public GameObject smokePrefab;
    ParticleSystem smokeParticles;

    public const string pilot = "3_Ramins Takeoff";
    public const string nuclearity = "5_Nuclearity";
    private Transform expParent;

    void Awake()
    {
        the_pilot = FindObjectOfType<ThePilot>();
        buildingMesh = GetComponentInChildren<MeshRenderer>();
        effectsMan = FindObjectOfType<EffectsManager>();
        //_deityManager = FindObjectOfType<DeityManager>();
        origPos = transform.localPosition;
        //set smoke effect
        if (smokePrefab == null)
        {
            smokePrefab = effectsMan.smokePrefab;
        }

        //set explosion effect
        if (explosionPrefab == null)
        {
            explosionPrefab = effectsMan.deityExplosionPrefab;
        }

        if (expParent == null)
        {
            expParent = GameObject.FindGameObjectWithTag("ExpParent").transform;
        }
    }

    private void OnEnable()
    {
        //only generate these effects in the pilot scene. 
        if (SceneManager.GetActiveScene().name == pilot)
        {
            Reset();
        }
    }

    void Start ()
    {
        GenerateEffects();

        totalHeight = buildingMesh.bounds.extents.y * 2;
        health = segments * healthMultiplier;
    }
    
    void GenerateEffects()
    {
        //only generate these effects in the pilot scene. 
        if (SceneManager.GetActiveScene().name == pilot)
        {
            //instantiate and parent smoke to me, get particle
            GameObject smoke = Instantiate(smokePrefab, transform);
            smokeParticles = smoke.GetComponent<ParticleSystem>();

            //instantiate and parent explosion to me, get particle
            GameObject explosion = Instantiate(explosionPrefab, transform);
            explosionParticles = explosion.GetComponent<ParticleSystem>();
        }
        else if (SceneManager.GetActiveScene().name == nuclearity)
        {
            //instantiate and parent explosion to me, get particle
            GameObject explosion = Instantiate(explosionPrefab, transform);
            explosionParticles = explosion.GetComponent<ParticleSystem>();
        }
    }
	
	void Update ()
    {
        //controls falling
        if (falling)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPos, fallSpeed * Time.deltaTime);

            if(Vector3.Distance(transform.position, nextPos) < 0.25f)
            {
                falling = false;
            }
        }
	}

    //when a bomb hits
    void OnTriggerEnter(Collider other)
    {
        //US bombing 
        if(other.gameObject.CompareTag("Bomb"))
        {
            //Debug.Log("ouch");
            health--;

            if(health % healthMultiplier == 0)
            {
                Fall();
            }
        }
       
        //immediately explode 
        if(other.gameObject.CompareTag("Deity") && health > 0)
        {
            //play efx
            explosionParticles.Play();
            smokeParticles.Play();
            //play from d sound 
            DeitySound dSound = other.transform.parent.GetComponentInChildren<DeitySound>();
            dSound.PlaySoundMultipleAudioSources(dSound.explosionSounds);
            
            //fall and set 0 hp
            health = 0;
        }

        //destroyed in nuclearity 
        if (other.gameObject.CompareTag("Explosion") && SceneManager.GetActiveScene().name == nuclearity)
        {
            explosionParticles.transform.SetParent(expParent);
            explosionParticles.Play();
            gameObject.SetActive(false);
        }
    }

    //called to set next fall pos
    void Fall()
    {
        nextPos = transform.position - new Vector3(0, totalHeight / segments, 0);
        falling = true;
        //Debug.Log("falling");
    }

    void FallBelow()
    {
        nextPos = transform.position - new Vector3(0, totalHeight * 3, 0);
        falling = true;
        //Debug.Log("falling");
    }

    private void Reset()
    {
        transform.localPosition = new Vector3(transform.localPosition.x, origPos.y, transform.localPosition.z);
        explosionParticles.Stop();
        smokeParticles.Stop();
        falling = false;
    }
}

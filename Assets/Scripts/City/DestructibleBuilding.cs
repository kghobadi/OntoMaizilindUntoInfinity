using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestructibleBuilding : MonoBehaviour {
    ThePilot the_pilot;
    
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

    void Awake()
    {
        the_pilot = FindObjectOfType<ThePilot>();
        buildingMesh = GetComponentInChildren<MeshRenderer>();
        effectsMan = FindObjectOfType<EffectsManager>();
        //_deityManager = FindObjectOfType<DeityManager>();

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
    }

    void GenerateEffects()
    {
        //instantiate and parent smoke to me, get particle
        GameObject smoke = Instantiate(smokePrefab, transform);
        smokeParticles = smoke.GetComponent<ParticleSystem>();

        //instantiate and parent explosion to me, get particle
        GameObject explosion = Instantiate(explosionPrefab, transform);
        explosionParticles = explosion.GetComponent<ParticleSystem>();
    }

    void Start ()
    {
        GenerateEffects();

        totalHeight = buildingMesh.bounds.extents.y * 2;
        health = segments * healthMultiplier;
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

        //pilot is in the scene 
        if(the_pilot != null)
        {
            //disable when i am behind the pilot 
            if(transform.position.z < (the_pilot.transform.position.z - 25f))
            {
                gameObject.SetActive(false);
            }
        }
	}

    //when a bomb hits
    void OnTriggerEnter(Collider other)
    {
        //US bombing 
        if(other.gameObject.tag == "Bomb")
        {
            //Debug.Log("ouch");
            health--;

            if(health % healthMultiplier == 0)
            {
                Fall();
            }
        }
       
        //immediately explode 
        if(other.gameObject.tag == "Deity" && health > 0)
        {
            //play efx
            explosionParticles.Play();
            smokeParticles.Play();
            //play from d sound 
            DeitySound dSound = other.transform.parent.GetComponentInChildren<DeitySound>();
            dSound.PlaySoundMultipleAudioSources(dSound.explosionSounds);
            
            //fall and set 0 hp
            FallBelow();
            health = 0;
        }

        //destroyed in nuclearity 
        if (other.gameObject.tag == "Explosion" && SceneManager.GetActiveScene().buildIndex == 3)
        {
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
}

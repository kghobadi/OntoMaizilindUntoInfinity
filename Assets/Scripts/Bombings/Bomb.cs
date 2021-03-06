﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//bombs are generated when player presses space while controlling planes
public class Bomb : MonoBehaviour {
    //world manager ref
    WorldManager worldMan;
    EffectsManager effectsMan;
    CameraSwitcher camSwitcher;

    //physics vars
    SphereCollider bombCol;
    Rigidbody bombBody;
    PooledObject _pooledObj;
    public float moveSpeedOverTime;
    public float startSpeed = 10000f;

    //audio vars
    AudioSource bombAudio;
    public AudioClip[] bombfalls;

    //explosion prefab
    public GameObject explosionPrefab;
    public Transform explosionParent;

    //only for kill player bomb
    [HideInInspector] public MoveTowards moveTowards;
    [HideInInspector] public Transform playerDest;

    void Awake()
    {
        //world man and add to list
        worldMan = FindObjectOfType<WorldManager>();
        effectsMan = FindObjectOfType<EffectsManager>();
        camSwitcher = worldMan.GetComponent<CameraSwitcher>();

        //get comp refs
        bombCol = GetComponent<SphereCollider>();
        bombBody = GetComponent<Rigidbody>();
        bombAudio = GetComponent<AudioSource>();

        //set parent for explosion
        explosionParent = GameObject.FindGameObjectWithTag("ExpParent").transform;
    }

    void Start () {
        //pooled 
        _pooledObj = GetComponent<PooledObject>();

        //set fall sound
        int randomFall = Random.Range(0, bombfalls.Length);
        bombAudio.clip = bombfalls[randomFall];
        bombAudio.Play();
	}

    //called by bomber
    public void SetBombFall()
    {
        bombBody.isKinematic = false;
        bombBody.AddForce(0f, startSpeed, 0f);
    }

    //fall force 
	void FixedUpdate ()
    {
        bombBody.AddForce(0, -moveSpeedOverTime, 0);

        //we have a movetowards
        if (moveTowards)
        {
            if(moveTowards.enabled)
                moveTowards.MoveTo(playerDest.position, 500f);
        }

        //y check 
        if(transform.position.y < 0f)
        {
            SpawnExplosion(null);

            ResetBomb();
        }
	}
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Building" || other.gameObject.tag == "Ground" || other.gameObject.tag == "Car" || other.gameObject.tag == "Human" || other.gameObject.tag == "Player") 
        {
            SpawnExplosion(other.gameObject);

            ResetBomb();
        } 
    }

    void SpawnExplosion(GameObject obj)
    {
        //Debug.Log("bomb went off");
        Vector3 spawnPos = transform.position;
        GameObject explosion = effectsMan.explosionPooler.GrabObject();
        //set pos & angle & parent
        explosion.transform.position = spawnPos;
        explosion.transform.localEulerAngles = new Vector3(-90, 0, 0);
        explosion.transform.SetParent(explosionParent); 

        if(obj != null)
        {
            //parent to building so when it falls, explosion falls with it
            if (obj.tag == "Building")
            {
                explosion.transform.SetParent(obj.transform);
            }
        }
    }

    void ResetBomb()
    {
        //check for moveTowarsd
        if (moveTowards)
        {
            moveTowards.enabled = false;
        }
        //zero velocity
        bombBody.velocity = Vector3.zero;
        //disable forces 
        bombBody.isKinematic = true;
        //return to pool
        _pooledObj.ReturnToPool();
    }
}

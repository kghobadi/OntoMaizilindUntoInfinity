﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    ThePilot pilot;
    public float bulletSpeed;
    public float speedOverTime = 10f;
    public float origSpeed;

    public Vector3 shotPos;
    public float shotDist = 500f;

    void Awake()
    {
        pilot = GameObject.FindGameObjectWithTag("Player").GetComponent<ThePilot>();
        origSpeed = bulletSpeed;
    }

    void Update () {
        //move forward on Z axis 
        transform.position = Vector3.MoveTowards(transform.position, shotPos + new Vector3(0, 0, shotDist + 1f), bulletSpeed * Time.deltaTime);
        bulletSpeed += speedOverTime;

        //return to pool once it has traveled too far 
        if(Vector3.Distance(transform.position, pilot.transform.position) > shotDist)
        {
            ResetBullet();
        }
	}

    void OnTriggerEnter(Collider other)
    {
        //return bullet and death cloud to their pools on impact 
        if(other.gameObject.tag == "DeathCloud")
        {
            //reset cloud scale && send to poolers
            other.gameObject.transform.localScale = other.gameObject.GetComponent<Cloud>().origScale;
            other.gameObject.GetComponent<PooledObject>().ReturnToPool();
            ResetBullet();
        }
    }

    void ResetBullet()
    {
        GetComponent<PooledObject>().ReturnToPool();
        bulletSpeed = origSpeed;
    }
}
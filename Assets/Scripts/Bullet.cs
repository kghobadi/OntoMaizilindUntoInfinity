using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    ThePilot pilot;
    public float bulletSpeed;
    public float origSpeed;

    void Awake()
    {
        pilot = GameObject.FindGameObjectWithTag("Player").GetComponent<ThePilot>();
        origSpeed = bulletSpeed;
    }

    void Update () {
        //move forward on Z axis 
        transform.position = Vector3.MoveTowards(transform.position, transform.position + new Vector3(0, 0, 10), bulletSpeed * Time.deltaTime);
        bulletSpeed += 1f;

        //return to pool once it has traveled too far 
        if(Vector3.Distance(transform.position, pilot.transform.position) > 250f)
        {
            GetComponent<PooledObject>().ReturnToPool();
        }
	}

    void OnTriggerEnter(Collider other)
    {
        //return bullet and death cloud to their pools on impact 
        if(other.gameObject.tag == "DeathCloud")
        {
            bulletSpeed = origSpeed;
            other.gameObject.GetComponent<PooledObject>().ReturnToPool();
            GetComponent<PooledObject>().ReturnToPool();
            Debug.Log("hit cloud");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//this script alters time as you approach an object.
public class TimeAlter : MonoBehaviour
{
    Transform player;

    public float alterSpeed = 0.1f;

    float dist;
    float startDist;

    public bool increase; 

	void Awake ()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;	
	}

    void Start()
    {
        startDist = Vector3.Distance(transform.position, player.position);
    }

    void Update ()
    {
        dist = Vector3.Distance(transform.position, player.position);

        float difference = Mathf.Abs(startDist - dist);

        // i think this value needs to be lerped back and forth between start dist and min dist
	}
}

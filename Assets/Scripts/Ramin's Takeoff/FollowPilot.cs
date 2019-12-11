using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPilot : MonoBehaviour {
    public GameObject pilot;
    public float xDistAtStart;
    public float yDistAtStart;
    public bool followY;

	void Start () {
        xDistAtStart =  transform.position.x - pilot.transform.position.x;
        yDistAtStart = transform.position.y - pilot.transform.position.y;
    }
	
	void Update ()
    {
        if (followY)
        {
            transform.position = new Vector3(pilot.transform.position.x + xDistAtStart, pilot.transform.position.y + yDistAtStart, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(pilot.transform.position.x + xDistAtStart, transform.position.y, transform.position.z);
        }
	}
}

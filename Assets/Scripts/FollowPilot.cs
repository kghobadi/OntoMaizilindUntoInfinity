using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPilot : MonoBehaviour {
    public GameObject pilot;
    public float xDistAtStart;
	void Start () {
        xDistAtStart =  transform.position.x - pilot.transform.position.x;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(pilot.transform.position.x + xDistAtStart, transform.position.y, transform.position.z);
	}
}

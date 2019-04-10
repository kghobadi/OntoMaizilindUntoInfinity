using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleBuilding : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Bomb" || collision.gameObject.tag == "Explosion")
        {
            Debug.Log("ouch");
        }
    }
}

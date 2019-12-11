using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleBuilding : MonoBehaviour {

    public int health;
    public int healthMultiplier;

    public int segments;

    public float totalHeight;

    public bool falling = true;

    Vector3 nextPos;
    public float fallSpeed;

    MeshRenderer buildingMesh;
    
	void Start () {
        buildingMesh = GetComponentInChildren<MeshRenderer>();
        totalHeight = buildingMesh.bounds.extents.y * 2;
        health = segments * healthMultiplier;
    }
	
    //controls falling
	void Update () {
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
        if(other.gameObject.tag == "Bomb")
        {
            Debug.Log("ouch");
            health--;

            if(health % healthMultiplier == 0)
            {
                Fall();
            }
        }
    }

    //called to set next fall pos
    void Fall()
    {
        nextPos = transform.position - new Vector3(0, totalHeight / segments, 0);
        falling = true;
        Debug.Log("falling");
    }
}

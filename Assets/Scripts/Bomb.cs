using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//bombs are generated when player presses space while controlling planes
public class Bomb : MonoBehaviour {
    //world manager ref
    WorldManager worldMan;
    CameraSwitcher camSwitcher;

    //physics vars
    SphereCollider bombCol;
    Rigidbody bombBody;
    public float moveSpeedOverTime;

    //audio vars
    AudioSource bombAudio;
    public AudioClip[] bombfalls;

    //explosion prefab
    public GameObject explosionPrefab;
    public Transform explosionParent;

	void Start () {
        //world man and add to list
        worldMan = GameObject.FindGameObjectWithTag("WorldManager").GetComponent<WorldManager>();
        camSwitcher = worldMan.GetComponent<CameraSwitcher>();

        //get comp refs
        bombCol = GetComponent<SphereCollider>();
        bombBody = GetComponent<Rigidbody>();
        bombAudio = GetComponent<AudioSource>();

        //set fall sound
        int randomFall = Random.Range(0, bombfalls.Length);
        bombAudio.clip = bombfalls[randomFall];
        bombAudio.Play();

        //set parent for explosion
        explosionParent = GameObject.FindGameObjectWithTag("ExpParent").transform;
	}

	void Update () {
        bombBody.AddForce(0, -moveSpeedOverTime, 0);
	}


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Building" || other.gameObject.tag == "Ground" || other.gameObject.tag == "Car" || other.gameObject.tag == "Human")
        {
            //Debug.Log("bomb went off");
            Vector3 spawnPos = transform.position ;
            GameObject explosion = Instantiate(explosionPrefab, spawnPos, Quaternion.Euler(-90, 0, 0), explosionParent);

            //destroy this bomb
            Destroy(gameObject);
        } 
    }

    //should create an object pool in each plane to spawn bombs from
    //when they hit the ground just teleport them back inside the plane, rather than destroy/instantiate
}

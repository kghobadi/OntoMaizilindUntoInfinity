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

        bombBody.AddForce(0, -1500, 0);
	}

	void Update () {
        bombBody.AddForce(0, -moveSpeedOverTime, 0);
	}


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Building" || other.gameObject.tag == "Ground" || other.gameObject.tag == "Car" || other.gameObject.tag == "Human")
        {
            Debug.Log("bomb went off");
            Vector3 spawnPos = transform.position ;
            GameObject explosion = Instantiate(explosionPrefab, spawnPos, Quaternion.Euler(-90, 0, 0));

            //kill a human
            if(other.gameObject.tag == "Human")
            {
                //if this is the human currently being played
                if(camSwitcher.cameraObjects[camSwitcher.currentCam] == other.gameObject)
                {
                    //switch to next viewer
                    camSwitcher.SwitchCam(true, -1);
                }

                //remove this human from cam objects list
                camSwitcher.cameraObjects.Remove(other.gameObject);   

                //destroy the human
                Destroy(other.gameObject);
            }

            //destroy this bomb
            Destroy(gameObject);
        } 
    }

    //should create an object pool in each plane to spawn bombs from
    //when they hit the ground just teleport them back inside the plane, rather than destroy/instantiate
}

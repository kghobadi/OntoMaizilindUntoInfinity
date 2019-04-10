using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
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
        if (other.gameObject.tag == "Building" || other.gameObject.tag == "Ground")
        {
            Debug.Log("bomb went off");
            Vector3 spawnPos = transform.position ;
            GameObject explosion = Instantiate(explosionPrefab, spawnPos, Quaternion.identity);

            Destroy(gameObject);
        } 
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestructibleBuilding : MonoBehaviour {
    

    public int health;
    public int healthMultiplier;

    public int segments;

    public float totalHeight;

    public bool falling = true;

    Vector3 nextPos;
    public float fallSpeed;

    MeshRenderer buildingMesh;

    [Header("Deity Reactions")]
    public ParticleSystem explosionParticles;
    public ParticleSystem smokeParticles;
    
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
        //US bombing 
        if(other.gameObject.tag == "Bomb")
        {
            Debug.Log("ouch");
            health--;

            if(health % healthMultiplier == 0)
            {
                Fall();
            }
        }
       
        //immediately explode 
        if(other.tag == "Deity" && health > 0)
        {
            //play efx
            explosionParticles.Play();
            smokeParticles.Play();
            //play from d sound 
            DeitySound dSound = FindObjectOfType<DeitySound>();
            dSound.PlaySoundMultipleAudioSources(dSound.explosionSounds);
            //fall and set 0 hp
            Fall();
            health = 0;
        }

        //destroyed in nuclearity 
        if (other.gameObject.tag == "Explosion" && SceneManager.GetActiveScene().buildIndex == 3)
        {
            gameObject.SetActive(false);
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

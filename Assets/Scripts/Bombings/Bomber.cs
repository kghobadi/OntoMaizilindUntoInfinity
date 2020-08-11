using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class Bomber : MonoBehaviour {
    EffectsManager effectsMan;

    public GameObject bombPrefab;
    public bool bombing;

    public float bombInterval = 0.45f;
    public float spawnRadius = 25f;
    public int bombMin = 5, bombMax = 15;
    ////int bombsToDrop;

	void Awake ()
    {
        effectsMan = FindObjectOfType<EffectsManager>();
	}
	
	void Update () {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        if (Input.GetKeyDown(KeyCode.Space) || inputDevice.Action1.WasPressed)
        {
            StartCoroutine(SpawnBombs());
        }
	}

    //public bomb call 
    public void DropBombs()
    {
        StartCoroutine(SpawnBombs());
    }

    //spawn a random count of bombs to drop
    IEnumerator SpawnBombs()
    {
        bombing = true;

        int randomBcount = Random.Range(bombMin, bombMax);

        //Debug.Log("Spawning " + randomBcount + " bombs");

        for(int i = 0; i < randomBcount; i++)
        {
            DropBomb();
            //wait
            yield return new WaitForSeconds(bombInterval);
        }

        bombing = false;
    }

    void DropBomb()
    {
        //find spawn pos and grab obj 
        Vector3 spawnPos = transform.position - new Vector3(0, 7, 0) + Random.insideUnitSphere * spawnRadius;
        GameObject bomb =  effectsMan.bombPooler.GrabObject();
        //set pos 
        bomb.transform.position = spawnPos;
        //enable force
        Bomb bombScript = bomb.GetComponent<Bomb>();
        bombScript.SetBombFall();
    }
}

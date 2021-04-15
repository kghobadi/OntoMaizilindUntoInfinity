using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public class Bomber : MonoBehaviour {
    EffectsManager effectsMan;
    CameraSwitcher camSwitcher;

    public bool captain;
    public GameObject bombPrefab;
    public bool bombing;

    public float bombInterval = 0.45f;
    public float spawnRadius = 25f;
    public int bombMin = 5, bombMax = 15;
    ////int bombsToDrop;

	void Awake ()
    {
        effectsMan = FindObjectOfType<EffectsManager>();
        camSwitcher = FindObjectOfType<CameraSwitcher>();
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

    //drops a bomb below plane 
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

    //spawn bombs directly above player location
    public void KillPlayer()
    {
        //get player transform 
        Transform playerT = camSwitcher.currentPlayer.transform;

        //find spawn pos and grab obj 
        Vector3 spawnPos = new Vector3(playerT.position.x, transform.position.y, playerT.position.z)
            + Random.insideUnitSphere * spawnRadius / 3;
        GameObject bomb = effectsMan.bombPooler.GrabObject();
        //set pos 
        bomb.transform.position = spawnPos;
        //enable force
        Bomb bombScript = bomb.GetComponent<Bomb>();
        bombScript.SetBombFall();

        //set move towards comp
        MoveTowards moveTo = bomb.GetComponent<MoveTowards>();
        if (moveTo == null)
        {
            //add move towards
            moveTo = bomb.AddComponent<MoveTowards>();
            moveTo.MoveTo(playerT.position, 500f);
            //set homing missle hehe 
            bombScript.moveTowards = moveTo;
            bombScript.playerDest = playerT;
        }
        //already has it, just enable/set 
        else
        {
            moveTo.enabled = true;
            moveTo.MoveTo(playerT.position, 500f);
        }
    }
    
    //spawn bombs directly above parents location
    public void KillParents()
    {
        Transform mom = camSwitcher.dad;
        //find spawn pos and grab obj 
        Vector3 spawnPos = new Vector3(mom.position.x, transform.position.y, mom.position.z)
                           + Random.insideUnitSphere * spawnRadius / 3;
        GameObject bomb = effectsMan.bombPooler.GrabObject();
        //set pos 
        bomb.transform.position = spawnPos;
        //enable force
        Bomb bombScript = bomb.GetComponent<Bomb>();
        bombScript.SetBombFall();

        //set move towards comp
        MoveTowards moveTo = bomb.GetComponent<MoveTowards>();
        if (moveTo == null)
        {
            //add move towards
            moveTo = bomb.AddComponent<MoveTowards>();
            moveTo.MoveTo(mom.position, 500f);
            //set homing missle hehe 
            bombScript.moveTowards = moveTo;
            bombScript.playerDest = mom;
        }
        //already has it, just enable/set 
        else
        {
            moveTo.enabled = true;
            moveTo.MoveTo(mom.position, 500f);
        }
    }
}

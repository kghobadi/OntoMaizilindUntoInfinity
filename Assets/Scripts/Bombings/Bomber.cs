using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

/// <summary>
/// Controls the bomber planes and spawning of bombs. 
/// </summary>
public class Bomber : MonoBehaviour {
    EffectsManager effectsMan;
    CameraSwitcher camSwitcher;

    public bool captain;
    public GameObject bombPrefab;
    public bool bombing;

    [SerializeField] private int bombsToDrop = 2;
    public float bombInterval = 0.45f;
    public float spawnRadius = 25f;
    public int bombMin = 5, bombMax = 15;

	void Awake ()
    {
        effectsMan = FindObjectOfType<EffectsManager>();
        camSwitcher = FindObjectOfType<CameraSwitcher>();
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
        //set bombs left
        int bombsLeft = randomBcount;
        for(int i = 0; i < randomBcount; i++)
        {
            if (bombsLeft > 0)
            {
                for (int b = 0; b < bombsToDrop; b++)
                {
                    DropBomb();
                    bombsLeft--;
                }
            }
            //wait
            yield return new WaitForSeconds(bombInterval);
        }

        bombing = false;
    }

    //drops a bomb below plane 
    void DropBomb()
    {
        //find spawn pos and grab obj 
        Vector3 randomInsideSphere = (Random.insideUnitSphere * spawnRadius);
        Vector3 spawnPos = transform.position - new Vector3(0, 7, 0) + randomInsideSphere;
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
            moveTo.MoveTo(playerT, 500f);
            //set homing missle hehe 
            bombScript.moveTowards = moveTo;
            bombScript.playerDest = playerT;
        }
        //already has it, just enable/set 
        else
        {
            moveTo.enabled = true;
            moveTo.MoveTo(playerT, 500f);
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
            moveTo.MoveTo(mom, 500f);
            //set homing missle hehe 
            bombScript.moveTowards = moveTo;
            bombScript.playerDest = mom;
        }
        //already has it, just enable/set 
        else
        {
            moveTo.enabled = true;
            moveTo.MoveTo(mom, 500f);
        }
    }
}

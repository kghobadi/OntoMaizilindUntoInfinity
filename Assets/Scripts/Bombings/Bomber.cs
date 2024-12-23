﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

/// <summary>
/// Controls the bomber planes and spawning of bombs. 
/// </summary>
public class Bomber : MonoBehaviour 
{
    EffectsManager effectsMan;
    CameraSwitcher camSwitcher;

    public bool captain;
    public bool transitionBomb;
    public GameObject bombPrefab;
    [SerializeField] GameObject trackerBombPrefab; //special for killing player/parents 
    [SerializeField] GameObject transitionBombPrefab;
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

    /// <summary>
    /// Public bomb call 
    /// </summary>
    public void DropBombs()
    {
        StartCoroutine(SpawnBombs());
    }

    /// <summary>
    /// Spawn a random count of bombs to drop
    /// </summary>
    /// <returns></returns>
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
                    //Final bomb of captain should be transition bomb!
                    if (b == bombsToDrop - 1 && captain && transitionBomb)
                    {
                        DropTransitionBomb();
                    }
                    else
                    {
                        DropBomb();
                    }
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

        //kill player 
        DropTrackingBomb(playerT);
    }
    
    //spawn bombs directly above parents location
    public void KillParents()
    {
        //get dad 
        Transform dad = camSwitcher.dad;
        //Drop bomb
        DropTrackingBomb(dad);
    }
    
    /// <summary>
    /// Drops a tracking bomb. 
    /// </summary>
    /// <param name="objToTrack"></param>
    public void  DropTrackingBomb(Transform objToTrack)
    {
        //find spawn pos and grab obj 
        Vector3 spawnPos = new Vector3(objToTrack.position.x, transform.position.y, objToTrack.position.z)
                           + Random.insideUnitSphere * spawnRadius / 3;
        //instantiate tracking bomb
        GameObject bomb = Instantiate(trackerBombPrefab, spawnPos, Quaternion.identity);
        //enable force
        Bomb bombScript = bomb.GetComponent<Bomb>();
        bombScript.SetBombFall();

        //set move towards comp
        MoveTowards moveTo = bomb.GetComponent<MoveTowards>();
        //set homing missile 
        bombScript.moveTowards = moveTo;
        bombScript.playerDest = objToTrack;
        moveTo.MoveTo(objToTrack, 500f);
    }
    
    /// <summary>
    /// Drops a tracking bomb. 
    /// </summary>
    public void  DropTransitionBomb()
    {
        //find spawn pos and grab obj 
        Vector3 randomInsideSphere = (Random.insideUnitSphere * spawnRadius);
        Vector3 spawnPos = transform.position - new Vector3(0, 7, 0) + randomInsideSphere;
        //instantiate tracking bomb
        GameObject bomb = Instantiate(transitionBombPrefab, spawnPos, Quaternion.identity);
        //enable force
        Bomb bombScript = bomb.GetComponent<Bomb>();
        bombScript.SetBombFall();
        
        //add cam to list and enable it as The One
        camSwitcher.AddCamObject(bombScript.CamObj);
        camSwitcher.SetCam(bombScript.CamObj);
        //disable trans
        transitionBomb = false;
    }
}

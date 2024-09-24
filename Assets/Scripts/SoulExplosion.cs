using System;
using System.Collections;
using System.Collections.Generic;
using NPC;
using UnityEngine;
using UnityEngine.Video;

public class SoulExplosion : MonoBehaviour
{
    public TrainEntranceTrigger[] allTrainTriggers;
    public List<Movement> npcMovers = new List<Movement>();
    public bool playerEntered;
    public Transform playerCamera;
    public bool allTrainsFull;

    public GameObject soulTrailPrefab;

    private LerpScale[] sphereExplosions;
    
    public bool soulsExploded;

    public AudioSource music;

    public Orbit trainOrbit;

    private void Start()
    {
        sphereExplosions = GetComponentsInChildren<LerpScale>();

        //disable the spheres 
        foreach (var sphere in sphereExplosions)
        {
            sphere.gameObject.SetActive(false);
        }
    }

    public void SetPlayerEntered()
    {
        playerEntered = true;
    }

    public void CheckAllTrainsFull()
    {
        for(int i = 0; i < allTrainTriggers.Length; i++)
        {
            if (allTrainTriggers[i].hasSeats)
            {
                return;
            }
        }

        allTrainsFull = true;

        if (playerEntered && !soulsExploded)
        {
            BeginSoulExplosion();
        }
    }

    void BeginSoulExplosion()
    {
        //already called -- do nothing.
        if (soulsExploded)
        {
            return;
        }
        
        Debug.Log("Beginning soul explosion");

        //loop through all NPCs
        foreach (var npc in npcMovers)
        {
            //instantiate 
            GameObject soulTrail = Instantiate(soulTrailPrefab, transform.position, Quaternion.identity);
            //get spirit trail script 
            SpiritTrail spirit = soulTrail.GetComponent<SpiritTrail>();
            //get npc face transform.
            Transform faceTransform = npc.GetComponent<Sounds>().FaceAnimation.faceShiftTrigger.transform;
            //set spirit to face
            spirit.projectionDisplayCorner = faceTransform;
            //enable spirit
            spirit.ProjectTrail();
        }
        
        //Spirit trail for player.
        //instantiate 
        GameObject trail = Instantiate(soulTrailPrefab, transform.position, Quaternion.identity);
        //get spirit trail script 
        SpiritTrail soul = trail.GetComponent<SpiritTrail>();
        //get npc face transform.
        Transform playerFace = playerCamera.transform;
        //set spirit to face
        soul.projectionDisplayCorner = playerFace;
        //enable spirit
        soul.ProjectTrail();

        //begin all sphere explosions 
        foreach (var sphereExplosion in sphereExplosions)
        {
            sphereExplosion.gameObject.SetActive(true);
            sphereExplosion.SetLerp();
        }
        
        //music should not loop. 
        music.loop = false;

        //set bool so this only happens once
        soulsExploded = true;
    }
}

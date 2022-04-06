using System.Collections;
using System.Collections.Generic;
using NPC;
using UnityEngine;

public class SoulExplosion : MonoBehaviour
{
    public TrainEntranceTrigger[] allTrainTriggers;
    public List<Movement> npcMovers = new List<Movement>();
    public bool playerEntered;
    public bool allTrainsFull;

    public GameObject soulTrailPrefab;
    
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

        if (playerEntered)
        {
            BeginSoulExplosion();
        }
    }

    public void BeginSoulExplosion()
    {
        Debug.Log("Beginning soul explosion");

        //loop through all NPCs
        foreach (var npc in npcMovers)
        {
            //instantiate 
            GameObject soulTrail = Instantiate(soulTrailPrefab, transform.position, Quaternion.identity);
            //get spirit trail script 
            SpiritTrail spirit = soulTrail.GetComponent<SpiritTrail>();
            //get npc face transform.
            Transform faceTransform = npc.GetComponent<Sounds>().FaceAnimation.transform;
            //set spirit to face
            spirit.projectionDisplayCorner = faceTransform;
            //enable spirit
            spirit.EnableSpirit();
        }
    }
}

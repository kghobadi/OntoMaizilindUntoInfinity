using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// TODO this script will change as we make this sequence more on rails/linear and less 'open-ended'.
/// Each Deity will appear in a certain order and we can control the timing of the landscape with RailLandscape calls. 
/// Use the DomeInterior objects as a sort of lure that exists between Deities. 
/// Ending should bring the final transformation to Deity of Destruction so ending makes sense. 
/// </summary>
public class DeityManager : MonoBehaviour {
    public List<DeityHealth> deities = new List<DeityHealth>();
    [SerializeField]
    private int currentDeity = 0;
    [SerializeField]
    private GameObject deityDome;

    public DeityHealth[] firstLayerDeities;
    public DeityHealth[] secondLayerDeities;

    public bool firstLayerDestroyed;
    public UnityEvent firstLayer;
    public bool secondLayerDestroyed;
    public UnityEvent secondLayer;
    public UnityEvent deityDied;

    private void OnEnable()
    {
        //add event listeners 
        deityDied.AddListener(OnDeityDied);
    }

    private void OnDisable()
    {
        //remove event listeners 
        deityDied.RemoveListener(OnDeityDied);
    }


    /// <summary>
    /// For spawning deities one at a time, in order. Generally should be called after hallucs finish. 
    /// </summary>
    public void SpawnDeity()
    {
        //set deity active
        deities[currentDeity].gameObject.SetActive(true);
        //set pos to match deityDome
        deities[currentDeity].transform.position = deityDome.transform.position;

        if(currentDeity < deities.Count - 1)
        {
            currentDeity++;
        }
        else
        {
            Debug.Log("That's all the deities!");
        }
    }

    /// <summary>
    /// Every time a deity dies, detect if a layer was defeated.
    /// </summary>
    void OnDeityDied()
    {
        //first layer destroyed 
        if(DetectDeitiesDestroyed(firstLayerDeities))
        {
            if(firstLayerDestroyed == false)
            {
                firstLayer.Invoke();
                firstLayerDestroyed = true;
            }
        }

        //second layer destroyed
        if(DetectDeitiesDestroyed(secondLayerDeities))
        {
            if (secondLayerDestroyed == false)
            {
                secondLayer.Invoke();
                secondLayerDestroyed = true;
            }
        }
    }

    public bool DetectDeitiesDestroyed(DeityHealth[] deityLayer)
    {
        foreach (var deity in deityLayer)
        {
            //if any of the deities in the layer are alive, return false.
            if (deity.healthState == DeityHealth.HealthStates.ALIVE)
            {
                return false;
            }
        }

        //if we made it thru the for loop, return true. 
        return true;
    }

    public void FreezeDeities()
    {
        for (int i = 0; i < deities.Count; i++)
        {
            deities[i].deity.FreezeMovement();
        }
    }
    
    public void ResumeDeities()
    {
        for (int i = 0; i < deities.Count; i++)
        {
            deities[i].deity.ResumeMovement();
        }
    }

    public void PlayHallucination(Hallucination hallucination)
    {
        //play it
    }

    //for channeling explosion sounds
    public void PlaySoundFromRandomDeity()
    {
        
    }

}

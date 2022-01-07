using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeityManager : MonoBehaviour {
    public List<DeityHealth> deities = new List<DeityHealth>();
    public DeityHealth[] firstLayerDeities;
    public DeityHealth[] secondLayerDeities;

    public bool firstLayerDestroyed;
    public UnityEvent firstLayer;
    public bool secondLayerDestroyed;
    public UnityEvent secondLayer;
    public UnityEvent deityDied;

    private void Start()
    {
        //add event listeners 
        deityDied.AddListener(OnDeityDied);
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

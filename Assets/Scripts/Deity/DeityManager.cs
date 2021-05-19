using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DeityManager : MonoBehaviour {

    public List<DeityHealth> deities = new List<DeityHealth>();

    public bool firstLayerDestroyed;
    public UnityEvent firstLayer;
    public bool secondLayerDestroyed;
    public UnityEvent secondLayer;

    private void Update()
    {
        //first layer destroyed 
        if(deities.Count < 5)
        {
            if(firstLayerDestroyed == false)
            {
                firstLayer.Invoke();
                firstLayerDestroyed = true;
            }
        }

        //second layer destroyed
        if (deities.Count < 2)
        {
            if (secondLayerDestroyed == false)
            {
                secondLayer.Invoke();
                secondLayerDestroyed = true;
            }
        }
    }

    //for channeling explosion sounds
    public void PlaySoundFromRandomDeity()
    {
        
    }

}

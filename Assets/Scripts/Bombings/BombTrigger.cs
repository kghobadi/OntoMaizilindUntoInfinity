using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombTrigger : MonoBehaviour {
    CameraSwitcher camSwitcher;
    BombShelter bombShelter;

    [Tooltip("Counts # of times the squadron passes the trigger")]
    public int bombingRuns = 0;
    [Tooltip("Frequency at which captain will call KillPlayer()")]
    public int killPlayerFreq = 3;

    private void Awake()
    {
        camSwitcher = FindObjectOfType<CameraSwitcher>();
        bombShelter = FindObjectOfType<BombShelter>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Plane")
        {
            Bomber bomber = other.gameObject.GetComponent<Bomber>();

            if (bomber.bombing == false)
            {
                bomber.DropBombs();
                Debug.Log("triggering bombs");

                //inc runs 
                if (bomber.captain)
                {
                    bombingRuns++;
                    
                    //should we kill the player? only if player is NOT the planes  && not entered mosque yet
                    if(bombingRuns % killPlayerFreq == 0 && camSwitcher.killedParents == false)
                    {
                        bomber.KillParents();
                    }

                    //should we kill the player? only if player is NOT the planes  && not entered mosque yet
                    if(bombingRuns % killPlayerFreq == 0 && camSwitcher.GetCurrentCamIndex() != 0 && !bombShelter.projecting && camSwitcher.killedParents)
                    {
                        //bomber.KillPlayer();
                    }

                    //we are the planes -- transition to anything else. 
                    if (camSwitcher.GetCurrentCamIndex() == 0)
                    {
                        //enough people to transition back to running
                        if (camSwitcher.cameraObjects.Count > camSwitcher.transitionAmount)
                        {
                            camSwitcher.SwitchCam(true);
                        }
                        //out of people, transition directly to mosque view & begin projection    
                        else
                        {
                            bombShelter.BeginProjection(false);
                        }

                        //shouldnt count this for runs -- gives player more time 
                        bombingRuns--;
                    }
                }
            }
        }
    }
}

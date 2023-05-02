using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages overall behavior of the bomber squadron.
/// </summary>
public class BombSquadron : MonoBehaviour 
{
    CameraSwitcher camSwitcher;
    BombShelter bombShelter;
    private Bomber[] allBombers;

    [Tooltip("Counts # of times the squadron passes the trigger")]
    public int bombingRuns = 0;
    [Tooltip("Frequency at which captain will call KillPlayer()")]
    public int killPlayerFreq = 3;
    
    [Tooltip("Range from which we pick a rotation value to apply to parent Y rotation so we vary the approaches of the squad.")]
    public Vector2 squadRotationRange = new Vector2(60f, 90f);

    [SerializeField] private EventTrigger [] peopleSpawners;
    private int peopleSpawned = 0;

    private void Awake()
    {
        camSwitcher = FindObjectOfType<CameraSwitcher>();
        bombShelter = FindObjectOfType<BombShelter>();
        allBombers = FindObjectsOfType<Bomber>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Plane")
        {
            Bomber bomberino = other.gameObject.GetComponent<Bomber>();
            
            BombingRun(bomberino);
        }
    }

    /// <summary>
    /// All bombers start bombing run. Can be called by anim event. 
    /// </summary>
    public void FullSquadronBombing()
    {
        foreach (var bomber in allBombers)
        {
            BombingRun(bomber);
        }
    }

    void BombingRun(Bomber bomber)
    {
        if (bomber.bombing == false)
        {
            bomber.DropBombs();
            //Debug.Log("triggering bombs");

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
                else if(bombingRuns % killPlayerFreq == 0 && camSwitcher.GetCurrentCamIndex() != 0 && (int)bombShelter.transitionState < 1
                        && peopleSpawned < peopleSpawners.Length - 1)
                {
                    //bomber.KillPlayer();
                }

                //we are the planes -- transition to anything else. 
                if (camSwitcher.GetCurrentCamIndex() == 0)
                {
                    //spawn people under me somewhere     
                    if (peopleSpawned < peopleSpawners.Length)
                    {
                        peopleSpawners[peopleSpawned].SetTrigger();
                        peopleSpawned++;
                    }
                    else
                    {
                        Debug.Log("No more people spawners.");
                    }
                    
                    //enough people to transition back to running as random person. 
                    if (camSwitcher.cameraObjects.Count > camSwitcher.transitionAmount)
                    {
                        camSwitcher.WaitSetRandomCam();
                    }
                    //out of people, transition directly to mosque view & begin projection    
                    else
                    {
                        bombShelter.BeginProjection(false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Called at end of anim to randomly rotate the bomb squadron direction. 
    /// </summary>
    public void RandomSquadRotation()
    {
        float randomRotation = Random.Range(squadRotationRange.x, squadRotationRange.y);
        transform.parent.Rotate(0f, randomRotation,0f);
    }
}

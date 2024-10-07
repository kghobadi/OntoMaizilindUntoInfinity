using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

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

    private bool bomberMode;
    private float becamePlanesTime = 0f;
    [Tooltip("Minimum time we should be planes before a transition down to people.")]
    public float timeAsPlanesMinimum = 5f;

    [Tooltip("Wait time to transition from Bombers to Citizen")]
    public float planeTransitionWait = 2f;

    [Tooltip("This object will be toggled on and off when you are/are not the bombers.")]
    public GameObject bomberModeView;

    /// <summary>
    /// Returns a random bomber transform. 
    /// </summary>
    public Transform GetRandomBomber
    {
        get
        {
            Transform bomberTrans = allBombers[0].transform;
            int random = Random.Range(0, allBombers.Length);
            
            bomberTrans = allBombers[random].transform;
            return bomberTrans;
        }
    }

    private void Awake()
    {
        camSwitcher = FindObjectOfType<CameraSwitcher>();
        bombShelter = FindObjectOfType<BombShelter>();
        allBombers = FindObjectsOfType<Bomber>();
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Plane"))
        {
            Bomber bomberino = other.gameObject.GetComponent<Bomber>();
            
            BombingRun(bomberino);
        }
    }

    /// <summary>
    /// Enables bomber mode related stuff
    /// </summary>
    public void EnableBomberMode()
    {
        if (bomberModeView)
        {
            bomberModeView.SetActive(true);
        }

        becamePlanesTime = Time.fixedTime;
        bomberMode = true;
    }

    /// <summary>
    /// Turns off bomber mode related stuff
    /// </summary>
    public void DisableBomberMode()
    {
        if (bomberModeView)
        {
            bomberModeView.SetActive(false);
        }
        bomberMode = false;
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
            //inc runs 
            if (bomber.captain)
            {
                bombingRuns++;
                    
                //should we kill the player? only if player is NOT the planes  && not entered mosque yet
                if(bombingRuns % killPlayerFreq == 0 && camSwitcher.killedParents == false)
                {
                    bomber.KillParents();
                }

                //we are the planes -- transition to anything else. 
                if (camSwitcher.GetCurrentCamIndex() == 0 && (Time.fixedTime - becamePlanesTime >= timeAsPlanesMinimum))
                {
                    //spawn people under me somewhere     
                    if (peopleSpawned < peopleSpawners.Length)
                    {
                        if (!peopleSpawners[peopleSpawned].hasTriggered)
                        {
                            peopleSpawners[peopleSpawned].SetTrigger();
                            peopleSpawned++;
                        }
                    }
                    else
                    {
                        Debug.Log("No more people spawners.");
                    }
                    
                    //enough people to transition back to running as random person. 
                    if (camSwitcher.cameraObjects.Count > camSwitcher.transitionAmount)
                    {
                        //Drop a transition bomb as this final round's bomb 
                        bomber.transitionBomb = true;
                    }
                    //out of people, transition directly to mosque view & begin projection    
                    else
                    {
                        bombShelter.BeginProjection(false);
                    }
                }
            }
            
            //Now bombs away!
            bomber.DropBombs();
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

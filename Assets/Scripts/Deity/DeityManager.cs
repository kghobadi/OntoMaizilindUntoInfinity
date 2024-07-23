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
public class DeityManager : MonoBehaviour
{
    private ThePilot pilot;
    [SerializeField] private RailMgr railMgr;
    [SerializeField]
    private List<Deity> deities = new List<Deity>();
    [SerializeField][Tooltip("For showing next deities in Hallucs")]
    private List<GameObject> deityVisions = new List<GameObject>();
    [SerializeField]
    private int currentDeity = 0;
    [SerializeField]
    private GameObject deityDome;

    public Deity CurrentDeity => deities[currentDeity - 1];

    public UnityEvent deityDied;

    private void OnEnable()
    {
        //add event listeners 
        //deityDied.AddListener(OnDeityDied);

        pilot = FindObjectOfType<ThePilot>();
    }

    private void OnDisable()
    {
        //remove event listeners 
        //deityDied.RemoveListener(OnDeityDied);
    }


    /// <summary>
    /// For spawning deities one at a time, in order. Generally should be called after hallucs finish. 
    /// </summary>
    public void SpawnDeity()
    {
        //Spawn wrath at the outset. We will helplessly watch it destroy the city. 
        if(currentDeity == 0)
        {
            deities[6].gameObject.SetActive(true);
            //set ramin dialogue
            pilot.PilotMonos.WaitToSetNewMonologue(0);
            //start counting bullets
            pilot.StartCountingBullets();
            //rails to city
            railMgr.SetPhase(2);
        }

        //Move the final deity towards the dome - quickly 
        if(currentDeity == 6)
        {
            deities[currentDeity].mover.MoveTo(deityDome.transform.position, 135f);
        }
        //Activate this deity at the dome position 
        else
        {
            //set deity active
            deities[currentDeity].gameObject.SetActive(true);
            //set pos to match deityDome
            deities[currentDeity].transform.position = deityDome.transform.position;
        }
      

        //Wait to disable the dome
        WaitToActivateDome(5f, false);

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
    /// Toggles on or off deity visions for Hallucs. 
    /// </summary>
    /// <param name="state"></param>
    public void ToggleDeityVisions(bool state)
    {
        for(int i = currentDeity; i < 6; i++)
        {
            deityVisions[i].SetActive(state);
        }
    }

    /// <summary>
    /// Waits to spawn a deity a given time. 
    /// </summary>
    /// <param name="time"></param>
    public void WaitToSpawnDeity(float time)
    {
        StartCoroutine(WaitForAction(time, SpawnDeity));
    }

    /// <summary>
    /// Wait an amount of time to activate/deactivate the deity dome. 
    /// </summary>
    /// <param name="time"></param>
    /// <param name="state"></param>
    public void WaitToActivateDome(float time, bool state)
    {
        //Wait to activate the dome
        StartCoroutine(WaitForAction(time, () => deityDome.gameObject.SetActive(state)));
    }

    /// <summary>
    /// Waits then performs an action. 
    /// </summary>
    /// <param name="wait"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    IEnumerator WaitForAction(float wait, Action action)
    {
        yield return new WaitForSeconds(wait);

        action.Invoke();
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
            deities[i].FreezeMovement();
        }
    }
    
    public void ResumeDeities()
    {
        for (int i = 0; i < deities.Count; i++)
        {
            deities[i].ResumeMovement();
        }
    }

    /// <summary>
    /// Removes a deity from our list. 
    /// </summary>
    /// <param name="deity"></param>
    public void RemoveDeity(Deity deity)
    {
        deities.Remove(deity);
    }
}

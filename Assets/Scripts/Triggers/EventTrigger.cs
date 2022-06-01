using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour {

    public bool hasTriggered;
    public bool playerOnly;
    public bool npcOnly;
    public bool specificObject;
    public GameObject specificObj;
    public UnityEvent[] events;

    [Header("Wait times")]
    public bool waits;
    public float waitTime = 5f;

    [Header("If trigger can activate repeatedly")]
    public bool repeats;
    public float resetTime = 5f;
    
    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered)
        {
            if (playerOnly)
            {
                if (other.gameObject.tag == "Player")
                {
                    SetTrigger();
                }
            }
            else if (npcOnly)
            {
                if (other.gameObject.tag == "Human")
                {
                    SetTrigger();
                }
            }
            else if (specificObject)
            {
                if (other.gameObject == specificObj)
                {
                    SetTrigger();
                }
            }
            else if(!playerOnly && !npcOnly && !specificObject)
            {
                if (other.gameObject.tag == "Human" || other.gameObject.tag == "Player")
                {
                    SetTrigger();
                }
            }
           
        }
    }

    public void SetTrigger()
    {
        if (hasTriggered)
        {
            return; 
        }
        
        if (waits)
        {
            StartCoroutine(WaitToTrigger());
        }
        else
        {
            Activate();
        }

        hasTriggered = true;
    }

    IEnumerator WaitToTrigger()
    {
        yield return new WaitForSeconds(waitTime);

        Activate();
    }

    void Activate()
    {
        //invoke the events
        for (int i = 0; i < events.Length; i++)
        {
            events[i].Invoke();
        }

        //call repeat if necessary 
        if (repeats)
            StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(resetTime);

        hasTriggered = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : MonoBehaviour {

    public bool hasTriggered;
    public UnityEvent[] events;

    [Header("If trigger can activate repeatedly")]
    public bool repeats;
    public float resetTime = 5f;

    void OnTriggerEnter(Collider other)
    {
        if (!hasTriggered)
        {
            if (other.gameObject.tag == "Human" || other.gameObject.tag == "Player")
            {
                SetTrigger();
            }
        }
    }

    void SetTrigger()
    {
        //invoke the events
        for(int i = 0; i < events.Length; i++)
        {
            events[i].Invoke();
        }

        hasTriggered = true;

        if (repeats)
            StartCoroutine(Reset());
    }

    IEnumerator Reset()
    {
        yield return new WaitForSeconds(resetTime);

        hasTriggered = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTrigger : TriggerBase 
{
    [Header("Event Settings")]
    public UnityEvent[] events;

    [Tooltip("Check this if you must have a count reached to trigger the events.")]
    [SerializeField] private bool requiresCount;
    [SerializeField] private int neededCount;
    [SerializeField] private int currentCount;
    
    public override void ActivateTriggerEffect()
    {
        //invoke the events
        for (int i = 0; i < events.Length; i++)
        {
            events[i].Invoke();
        }
        
        base.ActivateTriggerEffect();
    }

    /// <summary>
    /// Can be called by other methods and events. 
    /// </summary>
    /// <param name="amt"></param>
    public void AddToCount(int amt)
    {
        currentCount += amt;

        if (currentCount >= neededCount)
        {
            ActivateTriggerEffect();
        }
    }

}
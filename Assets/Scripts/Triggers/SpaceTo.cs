using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//script allows you to press Space to trigger events 
public class SpaceTo : MonoBehaviour {
    public UnityEvent[] events;
    public bool activated;
    
	
	void Update ()
    {
        if (!activated)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Activate();
            }
        }
	}

    public void Activate()
    {
        for (int i = 0; i < events.Length; i++)
        {
            events[i].Invoke();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using InControl;

//script allows you to press Space to trigger events 
public class SpaceTo : MonoBehaviour 
{
    public UnityEvent[] events;
    public bool activated;
    private InputDevice inputDevice;
    
	void Update ()
    {   
        inputDevice = InputManager.ActiveDevice;
        
        if (!activated)
        {
            if (Input.GetKeyDown(KeyCode.Space) || inputDevice.Action1.WasPressed)
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

        activated = true;
    }
}

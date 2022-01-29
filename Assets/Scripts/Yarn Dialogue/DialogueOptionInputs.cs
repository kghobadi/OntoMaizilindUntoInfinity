using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using UnityEngine.Events;

public class DialogueOptionInputs : MonoBehaviour
{
    private UnityEvent OnFirstChoice;
    private UnityEvent OnSecondChoice;
    private UnityEvent AdvanceDialogue;

    public float advanceWait = 0.25f;
    private float waitTimer;
    
    void Awake()
    {
        OnFirstChoice = EventMgr.Instance.CreateOrGetEventSink(EventSinks.Dialogue.OnFirstChoice);
        OnSecondChoice = EventMgr.Instance.CreateOrGetEventSink(EventSinks.Dialogue.OnSecondChoice);
        AdvanceDialogue = EventMgr.Instance.CreateOrGetEventSink(EventSinks.Dialogue.OnAdvance);
    }

    void Update()
    {
        if (waitTimer >= 0)
        {
            waitTimer -= Time.deltaTime;
        }

        CheckForInputs();
    }
    
    void CheckForInputs()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //left click/ square button for Dialogue Option 1
        if (Input.GetMouseButtonDown(0) || inputDevice.Action3.WasPressed)
        {
           OnFirstChoice?.Invoke();
        }
        //right click/ circle button for Dialogue Option 2
        if (Input.GetMouseButtonDown(1) || inputDevice.Action2.WasPressed)
        {
            OnSecondChoice?.Invoke();
        }
        
        //advance by space or x
        if ((Input.GetKeyDown(KeyCode.Space) || inputDevice.Action1.WasPressed) && waitTimer < 0) 
        {
            AdvanceDialogue?.Invoke();
            waitTimer = advanceWait;
        }
    }
}

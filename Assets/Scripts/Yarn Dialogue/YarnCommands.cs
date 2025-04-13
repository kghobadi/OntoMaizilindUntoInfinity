using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Yarn.Unity;

[Serializable]
public struct WaitReason
{
    public string name;
    public bool reason;
}

/// <summary>
/// Variety of Yarn events used and triggered across the game. 
/// </summary>
public class YarnCommands : MonoBehaviour
{
    private DialogueRunner dialogueRunner;
    
    public WaitReason[] waitReasons;

    public UnityEvent onRaminSuicide;
    public UnityEvent onPlayerKillsRamin;

    public UnityEvent[] onInteractEvents;

    private void Awake()
    {
        dialogueRunner = GetComponentInChildren<DialogueRunner>();
    }

    private void Start() 
    {
        //actual yarn commands
        dialogueRunner.AddCommandHandler<string,bool>("active", Active);
        dialogueRunner.AddCommandHandler<string>("raminkillsself", OnRaminSuicide);
        dialogueRunner.AddCommandHandler<string>("playerkillsramin", OnPlayerKillRamin);
        dialogueRunner.AddCommandHandler<string>("waitforreason", StartWaitForReason);
        dialogueRunner.AddCommandHandler<string>("waitfortime", StartWaitForTime);
        dialogueRunner.AddCommandHandler<int>("triggerevent", OnTriggerEvent);
    }

    private void Active(string targetName, bool active)
    {
        GameObject.Find(targetName).SetActive(active);
    }

    void OnRaminSuicide(string killer)
    {
        onRaminSuicide?.Invoke();
    }

    void OnPlayerKillRamin(string killer)
    {
        onPlayerKillsRamin?.Invoke();
    }
    
    void OnTriggerEvent(int eventIndex)
    {
        onInteractEvents[eventIndex]?.Invoke();
    }

    #region Waits
    void StartWaitForReason(string reason)
    {
        StopAllCoroutines();
        SetReasonTrue(reason);
        StartCoroutine(WaitForReason(reason));
    }

    //TODO make both buy and sell yarn commands wait until trade finishes. 
    IEnumerator WaitForReason(string reason)
    {
        int reasonIndex = FindReasonIndexByName(reason);
        
        //reason bool must become false for the dialogue runner to progress. 
        yield return new WaitUntil(() => waitReasons[reasonIndex].reason == false);
        
        Debug.Log("Wait for " + reason + " ended!");
        
        //allow yarn to progress
    }
    
    private int FindReasonIndexByName(string name)
    {
        int index = -1;
        for(int i = 0; i < waitReasons.Length; i++)
        {
            if (waitReasons[i].name == name)
            {
                index = i;
            }
        }

        if (index < 0)
        {
            Debug.LogError("Couldn't find wait reason index by that name.");
        }
        
        return index;
    }

    void SetReasonTrue(string reason)
    {
        int reasonIndex = FindReasonIndexByName(reason);
        WaitReason waitReason = waitReasons[reasonIndex];
        waitReason.reason = true;
        waitReasons[reasonIndex] = waitReason;
    }
    
    void SetReasonFalse(string reason)
    {
        int reasonIndex = FindReasonIndexByName(reason);
        WaitReason waitReason = waitReasons[reasonIndex];
        waitReason.reason = false;
        waitReasons[reasonIndex] = waitReason;
    }

    void StartWaitForTime(string time)
    {
        StopAllCoroutines();
        StartCoroutine(WaitForTime(time));
    }

    IEnumerator WaitForTime(string time)
    {
        float wait = float.Parse(time);
        
        //reason bool must become false for the dialogue runner to progress. 
        yield return new WaitForSeconds(wait);

        //allow yarn to progress
    }
    #endregion
    
}
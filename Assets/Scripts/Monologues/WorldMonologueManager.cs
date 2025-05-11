using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

[System.Serializable]
public struct Monologues
{
    public Monologue monologue;
    public MonologueTrigger mTrigger; 
}

public class WorldMonologueManager : NonInstantiatingSingleton<WorldMonologueManager>
{
    protected override WorldMonologueManager GetInstance () { return this; }
    public Monologues[] allMonologues;
    public MonologueManager[] allMonoManagers;
    public DialogueRunner _dialogueRunner;

    protected override void OnAwake()
    {
        base.OnAwake();
        
        if (allMonoManagers.Length == 0)
            allMonoManagers = FindObjectsOfType<MonologueManager>();

        if (_dialogueRunner)
            _dialogueRunner = FindObjectOfType<DialogueRunner>();
    }
    
    public void ClearAllPreviousMonologues()
    {
        for (int i = 0; i < allMonoManagers.Length; i++)
        {
            allMonoManagers[i].DisableMonologue();
        }
    }
    
    /// <summary>
    /// Stops all dialogue views. 
    /// </summary>
    public void StopAllViews()
    {
        //stop dialogue runner system. 
        _dialogueRunner.Stop();
        
        // Stop any processes that might be running already
        foreach (var dialogueView in _dialogueRunner.dialogueViews)
        {
            dialogueView.DialogueComplete();
        }

        //Stop any new dialogues which are waiting. This is important. 
        for (int i = 0; i < allMonoManagers.Length; i++)
        {
            allMonoManagers[i].AnimChar.StopAllCoroutines();
        }
    }
}

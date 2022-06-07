using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//Can use this scriptable object to create various types of tasks for NPCs to assign the player from their Task Manager
[CreateAssetMenu(fileName = "CheckpointData", menuName = "ScriptableObjects/CheckpointScriptable", order = 1)]
public class Checkpoint : ScriptableObject
{
    public string checkpointName;
    public float gameTime;
    public bool hasActivated;
    
    public UnityEvent activationEvent;
}

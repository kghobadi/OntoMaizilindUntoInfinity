using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cameras;
using NPC;

//Can use this scriptable object to create various types of tasks for NPCs to assign the player from their Task Manager
[CreateAssetMenu(fileName = "MonologueData", menuName = "ScriptableObjects/MonologueScriptable", order = 1)]
public class Monologue : ScriptableObject
{
    [Header("Info for Monologue Manager")]
    public TextAsset monologue;
    [Tooltip("This Monologue's Index within World Monologue Manager on WorldManager GameObject")]
    public int worldMonoIndex;
    [Tooltip("Check this to wait timeUntilStart from trigger Activation to enable Monologue")]
    public bool waitToStart;
    public float timeUntilStart;
    [Tooltip("Check this to lock your player's movement")]
    public bool lockPlayer = true;
    public bool loadsScene;

    [Header("Info for Monologue Reader")]
    public float timeBetweenLetters = 0.1f;
    public float timeBetweenLines = 3f;
    [Tooltip("Check this and fill in array below so that each line of text can be assigned a different wait")]
    public bool conversational;
    public float[] waitTimes;

    [Header("Repeat?")]
    [Tooltip("The Monologue Manager will repeat this monologue until further notice")]
    public bool repeatsAtFinish;
    [Tooltip("A condensed version of the Task assignment for repeating")]
    public TextAsset condensedMonologue;

    [Header("New Monologues")]
    [Tooltip("Use this for Monologues that do not assign tasks to activate a new dialogue after a certain amount of time")]
    public bool triggersMonologues;
    [Tooltip("If there are monologue Managers we directly call to")]
    public int[] monologueManagerIndeces;
    [Tooltip("Array values correspond to the specific monologue within manager array of which we will activate")]
    public int[] monologueIndecesWithinManager;
    [Tooltip("Indeces of the Monologue Triggers to activate from within WorldMonoManager")]
    public int[] monologueTriggerIndeces;
    [Tooltip("How long should these Monologue Triggers wait to activate?")]
    public float[] monologueWaits;

    [Header("Cinematics")]
    [Tooltip("After this Monologue finishes, the manager will play a cinematic")]
    public bool playsCinematic;
    public Cinematic cinematic;
    [Tooltip("After this Monologue finishes, the manager will enable a cinematic triggers somewhere in the game")]
    public bool enablesCinematicTriggers;
    public Cinematic[] cTriggers;

    [Header("NPC changes")]
    [Tooltip("After this Monologue finishes, the manager will set NPC movement using this")]
    public MovementPath newMovement;
    [Tooltip("Uses NPC movement script idle look at to change body rotation")]
    public int newIdleLook = -1; 
    [Tooltip("Check this to make character body look at something when they start mono")]
    public bool bodyLooks;
    [Tooltip("Int of obj to look at within LookAtObjs of NPCMovementManager")]
    public int bodyLookAt;
    [Tooltip("Check this to make character head look at something when they start mono")]
    public bool headLooks;
    [Tooltip("Int of obj to look at within LookAtObjs of NPCMovementManager")]
    public int headLookAt;
    [Tooltip("Uncheck this to make the NPC continue looking at new points")]
    public bool returnToOriginalRotation = true;

    [Tooltip("Check to use distance fading and active distance below.")]
    public bool useDistFading;
    [Tooltip("Sets the Mono Managers distance Active for subtitle fading.")]
    public float activeFadeDistance = 10f;
    
    [Tooltip("Check to make this dialogue end a hallucination if there is one active.")]
    public bool endsCurrentHallucination;
    [Tooltip("Check to make this dialogue interruptible by Yarn")]
    public bool interruptible;

    [Tooltip("Check to to update the Subtitle controllers lifetime")]
    public bool updatesSubLifetime;
    [Tooltip("Allows you to update the Subtitle controllers lifetime.")]
    public float subtitleLifetime;
    //new animation state?
    //new vo?
}
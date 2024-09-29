using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NPC;

//this class stores a list of movement paths for the MovementPath scriptable objs to reference 

[System.Serializable]
public struct MovementPaths
{
    public string pathName;
    [Tooltip("is this a Pathfinder or a Waypoint loop?")]
    public Movement.NPCMovementTypes moveType;
    [Tooltip("Drop in any number of points, game objs, or empty transforms for the NPC to nav to ")]
    public Transform[] movementPoints;
    //or a radius 
    public float moveRadius;
    [Tooltip("If this is an IDLE move type, designate the Idle Type")]
    public Movement.IdleType idleType;
    [Tooltip("The run anim type")]
    public Movement.RunType runType;
    [Tooltip("For follower NPC behavior")]
    public Transform followObject;
}

public class NPCMovementManager : NonInstantiatingSingleton<NPCMovementManager>
{
    // impl for NonInstantiatingSingleton
    protected override NPCMovementManager GetInstance () { return this; }

    public MovementPaths[] movementPaths;

    public Transform [] lookAtObjects;

    public bool getAllNpcs;
    public Controller[] npcControllers;
    
    protected override void OnAwake()
    {
        base.OnAwake();

        if (getAllNpcs)
        {
            //find all npc controllers in scene if the array is empty. 
            FindAllNPCs();
        }
    }

    /// <summary>
    /// Grabs all NPC controllers in the scene.
    /// </summary>
    public void FindAllNPCs()
    {
        //get all npc controllers in the scene. 
        npcControllers = FindObjectsOfType<Controller>();
    }

    /// <summary>
    /// Snaps all NPCs to Ground Point.
    /// </summary>
    public void SnapAllNPCsToGround()
    {
        foreach (Controller npc in npcControllers)
        {
            npc.Movement.SnapToGroundPoint();
        }
    }

    /// <summary>
    /// Used at the end of the bombing scene. 
    /// </summary>
    public void DisableAllNPCs()
    {
        FindAllNPCs();
        foreach (Controller npc in npcControllers)
        {
            npc.gameObject.SetActive(false);
        }
    }
    
    public void DestroyAllNPCs()
    {
        FindAllNPCs();
        foreach (Controller npc in npcControllers)
        {
            Destroy(npc.gameObject);
        }
    }

    #region Add Face Commands - May deprecate
    /// <summary>
    /// Adds a normal face to all NPCs. 
    /// </summary>
    /// <param name="face"></param>
    public void AddNormalFaceToAllNPCs(Material face)
    {
        foreach (var npc in npcControllers)
        {
            npc.Faces.AddNormalFace(face);
        }
    }
    
    /// <summary>
    /// Adds a normal face to all NPCs. 
    /// </summary>
    /// <param name="face"></param>
    public void AddNormalFaceToAllMen(Material face)
    {
        foreach (var npc in npcControllers)
        {
            if (npc.isMan)
            {
                npc.Faces.AddNormalFace(face);
            }
        }
    }
    
    /// <summary>
    /// Adds a normal face to all NPCs. 
    /// </summary>
    /// <param name="face"></param>
    public void AddNormalFaceToAllWomen(Material face)
    {
        foreach (var npc in npcControllers)
        {
            if (!npc.isMan)
            {
                npc.Faces.AddNormalFace(face);
            }
        }
    }
    
    /// <summary>
    /// Adds a screaming face to all men NPCs. 
    /// </summary>
    /// <param name="face"></param>
    public void AddScreamingFaceToAllNPCs(Material face)
    {
        foreach (var npc in npcControllers)
        {
            npc.Faces.AddScreamingFace(face);
        }
    }

    #endregion
}

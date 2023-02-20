using System.Collections;
using System.Collections.Generic;
using NPC;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NPCMovementManager))]
public class NpcMovementMgrEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //Get planet man script
        NPCMovementManager npcMovementMgr = (NPCMovementManager)target;

        //button for finding all Npc controllers. 
        if (GUILayout.Button("Get All NPC Controllers"))
        {
            npcMovementMgr.FindAllNPCs();
        }

        //button for snapping All NPC to ground 
        if (GUILayout.Button("Snap All NPCs to Ground"))
        {
            npcMovementMgr.SnapAllNPCsToGround();
        }

        base.OnInspectorGUI();
    }
}
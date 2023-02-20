using System.Collections;
using System.Collections.Generic;
using NPC;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Movement))]
public class NpcMovementEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //Get planet man script
        Movement npcMovement = (Movement)target;

        //button for snapping NPC to ground 
        if (GUILayout.Button("Snap NPC to Ground"))
        {
            npcMovement.SnapToGroundPoint();
        }

        base.OnInspectorGUI();
    }
}
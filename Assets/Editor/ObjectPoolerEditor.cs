using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectPooler))]
public class ObjectPoolerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //Get objectPooler script
        ObjectPooler objectPooler = (ObjectPooler)target;

        //button for spawning NPCs to ground 
        if (GUILayout.Button("Spawn Citizens"))
        {
            objectPooler.GenerateObjects();
        }

        base.OnInspectorGUI();
    }
}
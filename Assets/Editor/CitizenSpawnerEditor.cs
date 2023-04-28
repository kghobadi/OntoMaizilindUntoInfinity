using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CitizenGenerator))]
public class CitizenSpawnerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //Get citizen generator script
        CitizenGenerator citizenGenerator = (CitizenGenerator)target;

        //button for spawning NPCs to ground 
        if (GUILayout.Button("Spawn Citizens"))
        {
            citizenGenerator.SpawnCitizens();
        }

        base.OnInspectorGUI();
    }
}
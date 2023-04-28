using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraSwitcher))]
public class CameraSwitcherEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //Get CameraSwitcher script
        CameraSwitcher cameraSwitcher = (CameraSwitcher)target;

        //button for clearing cam list to ground 
        if (GUILayout.Button("Clear Cam List"))
        {
            cameraSwitcher.ClearCamList();
        }

        base.OnInspectorGUI();
    }
}
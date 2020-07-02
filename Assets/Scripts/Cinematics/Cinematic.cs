using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cameras
{
    //Can use this scriptable object to create various types of tasks for NPCs to assign the player from their Task Manager
    [CreateAssetMenu(fileName = "CinematicData", menuName = "ScriptableObjects/CinematicScriptable", order = 1)]
    public class Cinematic : ScriptableObject
    {
        public string cinematicName;
        public int cIndex;
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cameras;

namespace NPC
{
    public class Controller : MonoBehaviour
    {
        //player and mode manager refs for all to share 
        [HideInInspector]
        public CameraSwitcher camSwitcher;
        [HideInInspector]
        public CameraManager camManager;
        [HideInInspector]
        public NPCMovementManager moveManager;
        [HideInInspector]
        public CinematicsManager cineManager;
        [HideInInspector]
        public WorldMonologueManager wmManager;

        //npc state manager
        public NPCStates npcState;
        public enum NPCStates { IDLE, MOVING, TALKING, WAVING, ACTING, TRADING }

        public LayerMask npcLayer;

        Animations npcAnimations;
        public Animations Animation { get { return npcAnimations; } }

        Movement npcMovement;
        public Movement Movement { get { return npcMovement; } }

        Sounds npcSounds;
        public Sounds Sounds { get { return npcSounds; } }

        MonologueManager npcMonologues;
        public MonologueManager Monologues { get { return npcMonologues; } }

        private void Awake()
        {
            moveManager = FindObjectOfType<NPCMovementManager>();

            //npc component refs 
            npcAnimations = GetComponent<Animations>();
            npcMovement = GetComponent<Movement>();
            npcSounds = GetComponent<Sounds>();

            //prob need to fetch monologue text from children 
            npcMonologues = GetComponent<MonologueManager>();
            if (npcMonologues == null)
                npcMonologues = GetComponentInChildren<MonologueManager>();
            if (npcMonologues)
                npcMonologues.npcController = this;
            
            //player refs
            camSwitcher = FindObjectOfType<CameraSwitcher>();
            camManager = FindObjectOfType<CameraManager>();
            cineManager = FindObjectOfType<CinematicsManager>();
            wmManager = FindObjectOfType<WorldMonologueManager>();
        }
    }
}

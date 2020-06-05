using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using Cameras;
using Cinemachine;
using NPC;

public class MonologueManager : MonoBehaviour
{
    //player refs
    GameObject currentPlayer;
    CameraSwitcher camSwitcher; 

    //npc management refs 
    [HideInInspector]
    public WorldMonologueManager wmManager;
    CameraManager camManager;
    [HideInInspector]
    public Controller npcController;
    MonologueReader monoReader;
    
    [Tooltip("if there is a background for speaking text")]
    public FadeUI textBack;
    AnimateDialogue animateTextback;
    //text component and string array of its lines
    public int currentMonologue;
    [Tooltip("Fill this with all the individual monologues the character will give")]
    public List<Monologue> allMyMonologues = new List<Monologue>();
    
    public bool inMonologue;
    [HideInInspector]
    public MonologueTrigger mTrigger;

    [Tooltip("Check to Enable monologue at index 0 at start")]
    public bool enableOnStart;

    void Awake()
    {
        camSwitcher = FindObjectOfType<CameraSwitcher>();

        if (textBack)
            animateTextback = textBack.GetComponent<AnimateDialogue>();

        wmManager = FindObjectOfType<WorldMonologueManager>();
        camManager = FindObjectOfType<CameraManager>();
        monoReader = GetComponentInChildren<MonologueReader>();
        monoReader.hostObj = gameObject;
        monoReader.monoManager = this;
    }

    void Start()
    {
        //set text to first string in my list of monologues 
        SetMonologueSystem(0);

        //play mono 0 
        if (enableOnStart)
        {
            EnableMonologue();
        }
    }

    //sets monologue system to values contained in Monologue[index]
    public void SetMonologueSystem(int index)
    {
        //set current monologue
        currentMonologue = index;

        //set mono reader text lines 
        monoReader.textLines = (allMyMonologues[currentMonologue].monologue.text.Split('\n'));

        //set current to 0 and end to length 
        monoReader.currentLine = 0;
        monoReader.endAtLine = monoReader.textLines.Length;

        //set mono reader text speeds 
        monoReader.timeBetweenLetters = allMyMonologues[currentMonologue].timeBetweenLetters;
        monoReader.timeBetweenLines = allMyMonologues[currentMonologue].timeBetweenLines;
        monoReader.conversational = allMyMonologues[currentMonologue].conversational;
        monoReader.waitTimes = allMyMonologues[currentMonologue].waitTimes;
    }

    //has a wait for built in
    public void EnableMonologue()
    {
        //disable until its time to start 
        if (allMyMonologues[currentMonologue].waitToStart)
        {
            if (monoReader.usesTMP)
                monoReader.the_Text.enabled = false;
            else
                monoReader.theText.enabled = false;

            StartCoroutine(WaitToStart());
        }
        //starts now
        else
        {
            StartMonologue();
        }
    }

    IEnumerator WaitToStart()
    {
        yield return new WaitForSeconds(allMyMonologues[currentMonologue].timeUntilStart);

        StartMonologue();
    }

    //actually starts
    void StartMonologue()
    {
        //enable text comps 
        if (monoReader.usesTMP)
            monoReader.the_Text.enabled = true;
        else
            monoReader.theText.enabled = true;

        //textback
        if (textBack)
        {
            textBack.FadeIn();
            if (animateTextback)
                animateTextback.active = true;
        }

        //player ref 
        CamObject cam = camSwitcher.cameraObjects[camSwitcher.currentCam];
        currentPlayer = cam.gameObject;

        //lock player movement
        if (allMyMonologues[currentMonologue].lockPlayer)
        {
            
        }

        //is this an npc?
        if (npcController)
        {
            //set talking anim
            if (npcController.Animation)
            {
                npcController.Animation.SetAnimator("talking");
            }
        }
        

        //begin mono 
        inMonologue = true;

        //start the typing!
        monoReader.SetTypingLine();
    }
    
    public void DisableMonologue()
    {
        StopAllCoroutines();

        //disable text components 
        if (monoReader.usesTMP)
            monoReader.the_Text.enabled = false;
        else
            monoReader.theText.enabled = false;

        //textback
        if (textBack)
        {
            textBack.FadeOut();
            if(animateTextback)
                animateTextback.active = false;
        }

        //is this an npc?
        if (npcController)
        {
            //set speaker to idle 
            if (npcController.Animation)
                npcController.Animation.SetAnimator("idle");
        }
        
        StartCoroutine(WaitForCameraTransition());
    }

    IEnumerator WaitForCameraTransition()
    {
        yield return new WaitForSeconds(1f);

        Monologue mono = allMyMonologues[currentMonologue];

        //is this an npc?
        if (npcController)
        {
            //stop that waiting!
            if (npcController.Movement)
            {
                npcController.Movement.waitingToGiveMonologue = false;
            }
        }

        //player ref 
        CamObject cam = camSwitcher.cameraObjects[camSwitcher.currentCam];
        currentPlayer = cam.gameObject;

        //unlock player
        if (mono.lockPlayer)
        {
            
        }

        //check for cinematic to enable 
        if (mono.playsCinematic)
        {
            npcController.cineManager.allCinematics[mono.cinematic.cIndex].cPlaybackManager.StartTimeline();
        }
        //cinematic triggers to enable
        if (mono.enablesCinematicTriggers)
        {
            for (int i = 0; i < mono.cTriggers.Length; i++)
            {
                npcController.cineManager.allCinematics[mono.cTriggers[i].cIndex].cTrigger.gameObject.SetActive(true);
            }
        }

      
        //if this monologue repeats at finish
        if (mono.repeatsAtFinish)
        {
            //reset the monologue trigger after 3 sec 
            mTrigger.WaitToReset(5f);
        }
        //disable the monologue trigger, it's done 
        else
        {
            mTrigger.gameObject.SetActive(false);
        }

        //if this monologue has a new monologue to activate
        if (mono.triggersMonologues)
        {
            //enable the monologues but wait to make them usable to player 
            for(int i = 0; i< mono.monologueIndeces.Length; i++)
            {
                MonologueTrigger mTrigger = wmManager.allMonologues[mono.monologueIndeces[i]].mTrigger;
                mTrigger.gameObject.SetActive(true);
                mTrigger.hasActivated = true;
                mTrigger.WaitToReset(mono.monologueWaits[i]);
            }
        }

        //new npc movement?
        if (mono.newMovement)
        {
            npcController.Movement.ResetMovement(mono.newMovement);
        }

        inMonologue = false;
    }
}


using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using Cameras;
using Cinemachine;
using NPC;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class MonologueManager : MonoBehaviour
{
    //player refs
    GameObject currentPlayer;
    CameraSwitcher camSwitcher; 
    private Camera mainCam;

    //npc management refs 
    [HideInInspector]
    public WorldMonologueManager wmManager;
    CameraManager camManager;
    [HideInInspector]
    public Controller npcController;
    [SerializeField]
    private MonologueReader monoReader;
    [SerializeField] 
    private SpeakerSound speakerSound;
    [SerializeField]
    [Tooltip("This is for specific characters, like the grandmothers.")]
    private bool sharesReader;

    [Tooltip("Is this the player's monologue manager?")]
    public bool isPlayer;
    private float distToRealP;
    [Tooltip("if there is a background for speaking text")]
    public FadeUI textBack;
    AnimateDialogue animateTextback;
    [Tooltip("if there is a background for speaking text")]
    [SerializeField]
    private FadeUiRevamped[] allFades;
    //text component and string array of its lines
    public int currentMonologue;
    [Tooltip("Fill this with all the individual monologues the character will give")]
    public List<Monologue> allMyMonologues = new List<Monologue>();
    
    public bool inMonologue;
    [HideInInspector]
    public MonologueTrigger mTrigger;

    [Tooltip("Check to Enable monologue at index 0 at start")]
    public bool enableOnStart;

    [Tooltip("Sprite Object with head")]
    public Transform head;
    Transform origIdleLook;
    Vector3 origBodyRot;
    Vector3 origHeadRot;

    private IEnumerator newMonologue;
    
    [Header("Events")]
    [SerializeField] private UnityEvent onMonoBegin;
    [SerializeField] private UnityEvent onMonoEnd;

    [Header("Subtitle Settings")] 
    [SerializeField]
    private GameObject customSubtitlePrefab;
    [SerializeField]
    private GameObject leftSideSubtitlePrefab;
    [SerializeField] private string characterName;
    [SerializeField] private float subtitleLifetime = 6;
    [SerializeField] private bool useLineSkipping;
    [SerializeField] private FaceVisibility _faceVisibility;
    public FaceVisibility FaceVisible => _faceVisibility;

    public bool UseLineSkipping => useLineSkipping;
    public MonologueReader MonologueReader => monoReader;

    /// <summary>
    /// Gets player pos in different scenes. 
    /// </summary>
    public Vector3 PlayerPos
    {
        get
        {
            if (camSwitcher != null)
            {
                return camSwitcher.currentPlayer.transform.position;
            }
            else
            {
                return Vector3.zero;
            }
        }
    }
    
    /// <summary>
    /// Accessor for distance from character to player.
    /// </summary>
    public float DistToRealP
    {
        get
        {
            distToRealP = Vector3.Distance(transform.position, PlayerPos);

            return distToRealP;
        }
    }

    public Monologue CurrentMonologue => allMyMonologues[currentMonologue];
    
    void Awake()
    {
        mainCam = Camera.main;
        camSwitcher = FindObjectOfType<CameraSwitcher>();

        if (textBack)
            animateTextback = textBack.GetComponent<AnimateDialogue>();

        wmManager = FindObjectOfType<WorldMonologueManager>();
        camManager = FindObjectOfType<CameraManager>();
        speakerSound = GetComponent<SpeakerSound>();
        //Most every character will have a Monologue reader as a child.
        if (!sharesReader)
        {
            if(monoReader == null)
                monoReader = GetComponentInChildren<MonologueReader>();
            if (monoReader)
            {
                monoReader.hostObj = gameObject;
                monoReader.monoManager = this;
            }
        }
        //For shared readers, they are target in editor and we are added to their list. 
        else
        {
            monoReader.AddSharedReader(this);
        }
    }

    void Start()
    {
        //set text to first string in my list of monologues 
        if(allMyMonologues.Count > 0)
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
        SetMonologueSystem(allMyMonologues[currentMonologue]);
    }

    public void SetMonologueSystem(Monologue mono)
    {
        //set mono reader text lines 
        monoReader.textLines = (mono.monologue.text.Split('\n'));

        //set current to 0 and end to length 
        monoReader.currentLine = 0;
        monoReader.endAtLine = monoReader.textLines.Length;

        //set mono reader text speeds 
        monoReader.timeBetweenLetters = mono.timeBetweenLetters;
        monoReader.timeBetweenLines = mono.timeBetweenLines;
        monoReader.conversational = mono.conversational;
        monoReader.waitTimes = mono.waitTimes;
    }

    //overwrites any previous wait to set New mono call
    public void WaitToSetNewMonologue(int index)
    {
        if(newMonologue != null)
            StopCoroutine(newMonologue);
        newMonologue = WaitToSetNew(index);
        StartCoroutine(newMonologue);
    }

    //waits until this mono manager is no longer in mono to set system and enable new monologue
    IEnumerator WaitToSetNew(int index)
    {
        yield return new WaitUntil(() => inMonologue == false);

        yield return new WaitForEndOfFrame();

        SetMonologueSystem(index);
        
        EnableMonologue();
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
        //Use all fades
        else
        {
            foreach(var fade in allFades)
            {
                fade.FadeIn();
            }
        }

        //Activate face animation UI if it has it / we dont use subtitles 
        if(monoReader.faceAnimationUI != null)
        {
            monoReader.faceAnimationUI.Activate();
        }

        //player ref 
        if (camSwitcher)
        {
            CamObject cam = camSwitcher.currentCamObj;
            currentPlayer = cam.gameObject;
        }

        //set mono
        Monologue mono = allMyMonologues[currentMonologue];

        //lock player movement
        if (mono.lockPlayer)
        {
            
        }
        
        //is this an npc?
        if (npcController)
        {
            //assign new idle look at 
            if (mono.newIdleLook >= 0)
            {
                //grab original look
                origIdleLook = npcController.Movement.lookAtTransform;
                npcController.Movement.SetLookAt(mono.newIdleLook);
            }

            //body looks at?
            if (mono.bodyLooks)
            {
                origBodyRot = transform.localEulerAngles;

                Vector3 point = npcController.moveManager.lookAtObjects[mono.bodyLookAt].position;

                Vector3 bodyLook = new Vector3(point.x, transform.position.y, point.z);

                transform.LookAt(bodyLook);
            }

            //head looks at? 
            if (mono.headLooks)
            {
                origHeadRot = head.transform.localEulerAngles;

                Vector3 point = npcController.moveManager.lookAtObjects[mono.headLookAt].position;

                Vector3 headLook = new Vector3(point.x, head.position.y, point.z);

                head.transform.LookAt(headLook);
            }
            
            //set talking anim
            if (npcController.Animation)
            {
                npcController.Animation.SetAnimator("talking");
            }
        }

        //begin mono 
        inMonologue = true;
        
        onMonoBegin.Invoke();

        //start the typing!
        monoReader.SetTypingLine();
    }
    
    public void DisableMonologue()
    {
        StopAllCoroutines();
        
        if(!inMonologue)
            return;

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
        //Use all fades 
        else
        {
            foreach (var fade in allFades)
            {
                fade.FadeOut();
            }
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
            
            //new npc movement?
            if (mono.newMovement)
            {
                npcController.Movement.ResetMovement(mono.newMovement);
            }
        }

        //player ref 
        if (camSwitcher)
        {
            CamObject cam = camSwitcher.currentCamObj;
            if(cam)
                currentPlayer = cam.gameObject;
        }
       

        //unlock player
        if (mono.lockPlayer)
        {
            
        }

        //return to orignal look rotations
        if (mono.returnToOriginalRotation)
        {
            //reset lookat point 
            if(npcController && mono.newIdleLook >= 0)
                npcController.Movement.SetLook(origIdleLook);

            //body looks at?
            if (mono.bodyLooks)
            {
                transform.localEulerAngles = origBodyRot;
            }

            //head looks at? 
            if (mono.headLooks)
            {
                head.transform.localEulerAngles = origHeadRot;
            }
        }
        
        //check for cinematic to enable 
        if (mono.playsCinematic && npcController)
        {
            npcController.cineManager.allCinematics[mono.cinematic.cIndex].cPlaybackManager.StartTimeline();
        }
        //cinematic triggers to enable
        if (mono.enablesCinematicTriggers && npcController)
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
            if(mTrigger)
                mTrigger.WaitToReset(5f);
        }
        //disable the monologue trigger, it's done 
        else
        {
            if(mTrigger)
                mTrigger.gameObject.SetActive(false);
        }

        //if this monologue has a new monologue to activate
        if (mono.triggersMonologues)
        {
            //enable the monologues but wait to make them usable to player 
            for(int i = 0; i< mono.monologueTriggerIndeces.Length; i++)
            {
                MonologueTrigger mTrigger = wmManager.allMonologues[mono.monologueTriggerIndeces[i]].mTrigger;
                mTrigger.gameObject.SetActive(true);
                mTrigger.hasActivated = true;
                mTrigger.WaitToReset(mono.monologueWaits[i]);
            }

            //loop thru other managers to activate
            for (int i = 0; i < mono.monologueManagerIndeces.Length; i++)
            {
                //get manager
                MonologueManager otherMonoManager = wmManager.allMonoManagers[mono.monologueManagerIndeces[i]];
                //set manager to new monologue from within its list
                otherMonoManager.WaitToSetNewMonologue(mono.monologueIndecesWithinManager[i]);
            }
        }

        //Call end to any hallucs 
        if (mono.endsCurrentHallucination)
        {
            Hallucination[] possibleHallucs = FindObjectsOfType<Hallucination>();

            foreach (var halluc in possibleHallucs)
            {
                if (halluc.hallucinating)
                {
                    halluc.WaitToEnd(halluc.hallucinationEndWait);
                }
            }
        }
        
        onMonoEnd.Invoke();
        
        //disable mono 
        inMonologue = false;
    }

    #region Subtitle Controller

    public SubtitleController currentSub;
    /// <summary>
    /// Creates a new type of subtitle. 
    /// </summary>
    public void CreateSubtitle(string lineOfText)
    {
        //Get subtitle prefab based on side of screen. 
        GameObject subtitlePrefab = customSubtitlePrefab;

        //Default to right side
        ScreenSide side = ScreenSide.RightSide;
        if (_faceVisibility != null)
        {
            side =  _faceVisibility.GetScreenSide();
            if (side == ScreenSide.LeftSide && leftSideSubtitlePrefab)
            {
                subtitlePrefab = leftSideSubtitlePrefab;
            }
        }
       
        //Instantiate it
        currentSub = SubtitleMgr.Instance.GenerateSubtitle(this,side,  subtitlePrefab);
        //set character name
        currentSub.SetCharacterTitle(characterName);
        //mono reader calls and supplies this param
        currentSub.SetText(lineOfText);
        
        //should tell it to fade in
        currentSub.FadeControls.FadeIn();
        
        //give it fade out time (expiration)
        currentSub.FadeControls.SetWaitToFadeOut(subtitleLifetime);
        currentSub.SetSpeakerSound(speakerSound);
    }
    #endregion
}


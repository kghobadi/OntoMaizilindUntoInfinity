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
    private Camera mainCam;

    //npc management refs 
    [HideInInspector]
    public WorldMonologueManager wmManager;
    CameraManager camManager;
    [HideInInspector]
    public Controller npcController;
    MonologueReader monoReader;

    [Tooltip("Is this the player's monologue manager?")]
    public bool isPlayer;
    
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

    [Tooltip("Sprite Object with head")]
    public Transform head;
    Transform origIdleLook;
    Vector3 origBodyRot;
    Vector3 origHeadRot;

    private IEnumerator newMonologue;

    [Header("Subtitle System")] 
    public bool useSubtitles;
    private GameObject mySubtitle;
    public float subSizeMult = 1f;
    public bool centerOffScreenSub;
    SubtitleInWorldManager subtitleInWorldManager;
    private RectTransform subRectTransform;
    private Image subImageBack;
    [HideInInspector] public TextMeshProUGUI subtitleTMP;
    [HideInInspector] public CanvasGroup subCanvasGroup;
    [HideInInspector] public bool subChanging;
    private string prevSubText;
    [HideInInspector] public float currentSubTime;
    //[HideInInspector] public float voiceAudibility;
    private float distToRealP;

    /// <summary>
    /// Accessor for distance from character to player.
    /// </summary>
    public float DistToRealP
    {
        get
        {
            distToRealP = Vector3.Distance(transform.position, camSwitcher.currentPlayer.transform.position);

            return distToRealP;
        }
    }

    Transform rootT;
    //SpriteRenderer mainSR;

    [HideInInspector] public Image arrowImg;
    [HideInInspector] public float subPointOffsetX;
    
    void Awake()
    {
        mainCam = Camera.main;
        camSwitcher = FindObjectOfType<CameraSwitcher>();
        subtitleInWorldManager = FindObjectOfType<SubtitleInWorldManager>();

        if (textBack)
            animateTextback = textBack.GetComponent<AnimateDialogue>();

        wmManager = FindObjectOfType<WorldMonologueManager>();
        camManager = FindObjectOfType<CameraManager>();
        monoReader = GetComponentInChildren<MonologueReader>();
        if (monoReader)
        {
            monoReader.hostObj = gameObject;
            monoReader.monoManager = this;
        }
    }

    void Start()
    {
        //set text to first string in my list of monologues 
        if(allMyMonologues.Count > 0)
            SetMonologueSystem(0);

        //set up my subtitle.
        if (useSubtitles)
        {
            mySubtitle = subtitleInWorldManager.SetupNewSubtitle(this);
            subRectTransform = mySubtitle.GetComponent<RectTransform>();
            subImageBack = mySubtitle.GetComponent<Image>();
        }
        
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
        if (camSwitcher)
        {
            CamObject cam = camSwitcher.currentCamObj;
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
            if(mono.newIdleLook >= 0)
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

        //new npc movement?
        if (mono.newMovement)
        {
            npcController.Movement.ResetMovement(mono.newMovement);
        }

        //disable mono and set sub time to 0
        currentSubTime = 0;
        inMonologue = false;
    }

    #region Subtitle Management

    /// <summary>
    /// Actually sets the subtitle text. 
    /// </summary>
    /// <param name="text"></param>
    public void SetSubtitleText(string text)
    {
        subtitleTMP.text = text;
        RendererExtensions.ChangeWidthOfObject(subRectTransform,subtitleTMP, monoReader.maxWidth, monoReader.sideOffset);

        currentSubTime += Time.deltaTime;
        
        subChanging = subtitleTMP.text != prevSubText;
        prevSubText = subtitleTMP.text;
    }

    public void ManageSubHeightPos()
    {
        if (isPlayer)
        {
            mySubtitle.transform.position = mainCam.transform.position + mainCam.transform.forward;
        }
        else
        {
            //TODO this is still off a bit -- need to figure out why. 
            //maybe i need to convert textback rectransform pos to world pos? just doesn't seem like i am getting the right Y pos. could even use an empty transform. 
            
            mySubtitle.transform.position = new Vector3(mySubtitle.transform.position.x, textBack.transform.transform.position.y,
                mySubtitle.transform.position.z);
        }
    }

    #endregion
}


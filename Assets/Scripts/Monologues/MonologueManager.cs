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
    [SerializeField] private string characterName;
    [SerializeField] private float subtitleLifetime = 6;

    [SerializeField] private FaceVisibility _faceVisibility;
    public FaceVisibility FaceVisible => _faceVisibility;
    //Face settings
    public float faceSizeMult = 2f;
    public FaceAnimationUI facePointer;
    [HideInInspector] public RectTransform faceRect;
    #region SubtitleInWorld - Deprecate
    [Header("Subtitle In World System - Deprecated")] 
    [FormerlySerializedAs("useSubtitles")] 
    public bool useSubtitlesInWorld;
    [Tooltip("Position for the Subtitle system to target.")]
    public Transform subtitleTarget;
    [HideInInspector] public GameObject mySubtitle;
    public float subSizeMult = 1f;
    public bool centerOffScreenSub;
    SubtitleInWorldManager subtitleInWorldManager;
    private RectTransform subRectTransform;
    private Image subImageBack;
    [HideInInspector] public TextMeshProUGUI subtitleTMP;
    [HideInInspector] public CanvasGroup subCanvasGroup;
    /// <summary>
    /// True whenever our subtitle's text does not match previously recorded set text. 
    /// </summary>
    public bool SubChanging => subtitleTMP.text != prevSubText;
    private string prevSubText;
    [HideInInspector] public float currentSubTime;

    private float distToRealP;
    
    [Tooltip("If you set this to anything greater than 0 the character's Subtitle will fade in and out at that dist from player.")]
    [SerializeField]
    private float distanceActive = 0f;
    private FadeUiRevamped[] subtitleFades;

    public MonologueReader MonologueReader => monoReader;
    
    /// <summary>
    /// Fetch the face height. 
    /// </summary>
    public float FaceHeightOffset
    {
        get
        {
            float height = faceRect.sizeDelta.y;

            if (facePointer.active)
            {
                height *= 2;
            }
            else
            {
                height = 0;
            }

            return height;
        }
    }

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

    public float DistActive => distanceActive;
    public FadeUiRevamped[] SubtitleFades => subtitleFades;

    Transform rootT;
    //SpriteRenderer mainSR;

    [HideInInspector] public Image arrowImg;
    [HideInInspector] public float subPointOffsetX;
    #endregion
    
    void Awake()
    {
        mainCam = Camera.main;
        camSwitcher = FindObjectOfType<CameraSwitcher>();
        subtitleInWorldManager = FindObjectOfType<SubtitleInWorldManager>();

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

        if (facePointer)
        {
            faceRect = facePointer.GetComponent<RectTransform>();
        }
    }

    void Start()
    {
        //set text to first string in my list of monologues 
        if(allMyMonologues.Count > 0)
            SetMonologueSystem(0);

        //set up my subtitle.
        if (useSubtitlesInWorld)
        {
            mySubtitle = subtitleInWorldManager.SetupNewSubtitle(this);
            subRectTransform = mySubtitle.GetComponent<RectTransform>();
            subImageBack = mySubtitle.GetComponent<Image>();
            subtitleFades = mySubtitle.GetComponentsInChildren<FadeUiRevamped>();
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
        //Use all fades
        else
        {
            foreach(var fade in allFades)
            {
                fade.FadeIn();
            }
        }

        //Activate face animation UI if it has it / we dont use subtitles 
        if(monoReader.faceAnimationUI != null && !useSubtitlesInWorld)
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

        //use new dist fading value provided by the mono.
        if (mono.useDistFading)
        {
            distanceActive = mono.activeFadeDistance;
        }
        //return to 0 for distance active so we have no fading.
        else
        {
            distanceActive = 0f;
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
        
        //disable mono and set sub time to 0
        currentSubTime = 0;
        inMonologue = false;
    }

    #region Subtitle Controller

    /// <summary>
    /// Creates a new type of subtitle. 
    /// </summary>
    public void CreateSubtitle(string lineOfText)
    {
        //Instantiate it
        SubtitleController newSubtitle = SubtitleMgr.Instance.GenerateSubtitle(this, customSubtitlePrefab);
        //set character name
        newSubtitle.SetCharacterTitle(characterName);
        //mono reader calls and supplies this param
        newSubtitle.SetText(lineOfText);
        //should tell it to fade in
        newSubtitle.FadeControls.FadeIn();
        
        //give it fade out time (expiration)
        newSubtitle.FadeControls.SetWaitToFadeOut(subtitleLifetime);
        newSubtitle.SetSpeakerSound(speakerSound);
    }
    #endregion

    #region Subtitle In World Management - Deprecated

    /// <summary>
    /// Enables the subtitle obj.
    /// </summary>
    public void EnableSubtitle()
    {
        mySubtitle.SetActive(true);
        facePointer.Activate();
    }

    /// <summary>
    /// Disables the subtitle obj.
    /// </summary>
    public void DisableSubtitle()
    {
        mySubtitle.SetActive(false);
        facePointer.Deactivate();
    }
    
    /// <summary>
    /// Actually sets the subtitle text. 
    /// </summary>
    /// <param name="text"></param>
    public void SetSubtitleText(string text)
    {
        subtitleTMP.text = text;
        RendererExtensions.ChangeWidthOfObject(subRectTransform,subtitleTMP, monoReader.maxWidth, monoReader.sideOffset);

        //set sub time - is this correct?
        currentSubTime += Time.deltaTime;
        
        prevSubText = subtitleTMP.text;
    }

    /// <summary>
    /// Fades in the subtitle.
    /// </summary>
    public void FadeInSubtitle()
    {
        foreach (var subtitleFade in subtitleFades)
        {
            subtitleFade.FadeIn();
        }

        if (facePointer)
        {
            facePointer.FadeInFaces();
        }
    }
    
    /// <summary>
    /// Fades out the subtitle.
    /// </summary>
    public void FadeOutSubtitle()
    {
        foreach (var subtitleFade in subtitleFades)
        {
            subtitleFade.FadeOut();
        }

        if (facePointer)
        {
            facePointer.FadeOutFaces();
        }
    }

    /// <summary>
    /// Determines the height y position of the subtitle's transform. 
    /// </summary>
    public void ManageSubHeightPos()
    {
        if (isPlayer)
        {
            mySubtitle.transform.position = mainCam.transform.position + mainCam.transform.forward;
        }
        else
        {
            mySubtitle.transform.position = new Vector3(mySubtitle.transform.position.x, textBack.transform.transform.position.y, mySubtitle.transform.position.z);
        }
    }

    /// <summary>
    /// Arrow pos passed in from subtitle manager. 
    /// </summary>
    /// <param name="pos"></param>
    public void SetFacePointerPos()
    {
        float heightOffset = faceRect.sizeDelta.y / faceSizeMult;
        faceRect.localPosition = Vector3.zero - new Vector3(0f, heightOffset,0f);
    }

    #endregion
}


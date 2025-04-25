using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using InControl;
using UnityEngine.Events;

//this script is responsible for the active reading out of a monologue 
public class MonologueReader : MonoBehaviour {
    public GameObject hostObj;  // parent NPC obj set by MonologueManager
    [HideInInspector]
    public MonologueManager monoManager; //my mono manager
    [SerializeField]
    private bool sharedReader;
    private List<MonologueManager> monoManagers = new List<MonologueManager>();

    private Camera mainCam;
    private RectTransform myRectTransform;
    private RectTransform textBackTransform;
    SpeakerSound speakerAudio;
    private List<SpeakerSound> speakers = new List<SpeakerSound>(); // for shared readers
    [HideInInspector] public Text theText;
    [HideInInspector] public TMP_Text the_Text;
    [HideInInspector] public bool usesTMP;
    [Tooltip("No need to fill this in, that will happen automatically")]
    public string[] textLines;
    //current and last lines
    public int currentLine;
    public int endAtLine;

    public bool canSkip = true;
    public bool readingMono;
    //typing vars
    private bool isTyping = false;
    IEnumerator currentTypingLine;
    IEnumerator waitForNextLine;

    [Header("Text Timing")]
    public float timeBetweenLetters;
    //wait between lines
    public float timeBetweenLines;
    [Tooltip("Check this and fill in array below so that each line of text can be assigned a different wait")]
    public bool conversational;
    public float[] waitTimes;
    bool waiting;
    [Tooltip("Check this if you've set up another Mono Reader to progress this one.")]
    public bool waitForDialogue;
    [Tooltip("Check this if this should progress another Mono Reader.")]
    public bool progressesOthers;
    [Tooltip("Allows this mono reader to progress another ")]
    public UnityEvent onProgressLine;
    [Tooltip("Allows this mono reader to progress another ")]
    public UnityEvent progressOthers;
    
    [Header("Width Altering")]
    [Tooltip("Check to dynamically alter width of text box.")]
    public bool useDynamicWidth;
    public float maxWidth = 1000f;
    public float sideOffset = 25f;

    [Header("Subtitle Reader")] 
    public FaceAnimationUI faceAnimationUI;

    [SerializeField] private bool useFaceSubs;

    void Awake()
    {
        myRectTransform = GetComponent<RectTransform>();
        textBackTransform = transform.parent.GetComponent<RectTransform>();
        mainCam = Camera.main;
        theText = GetComponent<Text>();
        
        if (theText == null)
        {
            usesTMP = true;
            the_Text = GetComponent<TMP_Text>();
        }
    }

    /// <summary>
    /// Adds a shared reader. 
    /// </summary>
    /// <param name="mgr"></param>
    public void AddSharedReader(MonologueManager mgr)
    {
        if (!monoManagers.Contains(mgr))
        {
            monoManagers.Add(mgr);
            speakers.Add(mgr.GetComponent<SpeakerSound>());
        }
    }

    void Start()
    {
        //get speaker audio from host object
        if(hostObj)
            speakerAudio = hostObj.GetComponent<SpeakerSound>();

        //check if TMP or normal Text
        if (usesTMP)
            the_Text.enabled = false;
        else
            theText.enabled = false;
    }
    
    void Update ()
    {
        if(monoManager && monoManager.UseLineSkipping)
            LineSkipping();
    }
    
    /// <summary>
    /// Used for player thought monologues. 
    /// </summary>
    void LineSkipping()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //speaker is typing out message
        if (isTyping)
        {
            //player skips to the end of the line
            if ((Input.GetMouseButtonDown(0)|| inputDevice.Action3.WasPressed) && canSkip)
            {
                if (currentTypingLine != null)
                {
                    StopCoroutine(currentTypingLine);
                }

                //set to full line
                if (isTyping)
                {
                    if (useFaceSubs)
                    {
                        monoManager.currentSub.SetFullLine(textLines[currentLine]);
                    }
                    else
                        CompleteTextLine(textLines[currentLine]);
                }

                SetWaitForNextLine();
            }
        }

        //player is waiting for next message
        if (waiting)
        {
            //player skips to next line
            if ((Input.GetMouseButtonDown(0) || inputDevice.Action3.WasPressed) && canSkip)
            {
                if (waitForNextLine != null)
                {
                    StopCoroutine(waitForNextLine);
                }

                ProgressLine();
            }
        }
    }

    public void Progress()
    {
        if (readingMono)
        {
            ProgressLine();
        }
    }

    void ProgressLine()
    {
        currentLine += 1;
        waiting = false;

        //reached the  end, reset
        if (currentLine >= endAtLine)
        {
            EndMono();
        }
        //set next typing line 
        else
        {
            SetTypingLine();
        }

        onProgressLine.Invoke();
    }

    /// <summary>
    /// Called when interrupted. 
    /// </summary>
    public void ManualEnd()
    {
        StopAllCoroutines();
        EndMono();
    }

    void EndMono()
    {
        readingMono = false;
       
        //shared readers 
        if (sharedReader)
        {
            foreach(var mgr in monoManagers)
            {
                mgr.DisableMonologue();
            }
        }
        else
        {
            //standard disable
            monoManager.DisableMonologue();
        }
            
        //deactivate face anim ui
        if (faceAnimationUI)
        {
            faceAnimationUI.Deactivate();
        }
    }

    //calls text scroll coroutine 
    public void SetTypingLine()
    {
        //start
        if (readingMono == false)
            readingMono = true;
        
        //Set all shared readers 
        if (sharedReader)
        {
            foreach (var mgr in monoManagers)
            {
                SetAnimations(mgr, "talking");
            }
        }
        else
        {
            //set talking anim
            SetAnimations(monoManager, "talking");
        }

        //Create a face sub for this line of text 
        if (useFaceSubs)
        {
            //If we do distance check here to decide whether the subtitle even spawns, we can wait with SetWaitForLine and +3 sec or something 
            if (monoManager.DistToRealP <= monoManager.CurrentMonologue.activeFadeDistance)
            {
                monoManager.CreateSubtitle(textLines[currentLine]);
                isTyping = true;
            }
            else
            {
                //Character says an Audio Only line - no subtitle. 
                StartCoroutine(AudioOnlyLine(textLines[currentLine]));
            }
        }
        //Individual letters type out
        else
        {
            if (currentTypingLine != null)
            {
                StopCoroutine(currentTypingLine);
            }
            currentTypingLine = TextScroll(textLines[currentLine]);

            StartCoroutine(currentTypingLine);
        }
    }

    //TODO Create Scroll by Line method as an option for Mono Reader
    //Make back and forth between this and MonoMgr for new subtitle system. 
    
    //Coroutine that types out each letter individually
    private IEnumerator TextScroll(string lineOfText)
    {
        // set first letter
        int letter = 0;
        if (usesTMP)
            the_Text.text = "";
        else
            theText.text = "";

        isTyping = true;

        while (isTyping && (letter < lineOfText.Length - 1))
        {
            //add this letter to our text
            if (usesTMP)
            {
                the_Text.text += lineOfText[letter];
            }
            else
            {
                theText.text += lineOfText[letter];
            }

            if (!sharedReader)
            {
                //check what audio to play 
                if (speakerAudio)
                    speakerAudio.AudioCheck(lineOfText, letter);
            }
            else
            {
                foreach(var speaker in speakers)
                {
                    speaker.AudioCheck(lineOfText, letter);
                }
            }

            //adjust width of ui
            if (useDynamicWidth)
            {
                RendererExtensions.ChangeWidthOfObject(textBackTransform, the_Text, maxWidth, sideOffset);
            }

            //next letter
            letter += 1;
            yield return new WaitForSeconds(timeBetweenLetters);
        }

        //player waited to read full line
        if (isTyping)
            CompleteTextLine(lineOfText);

        SetWaitForNextLine();
    }
    
    /// <summary>
    /// Coroutine that makes AI speak without any subtitle (you weren't close enough to see it. 
    /// </summary>
    /// <param name="lineOfText"></param>
    /// <returns></returns>
    private IEnumerator AudioOnlyLine(string lineOfText)
    {
        // set first letter
        int letter = 0;

        isTyping = true;

        while (isTyping && (letter < lineOfText.Length - 1))
        {
            //Speak this letter 
            if (speakerAudio)
                speakerAudio.AudioCheck(lineOfText, letter);

            //next letter
            letter += 1;
            yield return new WaitForSeconds(timeBetweenLetters);
        }
        
        isTyping = false;
        //player waited to read full line
        SetWaitForNextLine();
    }

    /// <summary>
    /// Tells a monologue manager to animate talking 
    /// </summary>
    /// <param name="mgr"></param>
    void SetAnimations(MonologueManager mgr, string param)
    {
        if (mgr.npcController)
        {
            if (mgr.npcController.Animation)
            {
                //set talking anim if not already talking 
                if (mgr.npcController.Animation.HasParameter(param))
                {
                    if (mgr.npcController.Animation.characterAnimator.GetBool(param) == false)
                        mgr.npcController.Animation.SetAnimator(param);
                }
            }
        }
    }

    //completes current line of text
    void CompleteTextLine(string lineOfText)
    {
        if (usesTMP)
            the_Text.text = lineOfText;
        else
            theText.text = lineOfText;
        
        isTyping = false;
    }

    /// <summary>
    /// Allows subtitle to tell us when it has finished. 
    /// </summary>
    public void OnLineFinished()
    {
        isTyping = false;
        //Check to ensure this is still reading!
        if (readingMono)
        {
            SetWaitForNextLine();
        }
    }

    //calls wait for next line coroutine 
    void SetWaitForNextLine()
    {
        //start waiting coroutine 
        if (waitForNextLine != null)
        {
            StopCoroutine(waitForNextLine);
        }

        //check what the wait time for this line should be 
        if (conversational)
        {
            waitForNextLine = WaitToProgressLine(waitTimes[currentLine]);
        }
        else if (waitForDialogue)
        {
            //do nothing
        }
        else
        {
            waitForNextLine = WaitToProgressLine(timeBetweenLines);
        }

        if (!waitForDialogue)
        {
            StartCoroutine(waitForNextLine);
        }
    }

    //start wait for next line after spacebar skip
    IEnumerator WaitToProgressLine(float time)
    {
        yield return new WaitForEndOfFrame();

        waiting = true;

        if (progressesOthers)
        {
            yield return new WaitForSeconds(time / 3);
    
            progressOthers.Invoke();
        
            yield return new WaitForSeconds((time / 3) * 2);
        }
        else
        {
            yield return new WaitForSeconds(time);
        }
        
        ProgressLine();
    }

    /// <summary>
    /// Lets you wait for an action. 
    /// </summary>
    /// <param name="wait"></param>
    /// <param name="onWaitFinished"></param>
    /// <returns></returns>
    IEnumerator WaitForAction(float wait, Action onWaitFinished)
    {
        yield return new WaitForSeconds(wait);
        
        onWaitFinished.Invoke();
    }

    /// <summary>
    /// So we can toggle this setting. 
    /// </summary>
    /// <param name="state"></param>
    public void SetWaitingFor(bool state)
    {
        waitForDialogue = state;
    }
}

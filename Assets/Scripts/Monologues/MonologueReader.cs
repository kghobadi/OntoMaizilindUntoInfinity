using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using InControl;

//this script is responsible for the active reading out of a monologue 
public class MonologueReader : MonoBehaviour {
    public GameObject hostObj;  // parent NPC obj set by MonologueManager
    [HideInInspector]
    public MonologueManager monoManager; //my mono manager

    private Camera mainCam;
    private RectTransform myRectTransform;
    private RectTransform textBackTransform;
    SpeakerSound speakerAudio;
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

    [Header("Main Canvas Reader")] 
    public bool usesScreenReader;
    public ScreenReader screenReader; //assigned by main canvas mono reader
    private MainCanvasMonologueReader mainCanvasReaderCreator;

    void Awake()
    {
        mainCanvasReaderCreator = FindObjectOfType<MainCanvasMonologueReader>();
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

    void Start()
    {
        speakerAudio = hostObj.GetComponent<SpeakerSound>();

        if (usesTMP)
            the_Text.enabled = false;
        else
            theText.enabled = false;
    }
    
    void Update ()
    {
        //LineSkipping();

        if (readingMono)
        {
            CheckVisible();
        }
    }

    public void SetScreenReader(ScreenReader reader)
    {
        screenReader = reader;
    }
    
    /// <summary>
    /// Checks whether the world space monologue reader is visible or not.
    /// </summary>
    void CheckVisible()
    {
        //seems we cannot just check renderer.isVisible since they are ui elements. 
        bool isVisible = myRectTransform.IsVisibleFrom(mainCam);

        if (isVisible)
        {
            //we good -- deactivate screen reader if there is one
            if (screenReader)
            {
                screenReader.Deactivate();
            }
        }
        else
        {
            //can't see it on screen, activate screen reader
            if (screenReader)
            {
                screenReader.Activate();
                //set text so its not empty!
                if (usesTMP)
                    screenReader.SetText(the_Text.text); 
                else
                    screenReader.SetText(theText.text); 
            }
            //don't have a screen reader yet 
            else
            {
                //generate one from the creator
                if (mainCanvasReaderCreator && usesScreenReader)
                {
                    mainCanvasReaderCreator.GenerateReader(this);
                    
                    //set text so its not empty!
                    if (usesTMP)
                        screenReader.SetText(the_Text.text); 
                    else
                        screenReader.SetText(theText.text); 
                }
            }
        }
    }
    
    void LineSkipping()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        //speaker is typing out message
        if (isTyping)
        {
            //player skips to the end of the line
            if ((Input.GetKeyDown(KeyCode.Space) || inputDevice.Action3.WasPressed) && canSkip)
            {
                if (currentTypingLine != null)
                {
                    StopCoroutine(currentTypingLine);
                }

                //set to full line
                if (isTyping)
                    CompleteTextLine(textLines[currentLine]);

                SetWaitForNextLine();
            }
        }

        //player is waiting for next message
        if (waiting)
        {
            //player skips to next line
            if ((Input.GetKeyDown(KeyCode.Space) || inputDevice.Action3.WasPressed) && canSkip)
            {
                if (waitForNextLine != null)
                {
                    StopCoroutine(waitForNextLine);
                }

                ProgressLine();
            }
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
    }

    void EndMono()
    {
        readingMono = false;
        monoManager.DisableMonologue();
            
        //deactivate screen reader if there is one
        if (screenReader)
        {
            screenReader.Deactivate();
        }
    }

    //calls text scroll coroutine 
    public void SetTypingLine()
    {
        //start
        if (readingMono == false)
            readingMono = true;
        
        if (currentTypingLine != null)
        {
            StopCoroutine(currentTypingLine);
        }
        currentTypingLine = TextScroll(textLines[currentLine]);

        StartCoroutine(currentTypingLine);
    }

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

        //set talking anim
        if (monoManager.npcController)
        {
            if (monoManager.npcController.Animation)
            {
                //set talking anim if not already talking 
                if (monoManager.npcController.Animation.characterAnimator.GetBool("talking") == false)
                    monoManager.npcController.Animation.SetAnimator("talking");
            }
        }

        while (isTyping && (letter < lineOfText.Length - 1))
        {
            //for screen reader 
            string screenText;
            
            //add this letter to our text
            if (usesTMP)
            {
                the_Text.text += lineOfText[letter];
                screenText = the_Text.text;
            }
            else
            {
                theText.text += lineOfText[letter];
                screenText = theText.text;
            }
                
            //set screen reader
            if (screenReader)
            {
                screenReader.SetText(screenText);
            }
            
            //adjust width of ui
            //ChangeWidthOfObject();
            
            //check what audio to play 
            if(speakerAudio)
                speakerAudio.AudioCheck(lineOfText, letter);
            //next letter
            letter += 1;
            yield return new WaitForSeconds(timeBetweenLetters);
        }

        //player waited to read full line
        if (isTyping)
            CompleteTextLine(lineOfText);

        SetWaitForNextLine();
    }

    //completes current line of text
    void CompleteTextLine(string lineOfText)
    {
        if (usesTMP)
            the_Text.text = lineOfText;
        else
            theText.text = lineOfText;
        
        if(screenReader)
            screenReader.SetText(lineOfText);
        
        isTyping = false;
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
        else
        {
            waitForNextLine = WaitToProgressLine(timeBetweenLines);
        }

        StartCoroutine(waitForNextLine);
    }

    //start wait for next line after spacebar skip
    IEnumerator WaitToProgressLine(float time)
    {
        yield return new WaitForEndOfFrame();

        waiting = true;

        yield return new WaitForSeconds(time);

        ProgressLine();
    }

    [Header("Width Altering")] 
    public float maxWidth = 1000f;
    public float sideOffset = 25f;
    private void ChangeWidthOfObject()
    {
        var width = 0f;
        if(usesTMP)
            width = the_Text.preferredWidth;
        else
            width = theText.preferredWidth;
        
        //set to width if it is less than max
        if (width < maxWidth)
        {
            myRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
            textBackTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width + sideOffset);
        }
        //set to max width 
        else
        {
            myRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
            textBackTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth + sideOffset);
        }
    }
}

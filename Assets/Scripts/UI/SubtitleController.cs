using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum ScreenSide
{
    LeftSide,
    RightSide,
}

/// <summary>
/// Controller for Subtitles spoken in the Apartment scene. 
/// </summary>
public class SubtitleController : MonoBehaviour
{
    //Provided connection when spawned in Subtitle Mgr. 
    [SerializeField] private MonologueManager monoMgr;
    private SpeakerSound speakerSound;
    private CanvasGroup canvasGroup;
    
    //Private local references
    [SerializeField] private FaceAnimationUI faceAnimation;
    [SerializeField] private FadeUiRevamped fader;
    
    [Header("Main UI Components")] 
    [SerializeField]
    private Image textBox;
    [SerializeField]
    private TMP_Text[] characterTitleTexts;
    [SerializeField]
    private TMP_Text subtitleText;
    [SerializeField]
    private Image arrowImg;

    [SerializeField] private ScreenSide screenSide;

    [SerializeField] private bool isTyping;

    [SerializeField] private WritingStyle writingStyle;
    public enum WritingStyle
    {
        Instant,
        FullWords,
        LetterByLetter,
    }

    private float minHeight = 75f; //starts at size of subtitle box
    [SerializeField] private float maxHeight = 500f;
    [SerializeField] private float topOffset = 15f;
    [SerializeField] private float timeBetweenLetters = 0.035f;
    [SerializeField] private bool dynamicHeight = true;
    [SerializeField] private bool useFaceDetection;
    [SerializeField] private float maxDistFromCenter = 500f;
    //Public properties
    public CanvasGroup CanvasGroup
    {
        get
        {
            if (canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }

            return canvasGroup;
        }
    }
    public MonologueManager MonoMgr
    {
        get => monoMgr;
        set => monoMgr = value;
    }
    public FadeUiRevamped FadeControls => fader;

    private void Update()
    {
        if (useFaceDetection)
        {
            FaceDetection();
        }
        else
        {
            if(faceAnimation && !faceAnimation.active) 
                faceAnimation.Activate();
        }
    }

    void FaceDetection()
    {
        if (monoMgr.FaceVisible)
        {
            //Is the character's face visible?
            if (monoMgr.FaceVisible.FaceIsVisible && monoMgr.FaceVisible.GetDistanceFromCenter() < maxDistFromCenter)
            {
                if(faceAnimation.active) 
                    faceAnimation.Deactivate();
            }
            //It's not visible -> show our face 
            else
            {
                if(!faceAnimation.active) 
                    faceAnimation.Activate();
            }
        }
    }

    /// <summary>
    /// Plugs in a speaker sound. 
    /// </summary>
    /// <param name="sound"></param>
    public void SetSpeakerSound(SpeakerSound sound)
    {
        speakerSound = sound;
    }

    /// <summary>
    /// Plugs in character title
    /// </summary>
    /// <param name="title"></param>
    public void SetCharacterTitle(string title)
    {
        foreach (var characterTitleText in characterTitleTexts)
        {
            characterTitleText.text = title;
        }
    }

    /// <summary>
    /// Sets the text according to this subtitle's writing style. 
    /// </summary>
    /// <param name="message"></param>
    public void SetText(string message)
    {
        switch (writingStyle)
        {
            case WritingStyle.Instant:
                subtitleText.text = message;
                break;
            case WritingStyle.FullWords:
                StartCoroutine(TextScrollByWord(message));
                break;
            case WritingStyle.LetterByLetter:
                StartCoroutine(TextScrollByLetter(message));
                break;
        }
    }
    
    /// <summary>
    /// Coroutine that types out each letter individually
    /// </summary>
    /// <param name="lineOfText"></param>
    /// <returns></returns>
    private IEnumerator TextScrollByWord(string lineOfText)
    {
        // set first letter
        int word = 0;
        subtitleText.text = "";

        //get words
        string[] words = lineOfText.Split(' ');
        isTyping = true;

        while (isTyping && (word < words.Length - 1))
        {
            string wordStr = words[word] + " ";
            //add this word to our text
            subtitleText.text += wordStr;
            
            //Try changing height as needed
            if(dynamicHeight)
                RendererExtensions.ChangeHeightOfRect(textBox.rectTransform, subtitleText, minHeight,  maxHeight, topOffset);

            //speak sound
            if (speakerSound)
                speakerSound.SpeakWord(lineOfText);

            //next word
            word += 1;
            yield return new WaitForSeconds(timeBetweenLetters);
        }

        //player waited to read full line
        if (isTyping)
            SetFullLine(lineOfText);
    }
    
    /// <summary>
    /// Coroutine that types out each letter individually
    /// </summary>
    /// <param name="lineOfText"></param>
    /// <returns></returns>
    private IEnumerator TextScrollByLetter(string lineOfText)
    {
        // set first letter
        int letter = 0;
        subtitleText.text = "";

        isTyping = true;

        while (isTyping && (letter < lineOfText.Length - 1))
        {
            //add this letter to our text
            subtitleText.text += lineOfText[letter];
            if (speakerSound)
                speakerSound.AudioCheck(lineOfText, letter);

            //next letter
            letter += 1;
            yield return new WaitForSeconds(timeBetweenLetters);
        }

        //player waited to read full line
        if (isTyping)
            SetFullLine(lineOfText);
    }
    
    /// <summary>
    /// Completes current line of text
    /// </summary>
    /// <param name="lineOfText"></param>
    public void SetFullLine(string lineOfText)
    {
        subtitleText.text = lineOfText;
             
        //Try changing height as needed
        if(dynamicHeight)
            RendererExtensions.ChangeHeightOfRect(textBox.rectTransform, subtitleText, minHeight, maxHeight, topOffset);
        
        MonoMgr.MonologueReader.OnLineFinished();
        isTyping = false;
    }
}

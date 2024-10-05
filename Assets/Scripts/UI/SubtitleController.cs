using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

/// <summary>
/// Controller for Subtitles spoken in the Apartment scene. 
/// </summary>
public class SubtitleController : MonoBehaviour
{
    //Provided connection when spawned in Subtitle Mgr. 
    [SerializeField] private MonologueManager monoMgr;
    private SpeakerSound speakerSound;
    
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
    
    //Public properties
    public MonologueManager MonoMgr
    {
        get => monoMgr;
        set => monoMgr = value;
    }
    public FaceAnimationUI FaceAnimationUI => faceAnimation;
    public FadeUiRevamped FadeControls => fader;
    
    //Todo should fade out / in face
    // private void Update()
    // {
    //     //check if face is overlapping the character's head. 
    // }

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

            //get random #
            int letter = Random.Range(0, 26);
            if (speakerSound)
                speakerSound.AudioCheck(lineOfText, letter);

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
    
    //completes current line of text
    void SetFullLine(string lineOfText)
    {
        subtitleText.text = lineOfText;
             
        //Try changing height as needed
        if(dynamicHeight)
            RendererExtensions.ChangeHeightOfRect(textBox.rectTransform, subtitleText, minHeight, maxHeight, topOffset);
        
        MonoMgr.MonologueReader.OnLineFinished();
        isTyping = false;
    }
}

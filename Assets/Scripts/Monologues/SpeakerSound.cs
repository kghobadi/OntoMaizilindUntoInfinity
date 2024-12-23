﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpeakerSound : AudioHandler
{
    //audio stuff
    [Header("Speaker Bools")]
    [Tooltip("enables voice audio")]
    public bool hasVoiceAudio;
    public bool randomPitch;
    public bool usesAllLetters, countsUp;
    public int speakFreq = 4;
	public float volume = 0.6f;
    
    //for random sound reading
    [Header("Speaker Sounds")]
    public AudioClip[] spokenSounds;
    //for spoken alphabet
    public List<char> letters = new List<char>();
    public List<char> capitalLetters = new List<char>();
    //for matching up letters with sounds 
    public List<AudioClip> spokenLetters = new List<AudioClip>();
    
    //Face anim stuff 
    public bool animateFaceToSound;
    [SerializeField] private FaceAnimationUI faceAnimUi;
    FaceAnimation _faceAnim;
    //Accessor for face. 
    public FaceAnimation FaceAnimation => _faceAnim;
    SpriteRenderer face; 

    private void Start()
    {
        if (animateFaceToSound)
        {
            GetFaceReferences();
        }
    }
    
    void GetFaceReferences()
    {
        //get face anim
        _faceAnim = GetComponent<FaceAnimation>();
        if (_faceAnim == null)
        {
            _faceAnim = GetComponentInChildren<FaceAnimation>();
        }
    }
    
    //checks what kind of audio to play for the speaker 
    public void AudioCheck(string lineOfText, int letter)
    {
        if (hasVoiceAudio)
        {
            if (!countsUp)
            {
                if (letter % speakFreq == 0)
                {
                    if (lineOfText.Length > letter) //protects from index out of range ex
                    {
                        Speak(lineOfText[letter]);
                    }
                }
            }
            else
            {
                if (!voices[voiceCounter].isPlaying)
                {
                    PlaySoundUp(spokenSounds, 1f);
                }
            }
        }
    }

    /// <summary>
    /// Speaks random letter from the line of text. 
    /// </summary>
    /// <param name="lineOfText"></param>
    public void SpeakWord(string lineOfText)
    {
        if (hasVoiceAudio)
        {  
            int randomLetter = Random.Range(0,lineOfText.Length);
            Speak(lineOfText[randomLetter]);
        }
    }

    //check through our alphabet of sounds and play corresponding character
    public void Speak(char letter)
    {
        //cycle through audioSources for voice
        voiceCounter = CountUpArray(voiceCounter, voices.Length - 1);

        if (usesAllLetters)
        {
            //check in letters
            if (letters.Contains(letter))
            {
                int index = letters.IndexOf(letter);
                voices[voiceCounter].clip = spokenSounds[index];
                voices[voiceCounter].PlayOneShot(spokenSounds[index]);
                //Debug.Log("spoke");
            }
            //check in capital letters
            else if (capitalLetters.Contains(letter))
            {
                int index = capitalLetters.IndexOf(letter);
                voices[voiceCounter].clip = spokenSounds[index];
                voices[voiceCounter].PlayOneShot(spokenSounds[index]);
                //Debug.Log("spoke capital");
            }
            //punctuation or other stuff?
            else
            {
                PlayRandomSound(spokenSounds, volume);
                //Debug.Log("gibberish");
            }
        }
        //for characters who only use gibberish sounds
        else
        {
            if (randomPitch)
            {
                PlayRandomSoundRandomPitch(spokenSounds, volume);
            }
            else
            {
                PlayRandomSound(spokenSounds, volume);
            }
            
            //Debug.Log("gibberish");
        }

    }

    void Update()
    {
        if (animateFaceToSound)
        {
            FaceSwap();
        }
    }

    //swaps face sprite for talking/idle
    void FaceSwap()
    {
        if (myAudioSource.isPlaying)
        {
            if (_faceAnim)
                _faceAnim.SetAnimator("talking");
            
            if(faceAnimUi)
                faceAnimUi.Activate();
        }
        else
        {
            if (_faceAnim)
                _faceAnim.SetAnimator("idle");
            
            if(faceAnimUi)
                faceAnimUi.SetIdle();
        }
    }

}

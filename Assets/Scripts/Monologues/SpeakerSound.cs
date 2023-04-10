using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeakerSound : AudioHandler
{
    //audio stuff
    [Header("Speaker Bools")]
    [Tooltip("enables voice audio")]
    public bool hasVoiceAudio;
    public bool randomPitch;
    public bool usesAllLetters, countsUp;
    public int speakFreq = 4;
    
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
                PlayRandomSound(spokenSounds, 1f);
                //Debug.Log("gibberish");
            }
        }
        //for characters who only use gibberish sounds
        else
        {
            if (randomPitch)
            {
                PlayRandomSoundRandomPitch(spokenSounds, 1f);
            }
            else
            {
                PlayRandomSound(spokenSounds, 1f);
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
        }
        else
        {
            if (_faceAnim)
                _faceAnim.SetAnimator("idle");
        }
    }

}

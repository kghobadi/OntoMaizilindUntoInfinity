using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Audio;

public class Television : MonoBehaviour {
    VideoPlayer vidPlayer;
    CameraSwitcher camSwitcher;
    public GameObject planes;
    public GameObject sirens;
    public GameObject citizens;
    public AudioSource music;
    public FadeUI speechPanel;
    public FadeUI shiftToChange;

    [Header("Audio Switching during Speech")]
    public GameObject radio;
    AudioSource tvSource;
    AudioSource radioSource;
    public DialogueText shahSpeech;
    public int [] transitionLines;
    int currentTransition = 0;
    public bool speechEnded;
    public AudioMixerSnapshot bombing;

	void Awake () {
        vidPlayer = GetComponent<VideoPlayer>();
        tvSource = GetComponent<AudioSource>();
        radioSource = radio.GetComponent<AudioSource>();
        camSwitcher = FindObjectOfType<CameraSwitcher>();
	}

    void Start()
    {
        //tv audio to start 
        //SwitchDeviceAudio();
    }

    void Update () {

        //switching audio back and forth between radio & tv
        if(currentTransition < transitionLines.Length)
        {
            if (shahSpeech.currentLine == transitionLines[currentTransition])
            {
                SwitchDeviceAudio();
                currentTransition++;
            }
        }

        //end speech, activate sirens & planes 
		if(vidPlayer.frame >= (long)vidPlayer.frameCount - 3  && shahSpeech.currentLine > shahSpeech.endAtLine - 1)
        {
            EndSpeech();
        }

        //debug
        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            EndSpeech();
        }

        //fade out graphic 
        if(shahSpeech.currentLine == shahSpeech.endAtLine)
        {
            speechPanel.FadeOut();
        }
	}

    void EndSpeech()
    {
        vidPlayer.Stop();
        planes.SetActive(true);
        sirens.SetActive(true);
        citizens.SetActive(true);
        music.Play();

        camSwitcher.canShift = true;
        shiftToChange.FadeIn();
        bombing.TransitionTo(3f);
       
        speechEnded = true;
    }

    //switch mutes on TV & Radio sources
    void SwitchDeviceAudio()
    {
        tvSource.mute = !tvSource.mute;
        radioSource.mute = !radioSource.mute;
    }
}

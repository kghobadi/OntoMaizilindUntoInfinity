using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;


public class Television : MonoBehaviour {
    VideoPlayer vidPlayer;
    public GameObject planes;
    public GameObject sirens;
    public FadeUI speechPanel;

    [Header("Audio Switching during Speech")]
    public GameObject radio;
    AudioSource tvSource;
    AudioSource radioSource;
    public DialogueText shahSpeech;
    public int [] transitionLines;
    int currentTransition = 0;

    [Header("Scene Transition")]
    public AdvanceScene advance;
    public bool speechEnded;
    public float changeSceneTimer = 0f, timeUntilChange = 30f;


	void Awake () {
        vidPlayer = GetComponent<VideoPlayer>();
        tvSource = GetComponent<AudioSource>();
        radioSource = radio.GetComponent<AudioSource>();
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

        //fade out graphic 
        if(shahSpeech.currentLine == shahSpeech.endAtLine)
        {
            speechPanel.FadeOut();
        }

        //time until loading next scene 
        if (speechEnded)
        {
            changeSceneTimer += Time.deltaTime;

            if(changeSceneTimer > timeUntilChange)
            {
                advance.LoadNextScene();
            }
        }
	}

    void EndSpeech()
    {
        vidPlayer.Stop();
        planes.SetActive(true);
        sirens.SetActive(true);
       
        speechEnded = true;
    }

    //switch mutes on TV & Radio sources
    void SwitchDeviceAudio()
    {
        tvSource.mute = !tvSource.mute;
        radioSource.mute = !radioSource.mute;
    }
}

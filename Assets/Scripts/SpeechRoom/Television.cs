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

        if(currentTransition < transitionLines.Length)
        {
            if (shahSpeech.currentLine == transitionLines[currentTransition])
            {
                SwitchDeviceAudio();
                currentTransition++;
            }
        }

        //end speech, activate sirens & planes 
		if(vidPlayer.frame >= (long)vidPlayer.frameCount - 3)
        {
            vidPlayer.Stop();
            planes.SetActive(true);
            sirens.SetActive(true);
            speechPanel.FadeOut();
        }
	}

    //switch mutes on TV & Radio sources
    void SwitchDeviceAudio()
    {
        tvSource.mute = !tvSource.mute;
        radioSource.mute = !radioSource.mute;
    }
}

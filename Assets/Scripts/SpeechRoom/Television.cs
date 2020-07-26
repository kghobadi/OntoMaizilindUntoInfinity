using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Audio;

public class Television : MonoBehaviour {
    VideoPlayer vidPlayer;
    CameraSwitcher camSwitcher;

    [Header("Channel Switching Before Speech")]
    public VideoClip[] tvChannels; // news clip, cartoon clip, western clip
    public int[] channelLastFrames;
    public int currentClip = 0; //defaults to the news channel 

    [Header("On Speech Start")]
    public VideoClip staticBroadcast;
    public VideoClip theSpeech;
    public Material staticEffect;
        Material origMat;
    public MusicFader warAmbience;
    public Material scarySky;
    public LerpLighting sunLerp;

    [Header("Objects to Activate")]
    public GameObject planes;
    public GameObject sirens;
    public GameObject citizens;
    public AudioSource music;
    public FadeUI speechPanel;
    public FadeUI shiftToChange;
    public BoxCollider frontDoorCollider;
    public GameObject backBuilding;
    public GameObject stairwell;
    public GameObject corridor;

    [Header("Audio Switching during Speech")]
    public Radio radio;
    AudioSource tvSource;
    AudioSource radioSource;
    public MonologueManager shahSpeech;
    public MonologueReader shahReader;
    public int [] transitionLines;
    int currentTransition = 0;
    public bool waitingForStatic;
    public bool speechStarted;
    public bool speechEnded;
    public AudioMixerSnapshot bombing;

	void Awake ()
    {
        vidPlayer = GetComponent<VideoPlayer>();
        tvSource = GetComponent<AudioSource>();
        radioSource = radio.GetComponent<AudioSource>();
        camSwitcher = FindObjectOfType<CameraSwitcher>();
        origMat = vidPlayer.targetMaterialRenderer.material;
	}

    void Start()
    {
        //set frame array length
        channelLastFrames = new int[tvChannels.Length];
        //disable
        transform.parent.gameObject.SetActive(false);
        stairwell.SetActive(false);
        corridor.SetActive(false);
    }

    void Update ()
    {
        //do this once speech starts
        if (speechStarted)
        {
            //our player is prepared but not playin 
            if (vidPlayer.isPrepared && vidPlayer.isPlaying == false && waitingForStatic)
            {
                //set mat to playable and play 
                vidPlayer.targetMaterialRenderer.material = origMat;
                vidPlayer.Play();
                //play radio and enable monologue at the same time 
                radioSource.Play();
                waitingForStatic = false;
            }

            //OLD METHOD -- using checkpoints now 
            //switching audio back and forth between radio & tv
            //if (currentTransition < transitionLines.Length)
            //{
            //    if (shahReader.currentLine == transitionLines[currentTransition])
            //    {
            //        SwitchDeviceAudio();
            //        currentTransition++;
            //    }
            //}

            //end speech, activate sirens & planes 
            if ((vidPlayer.isPlaying && vidPlayer.frame >= (long)vidPlayer.frameCount - 3) || shahReader.currentLine > shahReader.endAtLine - 1)
            {
                EndSpeech();
            }

            //debug
            if (Input.GetKeyDown(KeyCode.RightControl))
            {
                EndSpeech();

                shahSpeech.DisableMonologue();

                shahReader.hostObj.SetActive(false);
            }

            //fade out graphic 
            if (shahReader.currentLine == shahReader.endAtLine)
            {
                speechPanel.FadeOut();
            }
        }
        //anything before the speech starts
        else
        {
            //our player is prepared but not playin 
            if (vidPlayer.isPrepared && vidPlayer.isPlaying == false)
            {
                //set mat to playable and play 
                vidPlayer.targetMaterialRenderer.material = origMat;
                vidPlayer.Play();
                //set to previous last frame 
                if(channelLastFrames[currentClip] > 0)
                    vidPlayer.frame = channelLastFrames[currentClip];
            }

            //debug to start speech
            if (Input.GetKeyUp(KeyCode.RightControl))
            {
                StartSpeech();
            }
        }
	}

    //this can be called from elsewhere 
    public void SwitchChannel()
    {
        //set last frame
        channelLastFrames[currentClip] = (int)vidPlayer.frame; 

        //count up thru channels
        if(currentClip < tvChannels.Length - 1)
        {
            currentClip++;
        }
        else
        {
            currentClip = 0;
        }

        //set tv to that channel 
        SetVideoPlayer(tvChannels[currentClip]);
    }

    //stop -- static -- set new clip to play 
    public void SetVideoPlayer(VideoClip clip)
    {
        vidPlayer.Stop();
        vidPlayer.targetMaterialRenderer.material = staticEffect;
        vidPlayer.clip = clip;
        vidPlayer.Prepare();
    }

    //called when either we have triggered all the monologues or a timer? 
    public void StartSpeech()
    {
        //only works if it hasn't been called already 
        if(waitingForStatic == false && speechStarted == false)
            StartCoroutine(WaitForStatic());
    }

    IEnumerator WaitForStatic()
    {
        //set audio mutes -- tv on, radio off
        tvSource.mute = false;
        radioSource.mute = false;

        //radio 
        radioSource.Stop();
        radioSource.clip = radio.staticBroadcast;
        radioSource.volume = 1f;
        radioSource.Play();

        //set vid player, ambience
        SetVideoPlayer(staticBroadcast);
        warAmbience.FadeIn(1f, warAmbience.fadeSpeed);

        //sky and sun
        RenderSettings.skybox = scarySky;
        sunLerp.SetLightLerp(sunLerp.sunScary, sunLerp.sunNice);

        //wait for static to end
        waitingForStatic = true;
        yield return new WaitForSeconds((float)staticBroadcast.length);

        //speech truly begins
        SpeechBegins();
    }

    //speech actually starts
    void SpeechBegins()
    {
        //set vid player and enable mono
        SetVideoPlayer(theSpeech);
        shahSpeech.gameObject.SetActive(true);
        shahSpeech.EnableMonologue();

        //set radio audio
        radioSource.mute = true;
        radioSource.Stop();
        radioSource.clip = radio.shahSpeech;
        radioSource.volume = 1f;

        //enable front door, disable back building, enable corridor and stairwell
        frontDoorCollider.enabled = true;
        backBuilding.SetActive(false);
        stairwell.SetActive(true);
        corridor.SetActive(true);

        speechStarted = true;
    }

    //called when video player reaches final frames on speech clip 
    void EndSpeech()
    {
        vidPlayer.Stop();
        vidPlayer.targetMaterialRenderer.material = staticEffect;
        //planes.SetActive(true);
        sirens.SetActive(true);
        citizens.SetActive(true);
        //music.Play();

        camSwitcher.canShift = true;
        //shiftToChange.FadeIn();
        bombing.TransitionTo(3f);
       
        speechEnded = true;
    }

    //switch mutes on TV & Radio sources
    public void SwitchDeviceAudio()
    {
        tvSource.mute = !tvSource.mute;
        radioSource.mute = !radioSource.mute;
    }
}

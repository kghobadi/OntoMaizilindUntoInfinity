using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Audio;

public class Television : MonoBehaviour 
{
    VideoPlayer vidPlayer; 
    PauseMenu pauseMenu;
    MeshRenderer screenRender;

    [SerializeField]
    private bool debug;

    [Header("Channel Switching Before Speech")]
    [SerializeField]
    private VideoClip[] tvChannels; // news clip, cartoon clip, western clip
    [SerializeField]
    private int[] channelLastFrames;
    [SerializeField]
    private int currentClip = 0; //defaults to the news channel 

    public int CurrentClip => currentClip;

    [Header("On Speech Start")]
    [SerializeField]
    private VideoClip staticBroadcast;
    [SerializeField]
    private VideoClip theSpeech;
    [SerializeField]
    private Material staticEffect;
    Material origMat;
    [SerializeField]
    private MusicFader warAmbience;
    [SerializeField]
    private Material scarySky;
    [SerializeField]
    private LerpLighting sunLerp;

    [Header("Objects to Activate")]
    [SerializeField]
    private GameObject sirens;
    [SerializeField]
    private FadeUI speechPanel;
    [SerializeField]
    private BoxCollider frontDoorCollider;
    [SerializeField]
    private GameObject backBuilding;
    [SerializeField]
    private GameObject stairwell;
    [SerializeField]
    private GameObject corridor;

    [Header("Audio Switching during Speech")]
    [SerializeField]
    private Radio radio;
    AudioSource tvSource;
    AudioSource radioSource;
    [SerializeField]
    private AudioSource[] extraRadios;
    [SerializeField]
    private MonologueManager shahSpeech;
    [SerializeField]
    private MonologueReader shahReader;
    int currentTransition = 0;
    public bool waitingForStatic;
    public bool speechStarted;
    public bool speechEnded;
    public AudioMixerSnapshot bombing;

	void Awake ()
    {
        vidPlayer = GetComponent<VideoPlayer>();
        tvSource = GetComponent<AudioSource>();
        screenRender = GetComponent<MeshRenderer>();
        radioSource = radio.GetComponent<AudioSource>();
        pauseMenu = FindObjectOfType<PauseMenu>();
        origMat = screenRender.material;

        //Null checks
        if (tvSource == null)
        {
            Debug.LogError("Television could not find its audiosource!");
        }
        if (radio == null || radioSource == null)
        {
            Debug.LogError("Radio could not be found or it could not find its audiosource!");
        }
        if (sunLerp == null)
        {
            Debug.LogError("You are missing the reference to the Sun Lerp for the Lighting change!");   
        }
	}

    void Start()
    {
        //set frame array length
        channelLastFrames = new int[tvChannels.Length];
        //disable
        if (!debug)
        {
            transform.parent.gameObject.SetActive(false);
            if(stairwell)
                stairwell.SetActive(false);
            if(corridor)
                corridor.SetActive(false);
        }
        //debug scene -- start the tv at beginning
        else
        {
            SetVideoPlayer(tvChannels[0]);            
        }
    }

    void Update ()
    {
        //do this once speech starts
        if (speechStarted)
        {
            //our player is prepared but not playin 
            if (vidPlayer.isPrepared && vidPlayer.isPlaying == false && waitingForStatic && pauseMenu.paused == false)
            {
                //set mat to playable and play 
                screenRender.material = origMat;
                vidPlayer.Play();
                //play radio and enable monologue at the same time 
                radioSource.Play();
                //extra radios
                for (int i = 0; i < extraRadios.Length; i++)
                {
                    extraRadios[i].Play();
                    //disable their click scripts
                    TurnOnOrOff radioToggler = extraRadios[i].GetComponent<TurnOnOrOff>();
                    if(radioToggler)
                        radioToggler.enabled = false;
                    extraRadios[i].GetComponent<Collider>().enabled = false;
                }
                waitingForStatic = false;
            }

            //end speech, activate sirens & planes 
            if ((vidPlayer.isPlaying && vidPlayer.frame >= (long)vidPlayer.frameCount - 3) || shahReader.currentLine > shahReader.endAtLine - 1)
            {
                EndSpeech();
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
            if (vidPlayer.isPrepared && vidPlayer.isPlaying == false && pauseMenu.paused == false)
            {
                //set mat to playable and play 
                screenRender.material = origMat;
                vidPlayer.Play();
                //set to previous last frame 
                if(channelLastFrames[currentClip] > 0)
                    vidPlayer.frame = channelLastFrames[currentClip];
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

    /// <summary>
    /// Begins the starting interview with the Shah.
    /// </summary>
    public void SetStartingInterview()
    {
        SetVideoPlayer(tvChannels[0]);
    }

    //stop -- static -- set new clip to play 
    public void SetVideoPlayer(VideoClip clip)
    {
        vidPlayer.Stop();
        screenRender.material = staticEffect;
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
        radioSource.loop = false;
        radioSource.clip = radio.staticBroadcast;
        radioSource.volume = 1f;
        radioSource.Play();
        
        //extra radios
        for (int i = 0; i < extraRadios.Length; i++)
        {
            extraRadios[i].Stop();
            extraRadios[i].clip = radio.staticBroadcast;
            extraRadios[i].volume = 1f;
            extraRadios[i].Play();
        }

        //set vid player, ambience
        SetVideoPlayer(staticBroadcast);
        warAmbience.FadeIn(1f, warAmbience.fadeSpeed);

        //sky and sun
        RenderSettings.skybox = scarySky;
        if (sunLerp)
        {
            sunLerp.SetLightLerp(sunLerp.sunScary, sunLerp.sunNice);
        }
        
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
        
        //extra radios
        for (int i = 0; i < extraRadios.Length; i++)
        {
            extraRadios[i].mute = true;
            extraRadios[i].Stop();
            extraRadios[i].clip = radio.shahSpeech;
            extraRadios[i].volume = 1f;
        }

        //enable front door, disable back building, enable corridor and stairwell
        if(frontDoorCollider)
            frontDoorCollider.enabled = true;
        if(backBuilding)
            backBuilding.SetActive(false);
        if(stairwell)
            stairwell.SetActive(true);
        if(corridor)
            corridor.SetActive(true);

        speechStarted = true;
    }

    //called when video player reaches final frames on speech clip 
    void EndSpeech()
    {
        vidPlayer.Stop();
        screenRender.material = staticEffect;
        sirens.SetActive(true);

        bombing.TransitionTo(3f);
        speechEnded = true;
    }

    //switch mutes on TV & Radio sources
    public void SwitchDeviceAudio()
    {
        tvSource.mute = !tvSource.mute;
        radioSource.mute = !radioSource.mute;
        
        //extra radios
        for (int i = 0; i < extraRadios.Length; i++)
        {
            extraRadios[i].mute = radioSource.mute;
        }
    }
}

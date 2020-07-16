using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using Cameras;

public class TitleToRoom : MonoBehaviour {
    CameraManager camManager;
    Clock clock;
    DebugTime timeline;
    //for all the text refs
    [Header("Title Canvas Refs")]
    public FadeUI omFade;
    public FadeUI uiFade, actFade, dwFade, poemFader;
    public LerpScale omScale, uiScale;
    public MoveUI omMove, uiMove, dwMove;
    public Animator scribeAnimator;
    public FadeSprite scribeFade;
    public FadeSprite blackground;
    public MusicFader callToPrayer;
    public GameObject[] characters;
    public MusicFader warAmbience;
    public Material niceSky;
    public LerpLighting sunLerp;

    //player
    [Header("Player/Room Refs")]
    public FirstPersonController player;
    //other game objs involved in transition
    public GameCamera startCam, roomCam;
    public GameObject  textPanel;
    Television tele;
    Radio radioScript;
    GameObject tv, radio;

    [Header("Transition Vars")]
    public float poemTime = 10f;   //time until game starts 
    float poemTimer = 0, poemTimeNecessary = 15f;
    public bool readingPoem, startedTransition, transitioned;


    void Awake()
    {
        //managers
        camManager = FindObjectOfType<CameraManager>();
        clock = FindObjectOfType<Clock>();
        timeline = FindObjectOfType<DebugTime>();
        //tv ref
        tele = FindObjectOfType<Television>();
        tv = tele.transform.parent.gameObject;
        //radio ref
        radioScript = FindObjectOfType<Radio>();
        radio = radioScript.gameObject;
    }

    void Start()
    {
        player.canMove = false;
    }

    void Update ()
    {
        //get input device 
        var inputDevice = InputManager.ActiveDevice;

        if (Input.GetMouseButtonDown(0) || inputDevice.Action1.WasPressed)
        {
            //click to start poem
            if (!readingPoem)
            {
                ReadPoem();
            }
            else
            {
                //click for transition
                if(poemTimer > poemTimeNecessary && !startedTransition)
                {
                    Transition();
                }
            }
        }

        //poem timer
        if (readingPoem && !transitioned)
        {
            poemTimer += Time.deltaTime;
        }
	}

    void ReadPoem()
    {
        //fade out Act, fade in poem, scribe talks
        actFade.FadeOut();
        poemFader.FadeIn();
        scribeAnimator.SetTrigger("talk");
        //scale titles 
        omScale.lerping = true;
        uiScale.lerping = true;
        //move titles
        omMove.moving = true;
        uiMove.moving = true;
        dwMove.moving = true;

        readingPoem = true;

        StartCoroutine(WaitToTransition(poemTime));
    }

    IEnumerator WaitToTransition(float time)
    {
        yield return new WaitForSeconds(time);

        if (!startedTransition)
        {
            Transition();
        }
    }

    //transition from title to child wake up
    void Transition()
    {
        startedTransition = true;

        //fade out all UI
        omFade.FadeOut();
        uiFade.FadeOut();
        dwFade.FadeOut();
        poemFader.FadeOut();
        blackground.FadeOut();
        scribeAnimator.transform.SetParent(null);
        scribeFade.FadeOut();

        //activate room stuff 
        camManager.Set(roomCam);
        callToPrayer.SetSound(callToPrayer.musicTrack);
        warAmbience.FadeOut(0f, warAmbience.fadeSpeed);
        clock.gameObject.SetActive(true);
        //timeline.StartTimeline();

        //set sun
        sunLerp.SetLightLerp(sunLerp.sunNice, sunLerp.sunNice);

        //activate all the characters in the family 
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(true);
        }
        
        //wait to finalize transition 
        StartCoroutine(WaitForTransition(2f));
    }

    IEnumerator WaitForTransition(float time)
    {
        yield return new WaitForSeconds(time);
        player.canMove = true;
        transitioned = true;

        //enable TV and RADIO
        tv.SetActive(true);
        tele.SetVideoPlayer(tele.tvChannels[0]);
        radio.SetActive(true);

        //change skybox
        RenderSettings.skybox = niceSky;
    }
}

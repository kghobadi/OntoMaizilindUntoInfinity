using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using Cameras;

public class TitleToRoom : MonoBehaviour {
    CameraManager camManager;
    Clock clock;
    //for all the text refs
    [Header("Title Canvas Refs")]
    public FadeUI omFade;
    public FadeUI uiFade, actFade, dwFade, poemFader;
    public FadeUI [] howToStart;
    public LerpScale omScale, uiScale;
    public MoveUI omMove, uiMove, dwMove;
    public Animator scribeAnimator;
    public FadeSprite blackground;
    public MusicFader callToPrayer;
    public GameObject[] characters;
    public MusicFader warAmbience;
    public Material niceSky;
    public LerpLighting sunLerp;
    public MenuSelections quitMenu;
    public AudioSource trainSfx;
    public Vector3 sunStartRotation = new Vector3(43.197f, 72.951f, 4.012f);
    
    //player
    [Header("Player/Room Refs")]
    public FirstPersonController player;
    //other game objs involved in transition
    public GameCamera roomCam;
    Television tele;
    Radio radioScript;
    GameObject tv, radio;

    [Header("Transition Vars")]
    public float poemTime = 10f;   //time until game starts 
    float poemTimer = 0, poemTimeNecessary = 15f;
    public bool readingPoem, startedTransition, transitioned;
    public bool canClick;
    float clickTimer, clickReset = 0.5f;

    void Awake()
    {
        //managers
        camManager = FindObjectOfType<CameraManager>();
        clock = FindObjectOfType<Clock>();
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

        //continually make sure click will not go thru while quit menu open
        if (quitMenu.gameObject.activeSelf)
        {
            ClickReset();
        }
        
        //click inputs for advancing text --> game 
        if(canClick)
        {
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
                    if (poemTimer > poemTimeNecessary && !startedTransition)
                    {
                        Transition();
                    }
                }
            }

        }

        //quit menu 
        if (!transitioned)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //open quit menu
                if (quitMenu.gameObject.activeSelf == false)
                {
                    quitMenu.gameObject.SetActive(true);
                    quitMenu.ActivateMenu(true);
                }
                //close quit menu 
                else
                {
                    quitMenu.DeactivateMenu(true);
                }

                ClickReset();
            }
        }

        //click reset
        if(canClick == false)
        {
            clickTimer += Time.deltaTime;

            if (clickTimer > clickReset)
                canClick = true;
        }

        //poem timer
        if (readingPoem && !transitioned)
        {
            poemTimer += Time.deltaTime;
        }
	}

    public void ClickReset()
    {
        //start click reset 
        canClick = false;
        clickTimer = 0;
    }

    void ReadPoem()
    {
        //fade out Act, fade in poem, scribe talks
        actFade.FadeOut();
        for (int i = 0; i < howToStart.Length; i++)
        {
            howToStart[i].FadeOut();
        }
        poemFader.FadeIn();
        scribeAnimator.SetTrigger("talk");
        //scale titles 
        omScale.lerping = true;
        uiScale.lerping = true;
        //move titles
        omMove.moving = true;
        uiMove.moving = true;
        dwMove.moving = true;
        //train sfx
        if(trainSfx)
            trainSfx.Play();

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

        //activate room stuff 
        camManager.Set(roomCam);
        callToPrayer.SetSound(callToPrayer.musicTrack);
        callToPrayer.FadeIn(callToPrayer.fadeInAmount, callToPrayer.fadeSpeed);
        warAmbience.FadeOut(0f, warAmbience.fadeSpeed);
        if (clock)
        {
            clock.gameObject.SetActive(true);
        }

        //set cursor again
        Cursor.lockState = CursorLockMode.Locked;

        Cursor.visible = false;

        //set sun
        sunLerp.SetLightLerp(sunLerp.sunNice, sunLerp.sunNice);
        //set sun rotation
        sunLerp.transform.localEulerAngles = sunStartRotation;

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

        //enable RADIO
        tv.SetActive(true);
        //tele.SetStartingInterview(); TV now enabled by end of bedroom fade out 
        radio.SetActive(true);

        //change skybox
        RenderSettings.skybox = niceSky;
    }
}

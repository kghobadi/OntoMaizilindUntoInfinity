using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using Cameras;

public class TitleToRoom : MonoBehaviour 
{
    CameraManager camManager;
    Clock clock;
    //for all the text refs
    [Header("Intro Transition")]
    public MusicFader callToPrayer;
    public Material niceSky;
    public LerpLighting sunLerp;
    public Vector3 sunStartRotation = new Vector3(43.197f, 72.951f, 4.012f);
    public FadeUI introFade;
    
    //player
    [Header("Player/Room Refs")]
    public FirstPersonController player;
    //other game objs involved in transition
    public GameCamera roomCam;
    Television tele;
    Radio radioScript;
    GameObject tv, radio;

    [Header("Transition Vars")]
    public bool transitioned;

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
        Transition();
    }

    //transition from title to child wake up
    void Transition()
    {
        //disable player until camera is ready 
        player.canMove = false;
        //activate room stuff 
        camManager.Set(roomCam);
        if (clock)
        {
            clock.gameObject.SetActive(true);
        }

        //set cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        //set call to prayer 
        callToPrayer.SetSound(callToPrayer.musicTrack);
        callToPrayer.FadeIn(callToPrayer.fadeInAmount, callToPrayer.fadeSpeed);

        //set sun
        //sunLerp.SetLightLerp(sunLerp.sunNice, sunLerp.sunNice);
        //set sun rotation
        //sunLerp.transform.localEulerAngles = sunStartRotation;

        //wait to finalize transition 
        StartCoroutine(WaitForTransition(2f));
    }

    IEnumerator WaitForTransition(float time)
    {
        introFade.FadeOut();
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

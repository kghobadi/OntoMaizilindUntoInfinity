using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleToRoom : MonoBehaviour {
    //for all the text refs
    public FadeUItmp omFade, uiFade, actFade, dwFade, poemFader;
    public LerpScale omScale, uiScale;
    public MoveUI omMove, uiMove, dwMove;
    public Animator scribeAnimator;
    public FadeSprite scribeFade;
    //time until game starts 
    public float poemTime = 10f;
    float poemTimer= 0, poemTimeNecessary = 15f;

    //player
    public FirstPersonController player;
    //other game objs involved in transition
    public GameObject startCam, roomCam;
    public GameObject tv, radio, textPanel;
    public FadeSprite blackground;
    public bool readingPoem, startedTransition, transitioned;

    void Start()
    {
        player.canMove = false;
    }

    void Update ()
    {
        if (Input.GetMouseButtonDown(0))
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
        startCam.SetActive(false);
        roomCam.SetActive(true);
        tv.SetActive(true);
        radio.SetActive(true);
        textPanel.SetActive(true);
        
        StartCoroutine(WaitForTransition(2f));
    }

    IEnumerator WaitForTransition(float time)
    {
        yield return new WaitForSeconds(time);
        player.canMove = true;
        transitioned = true;
    }
}

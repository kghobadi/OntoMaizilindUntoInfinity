using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using Cameras;

public class IntroMgr : MonoBehaviour 
{
    //for all the text refs
    [Header("Title Canvas Refs")]
    public FadeUI omFade;
    public FadeUI uiFade, actFade, dwFade, poemFader;
    public FadeUI [] howToStart;
    public GameObject[] raminImages;
    public LerpScale omScale, uiScale;
    public MoveUI omMove, uiMove, dwMove;
    public Animator scribeAnimator;
    public FadeUI blackground;
    public AudioSource trainSfx;
    public MenuSelections quitMenu;
    public MusicFader warAmbience;

    [Header("Transition Vars")]
    public float poemTime = 10f;   //time until game starts 
    float poemTimer = 0, poemTimeNecessary = 15f;
    public bool readingPoem, startedTransition, transitioned;
    public bool canClick;
    float clickTimer, clickReset = 0.5f;
    
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

            //Quick load to PILOT scene for Bug REMOVE THIS EVENTUALLY
            if (Input.GetKeyDown(KeyCode.P))
            {
                LoadingScreenManager.LoadScene(3);
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
        //disable ramin images
        for (int i = 0; i < raminImages.Length; i++)
        {
            raminImages[i].SetActive(false);
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

        //wait until train sound finishes 
        StartCoroutine(WaitToTransition(trainSfx.clip.length));
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

        warAmbience.FadeOut(0f, warAmbience.fadeSpeed);
        //fade out all UI
        omFade.FadeOut();
        uiFade.FadeOut();
        dwFade.FadeOut();
        poemFader.FadeOut();
        blackground.FadeOut();
        scribeAnimator.transform.SetParent(null);

        //set cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //wait to finalize transition 
        LoadSceneAsync.Instance.Load();
        LoadSceneAsync.Instance.TransitionImmediate();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

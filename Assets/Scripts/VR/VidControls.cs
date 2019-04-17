using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using VRStandardAssets.Utils;

public class VidControls : MonoBehaviour {
    //actual vid player
    public VideoPlayer vidPlayer;
    public GameObject vidObject;

    //clip lists
    public List<VideoClip> weddingClips = new List<VideoClip>();

    //clip counters
    public int clipCounter;
    public GameObject[] clipSelectors;

    //ui menu vars
    public bool UIactive;
    public GameObject uiMenu;
    public Transform cameraToAppearInFrontOf;
    public float distanceFromCamera;
    public MeshRenderer[] controllerRenders;

    //menu fade timers -- if no input fades menu
    public float menuActiveTimer;
    public float menuWaitTotal = 3;

    //play/pause buttons vars
    public Image pausePlayImage;
    public Sprite pauseIcon, playIcon;

    //for fast forwarding and reversing -- 0 is normal speed
    public int playBackSpeed = 0;
    public Image fastFwdImage, reverseImage;
    public Sprite[] fastForwardSprites, reverseSprites;
    public int reverseLevel, reverseSpeed;
    public bool rewind;

    //for the vid time slider
    public Slider timeSlider;

    //audio stuff
    public AudioSource audSource;
    public AudioClip play, pause, fastFwd, reverseSnd, nxtClipSnd, lastClipSnd, selectVideoSnd;
    
    // music?
    //need a better method of stitching the footage

    void Start () {
        //set all my start vars
        menuActiveTimer = menuWaitTotal;
        UIactive = true;
        playBackSpeed = 0;
        rewind = false;
        reverseLevel = 0;
        reverseSpeed = 0;
        fastFwdImage.sprite = fastForwardSprites[0];
        reverseImage.sprite = reverseSprites[0];
        UpdateUI();

        audSource = GetComponent<AudioSource>();

        //set current clip object
        clipSelectors[clipCounter].transform.localScale *= 1.25f;
        Color colorComp = clipSelectors[clipCounter].GetComponent<Image>().color;
        colorComp.a = 1;
        clipSelectors[clipCounter].GetComponent<Image>().color = colorComp;

        //set menu dist
        distanceFromCamera = Vector3.Distance(uiMenu.transform.position, cameraToAppearInFrontOf.position);
    }
	
	void Update () {
        OVRInput.Update();
        //sets slider and play icon correctly
        if (vidPlayer.isPlaying)
        {
            //always set timer to current vid time
            timeSlider.value = (float)vidPlayer.time;
            pausePlayImage.sprite = pauseIcon;
        }
        else
        {
            pausePlayImage.sprite = playIcon;
        }
        
        //pops up UI menu
        if (OVRInput.Get(OVRInput.Button.Two) || Input.GetKeyDown(KeyCode.Space))
        {
            if (UIactive)
            {
                //fades out all the UI
                //for(int i = 0; i < fadeScripts.Length; i ++)
                //{
                //    fadeScripts[i].fadingOut = true;
                //    fadeScripts[i].fadingIn = false;
                //}
                //controller meshrenderers
                for(int i =0; i< controllerRenders.Length; i++)
                {
                    controllerRenders[i].enabled = false;
                }
            }

            //open UI menu
            else
            {
                //set menu transform
                uiMenu.transform.position = cameraToAppearInFrontOf.transform.position + (cameraToAppearInFrontOf.transform.forward * distanceFromCamera);
                uiMenu.transform.LookAt(cameraToAppearInFrontOf.transform.position);
                uiMenu.transform.localEulerAngles += new Vector3(0, 180, 0);

                //fades out all the UI
                //for (int i = 0; i < fadeScripts.Length; i++)
                //{
                //    fadeScripts[i].gameObject.SetActive(true);
                //    fadeScripts[i].fadingIn = true;
                //    fadeScripts[i].fadingOut = false;
                //}
                //controller meshrenderers
                for (int i = 0; i < controllerRenders.Length; i++)
                {
                    controllerRenders[i].enabled = true;
                }
            }

            UIactive = !UIactive;
            
            UpdateUI();
        }

        //run menu timer while ui active
        //also prob wanna know that mouse / pointer is not touching any of the UI
        if (UIactive)
        {
            menuActiveTimer -= Time.deltaTime;

            if(menuActiveTimer < 0)
            {
                //fades out all the UI
                //for (int i = 0; i < fadeScripts.Length; i++)
                //{
                //    fadeScripts[i].fadingOut = true;
                //    fadeScripts[i].fadingIn = false;
                //}

                //controller meshrenderers
                for (int i = 0; i < controllerRenders.Length; i++)
                {
                    controllerRenders[i].enabled = false;
                }

                UIactive = false;

                menuActiveTimer = menuWaitTotal;

                Debug.Log("menu fade out");
            }
        }

        //when rewinding, this called every frame for constant backtrack
        if (rewind)
        {
            Reverse(Time.deltaTime);
        }
    }

    //called from UI || -- |> 
    public void PauseNPlay()
    {
        bool hasChanged = false;

        if (vidPlayer.isPlaying && !hasChanged)
        {
            vidPlayer.Pause();
            PlaySound(pause);
            hasChanged = true;
        }
        else if(!vidPlayer.isPlaying && !hasChanged)
        {
            vidPlayer.Play();
            PlaySound(play);
            hasChanged = true;
        }
        
        playBackSpeed = 0;
        rewind = false;
        reverseLevel = 0;
        SwitchPlaybackSpeed();
        SwitchReverseSpeed();
        UpdateUI();
    }

    //called from UI |>|>
    public void FastForward()
    {
        //turn off rewind
        rewind = false;
        reverseLevel = 0;
        SwitchReverseSpeed();

        if (!vidPlayer.isPlaying)
        {
            PauseNPlay();
        }

        //increase speed
        if(playBackSpeed < 3)
        {
            playBackSpeed++;
        }
        //set back to normal
        else
        {
            playBackSpeed = 0;
        }

        SwitchPlaybackSpeed();
        UpdateUI();
        PlaySound(fastFwd);
    }

    void SwitchPlaybackSpeed()
    {
        fastFwdImage.sprite = fastForwardSprites[playBackSpeed];
        //fast forward levels
        switch (playBackSpeed)
        {
            case 0:
                vidPlayer.playbackSpeed = 1;
                break;
            case 1:
                vidPlayer.playbackSpeed = 2;
                break;
            case 2:
                vidPlayer.playbackSpeed = 8;
                break;
            case 3:
                vidPlayer.playbackSpeed = 16;
                break;
        }
    }

    //called from UI <|<|
    public void SetRewind()
    {
        if (!vidPlayer.isPlaying)
        {
            vidPlayer.Play();
        }

        //turn off fast fwd
        playBackSpeed = 0;
        SwitchPlaybackSpeed();

        //decrease speed
        if (reverseLevel < 3)
        {
            reverseLevel++;
            rewind = true;
        }
        //set back to normal
        else
        {
            reverseLevel = 0;
            rewind = false;
        }

        SwitchReverseSpeed();
        UpdateUI();
        PlaySound(reverseSnd);
    }

    //called in update when rewind true
    void Reverse(float deltaTime)
    {
        if (vidPlayer.isPlaying)
        {
            vidPlayer.time = vidPlayer.time - deltaTime * reverseSpeed;
        }
    }

    void SwitchReverseSpeed()
    {
        reverseImage.sprite = reverseSprites[reverseLevel];
        //reverse levels
        switch (reverseLevel)
        {
            case 0:
                reverseSpeed = 0;
                break;
            case 1:
                reverseSpeed = 8;
                break;
            case 2:
                reverseSpeed = 32;
                break;
            case 3:
                reverseSpeed = 96;
                break;
        }
    }

    //called by the video time Slider to set the vid player time
    public void TimeSlider()
    {
        vidPlayer.time = (double)timeSlider.value;
    }

    //call this whenever input is taken to make sure UI is responding to interaction accordingly
    public void UpdateUI()
    {
        //updates play button
        if (vidPlayer.isPlaying)
        {
            //switch UI object's sprite to play Icon
            pausePlayImage.sprite = pauseIcon;
        }
        else
        {
            //switch UI object's sprite to pause Icon
            pausePlayImage.sprite = playIcon;
        }

        //set time slider min and max
        timeSlider.minValue = 0;
        timeSlider.maxValue = (float)vidPlayer.clip.length;

        menuActiveTimer = menuWaitTotal;
    }

    //will be called from UI |--> 
    public void ChangeClipUp()
    {
        vidPlayer.Stop();

        //clip object size and fade
        clipSelectors[clipCounter].transform.localScale = new Vector3(1, 1, 1);

        Color colorCompOld = clipSelectors[clipCounter].GetComponent<Image>().color;
        colorCompOld.a = 0.2f;
        clipSelectors[clipCounter].GetComponent<Image>().color = colorCompOld;

        //count up through clips
        if (clipCounter < weddingClips.Count - 1)
        {
            clipCounter++;
        }
        else
        {
            clipCounter = 0;
        }

        //clip object size and fade
        clipSelectors[clipCounter].transform.localScale *= 1.25f;

        Color colorComp = clipSelectors[clipCounter].GetComponent<Image>().color;
        colorComp.a = 1;
        clipSelectors[clipCounter].GetComponent<Image>().color = colorComp;

        vidPlayer.clip = weddingClips[clipCounter];

        vidPlayer.Play();

        UpdateUI();
        
        PlaySound(nxtClipSnd);
    }

    //will be called from UI <--| 
    public void ChangeClipDown()
    {
        vidPlayer.Stop();

        //clip object size 
        clipSelectors[clipCounter].transform.localScale = new Vector3(1,1,1);
        //and fade
        Color colorCompOld = clipSelectors[clipCounter].GetComponent<Image>().color;
        colorCompOld.a = 0.2f;
        clipSelectors[clipCounter].GetComponent<Image>().color = colorCompOld;

        //count down thru clips
        if (clipCounter > 0)
        {
            clipCounter--;
        }
        else
        {
            clipCounter = weddingClips.Count - 1;
        }

        //clip object size 
        clipSelectors[clipCounter].transform.localScale *= 1.25f;
        //and fade
        Color colorComp = clipSelectors[clipCounter].GetComponent<Image>().color;
        colorComp.a = 1;
        clipSelectors[clipCounter].GetComponent<Image>().color = colorComp;

        vidPlayer.clip = weddingClips[clipCounter];

        vidPlayer.Play();

        UpdateUI();

        PlaySound(lastClipSnd);
    }

    //called by video clip buttons
    public void SelectClip(int clipNum)
    {
        //if the selected clip is not the current clip
        int lastClipCounter = clipCounter;

        if(lastClipCounter != clipNum)
        {
            clipSelectors[clipCounter].transform.localScale = new Vector3(1, 1, 1);

            Color colorCompOld = clipSelectors[clipCounter].GetComponent<Image>().color;
            colorCompOld.a = 0.2f;
            clipSelectors[clipCounter].GetComponent<Image>().color = colorCompOld;
        }
           

        vidPlayer.Stop();

        clipCounter = clipNum;

        //if the selected clip is not the current clip
        if (lastClipCounter != clipNum)
        {
            clipSelectors[clipCounter].transform.localScale *= 1.25f;
            Color colorComp = clipSelectors[clipCounter].GetComponent<Image>().color;
            colorComp.a = 1;
            clipSelectors[clipCounter].GetComponent<Image>().color = colorComp;
        }

        vidPlayer.clip = weddingClips[clipCounter];

        vidPlayer.Play();

        UpdateUI();

        PlaySound(selectVideoSnd);
    }

    public void PlaySound(AudioClip clip)
    {
        audSource.PlayOneShot(clip);
    }
}

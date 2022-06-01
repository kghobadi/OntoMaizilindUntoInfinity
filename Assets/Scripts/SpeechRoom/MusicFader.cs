using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicFader : MonoBehaviour {
    AudioSource musicSource;
    public bool fadingVolOut, fadingVolIn;
    public AudioClip musicTrack;

    public float fadeSpeed = 1f;
    float currentFadeSpeed;
    public float fadeInAmount = 0.5f;
    public float fadeOutAmount = 0f;

    public bool switchingSound;
    
    public AudioClip[] trackList;
    public int trackCounter = 0; 
    
    void Awake () 
    {
        musicSource = GetComponent<AudioSource>();
    }
	
	void Update () {
        if (fadingVolOut)
        {
            musicSource.volume -= Time.deltaTime * currentFadeSpeed;

            if (musicSource.volume <= 0)
            {
                fadingVolOut = false;
                //set fade in to new sound
                if (switchingSound)
                {
                    SetSound(musicTrack);
                    FadeIn(fadeInAmount, fadeSpeed);
                    switchingSound = false;
                }
            }
        }

        if (fadingVolIn)
        {
            musicSource.volume += Time.deltaTime * fadeSpeed;

            if (musicSource.volume >= fadeInAmount)
            {
                fadingVolIn = false;
            }
        }
    }

    public void SetSound(AudioClip track)
    {
        musicSource.Stop();
        musicSource.clip = track;
        musicSource.Play();
    }

    //takes any clip we want to fade to
    public void FadeTo(AudioClip nextTrack)
    {
        musicTrack = nextTrack;
        switchingSound = true;
        FadeOut(fadeInAmount, fadeSpeed);
    }

    //starts fade in to specified amount  
    public void FadeIn(float fadeIn, float speed)
    {
        fadeInAmount = fadeIn;
        currentFadeSpeed = speed;
        fadingVolIn = true;
    }

    //fades out to specified amount  
    public void FadeOut(float fadeOut, float speed)
    {
        fadeOutAmount = fadeOut;
        currentFadeSpeed = speed;
        fadingVolOut = true;
    }

    //fade in
    public void FadeInBasic()
    {
        fadingVolIn = true;
    }

    //fades out  
    public void FadeOutBasic()
    {
        fadingVolOut = true;
    }
    
    /// <summary>
    /// Fades to next track in list if we are not on the last one. 
    /// </summary>
    public void GoToNextTrack()
    {
        if (trackCounter < trackList.Length)
        {
            trackCounter++;
        }    
        
        FadeTo(trackList[trackCounter]);
    }

    /// <summary>
    /// Fades to specific track index. 
    /// </summary>
    /// <param name="index"></param>
    public void GoToTrack(int index)
    {
        //must be bigger than current track counter. 
        if (index < trackList.Length && index > trackCounter)
        {
            trackCounter = index;
            FadeTo(trackList[index]);
        }
    }

    /// <summary>
    /// Waits to go to the track at specified index. 
    /// </summary>
    /// <param name="index"></param>
    public void SetWaitGoToTrack(int index)
    {
        StopAllCoroutines();

        StartCoroutine(WaitForGoToTrack(index));
    }

    /// <summary>
    /// Waits until current clip is over to go to next track. 
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    private IEnumerator WaitForGoToTrack(int index)
    {
        yield return new WaitUntil(() => musicSource.time >= musicSource.clip.length - 0.1f);
        
        GoToTrack(index);
    }
}

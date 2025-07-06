using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows editor based calls to fade this game object's canvas group. 
/// </summary>
public class CanvasFader : MonoBehaviour
{
    private CanvasGroup myCanvasGroup;
    [SerializeField] private bool shownAtStart;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float fadeInAmt = 1f;
    [SerializeField] private float fadeOutAmt = 0f;
    [SerializeField] private float delayIn =0f;
    [SerializeField] private float delayOut =0f;
    public bool IsShowing => myCanvasGroup.alpha >= fadeInAmt;

    private void Awake()
    {
        myCanvasGroup = GetComponent<CanvasGroup>();
        if (shownAtStart)
        {
            myCanvasGroup.alpha = fadeInAmt;
        }
        else
        {
            myCanvasGroup.alpha = fadeOutAmt;
        }
    }

    public void SetFadeDuration(float amt)
    {
        fadeDuration = amt;
    }
    
    public void SetFadeIn(float amt)
    {
        fadeInAmt = amt;
    }
    
    public void SetFadeOut(float amt)
    {
        fadeOutAmt = amt;
    }
    
    public void FadeIn()
    {
        LeanTween.cancel(gameObject);
        if (delayIn > 0)
        {
            LeanTween.delayedCall(delayIn, () => LeanTween.alphaCanvas(myCanvasGroup, fadeInAmt, fadeDuration));
        }
        else
        {
            LeanTween.alphaCanvas(myCanvasGroup, fadeInAmt, fadeDuration);
        }
    }

    public void FadeOut()
    {
        LeanTween.cancel(gameObject);
        if (delayOut > 0)
        {
            LeanTween.delayedCall(delayOut, () =>   LeanTween.alphaCanvas(myCanvasGroup, fadeOutAmt, fadeDuration));
        }
        else
        {
            LeanTween.alphaCanvas(myCanvasGroup, fadeOutAmt, fadeDuration);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(menuName = "Data/Parameters/SpeechBubbleParameters")]
public class SpeechBubbleParameters : DataObject
{
    [Header("Appearance")]
    public int fontSize;
    public float maximumWidth; 

    [Header("Tail")]
    public bool hasTail; //👀
    public float tailWidth; //👀💦

    [Header("Animation")]
    public float tailOpenTime;
    public float bubbleOpenTime;
    [Tooltip("Ease used for bubble when it first opens")]
    public Ease openEase;

    [Space(10)]
    public float tailCloseTime;
    public float bubbleCloseTime; 
    [Tooltip("Ease used for bubble when it closes")]
    public Ease closeEase;

    [Space(10)]
    [Tooltip("Base time used for bubble tweening between sizes")]
    public float baseTweenTime; // its tween time
    [Tooltip("Ease used for bubble tweening between sizes")]
    public Ease baseTweenEase;
    [Range(0,60)]
    public int tweenFPS;

    [Space(10)]
    [Tooltip("The speed at which characters appear in the bubble")]
    public int charactersPerSecond;
    // [Tooltip("Makes tweens faster than base times to account for the tween distance.\n(0 for no effect on speed, 1 for 'full'\n[e.x. 2x tween distance means 2x tween speed])")]
    // public float tweenSizeCompensationEffect;

    // [Header("Shader")]

}
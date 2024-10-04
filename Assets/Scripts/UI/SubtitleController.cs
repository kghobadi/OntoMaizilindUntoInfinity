using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for Subtitles spoken in the Apartment scene. 
/// </summary>
public class SubtitleController : MonoBehaviour
{
    //Provided connection when spawned in Subtitle Mgr. 
    [SerializeField] private MonologueManager speaker;
    
    //Private local references
    [SerializeField] private FaceAnimationUI faceAnimation;
    [SerializeField] private FadeUiRevamped fader;
    
    [Header("Main UI Components")] 
    [SerializeField]
    private Image textBox;
    [SerializeField]
    private TMP_Text characterTitleText;
    [SerializeField]
    private TMP_Text subtitleText;
    [SerializeField]
    private Image arrowImg;
    
    
    //Public properties
    public MonologueManager MonoMgr
    {
        get => speaker;
        set => speaker = value;
    }
    public FaceAnimationUI FaceAnimationUI => faceAnimation;
    public FadeUiRevamped FadeControls => fader;

    public Image TextBox => textBox;
    public TMP_Text CharacterTitleText => characterTitleText;
    public TMP_Text SubtitleText => subtitleText;
    public Image ArrowImg => arrowImg;
    
    /// <summary>
    /// Allows a monologue manager to provide its desired animator to a subtitle. 
    /// </summary>
    /// <param name="newAnimator"></param>
    public void OverloadAnimator(Animator newAnimator)
    {
        faceAnimation.Animator = newAnimator;
    }

    public void SetCharacterTitle(string title)
    {
        characterTitleText.text = title;
    }

    public void SetText(string message)
    {
        subtitleText.text = message;
    }
}

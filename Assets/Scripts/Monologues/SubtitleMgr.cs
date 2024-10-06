using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simple subtitle manager for a scroll view of Subtitles that anyone with a Monologue setup can plug into. 
/// </summary>
public class SubtitleMgr : NonInstantiatingSingleton<SubtitleMgr>
{
    protected override SubtitleMgr GetInstance () { return this; }
    [Tooltip("The basic subtitle prefab")]
    [SerializeField] private GameObject subtitlePrefab;
    [Tooltip("The content parent for the subtitle scroll view.")]
    [SerializeField] private RectTransform scrollViewContent;
    [Tooltip("The content parent for the subtitle scroll view.")]
    [SerializeField] private RectTransform leftSideScrollViewContent;
    [Tooltip("The currently active generated subtitles in the scene.")]
    public List<SubtitleController> generatedSubtitles = new List<SubtitleController>();
    
    //TODO may want the option to do left or right view 
    //Would need to know from position while on screen - maybe commune with FaceVisibility to learn this info? 

    /// <summary>
    /// Pass a monologue manager to the system, get a Subtitle Controller in return. 
    /// </summary>
    /// <param name="monoMgr">The Monologue Manager trying to speak with a subtitle.</param>
    /// <param name="side"></param>
    /// <param name="customPrefab">You can pass a unique subtitle prefab to this for generation.</param>
    /// <returns></returns>
    public SubtitleController GenerateSubtitle(MonologueManager monoMgr,ScreenSide side,  GameObject customPrefab = null)
    {
        SubtitleController controller = null;
        GameObject prefab = subtitlePrefab;
        if (customPrefab != null)
            prefab = customPrefab;
        //Get proper scroll view depending on screen side 
        RectTransform scrollContent = scrollViewContent; 
        if (side == ScreenSide.LeftSide)
        {
            scrollContent = leftSideScrollViewContent;
        }
        GameObject subtitle = Instantiate(prefab, scrollContent);
        controller = subtitle.GetComponent<SubtitleController>();
        controller.MonoMgr = monoMgr;
        generatedSubtitles.Add(controller);

        return controller;
    }
    
    //TODO do I need a destroy method to remove a subtitle from the list?
}

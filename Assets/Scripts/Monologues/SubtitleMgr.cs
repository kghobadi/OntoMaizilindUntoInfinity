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
    [Tooltip("The currently active generated subtitles in the scene.")]
    public List<SubtitleController> generatedSubtitles = new List<SubtitleController>();

    /// <summary>
    /// Pass a monologue manager to the system, get a Subtitle Controller in return. 
    /// </summary>
    /// <param name="monoMgr">The Monologue Manager trying to speak with a subtitle.</param>
    /// <param name="customPrefab">You can pass a unique subtitle prefab to this for generation.</param>
    /// <returns></returns>
    public SubtitleController GenerateSubtitle(MonologueManager monoMgr, GameObject customPrefab = null)
    {
        SubtitleController controller = null;
        GameObject prefab = subtitlePrefab;
        if (customPrefab != null)
            prefab = customPrefab;
        GameObject subtitle = Instantiate(prefab, scrollViewContent);
        controller = subtitle.GetComponent<SubtitleController>();
        controller.MonoMgr = monoMgr;
        generatedSubtitles.Add(controller);

        return controller;
    }
}

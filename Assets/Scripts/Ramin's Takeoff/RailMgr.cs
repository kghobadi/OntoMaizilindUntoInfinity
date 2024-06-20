using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Allows us to easily manage the behavior of all RailLandscapes in the scene.
/// </summary>
public class RailMgr : MonoBehaviour
{

    [SerializeField]
    private RailLandscape[] railLandscapes;

    private void OnValidate()
    {
        if(railLandscapes == null)
            railLandscapes = FindObjectsOfType<RailLandscape>();
    }

    private void Awake()
    {
        if (railLandscapes == null)
            railLandscapes = FindObjectsOfType<RailLandscape>();
    }

    /// <summary>
    /// Allows us to control phase of the rails.
    /// </summary>
    /// <param name="phase"></param>
    public void SetPhase(int phase)
    {
        foreach (var rail in railLandscapes)
        {
            rail.SetPhase(phase);
        }
    }
}

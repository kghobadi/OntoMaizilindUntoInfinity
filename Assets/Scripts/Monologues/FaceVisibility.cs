using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Checks if the Face Renderer is visible
/// </summary>
public class FaceVisibility : MonoBehaviour
{
    private bool faceIsVisible;
    public bool FaceIsVisible => faceIsVisible;
    private void OnBecameVisible()
    {
        faceIsVisible = true;
    }

    private void OnBecameInvisible()
    {
        faceIsVisible = false;
    }
}

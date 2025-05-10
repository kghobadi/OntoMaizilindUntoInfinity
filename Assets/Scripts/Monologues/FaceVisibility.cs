using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Checks if the Face Renderer is visible
/// </summary>
public class FaceVisibility : MonoBehaviour
{
    private Camera mainCam;
    private bool faceIsVisible;
    public bool FaceIsVisible => faceIsVisible;

    [SerializeField] private Transform facePosition;
    private Vector3 currentScreenPos;
    public Vector3 CurrentScreenPos
    {
        get
        {
            GetScreenPosition();
            return currentScreenPos;
        }
    }
    private void Awake()
    {
        mainCam = Camera.main;
    }

    private void OnBecameVisible()
    {
        faceIsVisible = true;
        
        GetScreenPosition();
    }

    private void OnBecameInvisible()
    {
        faceIsVisible = false;
    }

    /// <summary>
    /// Gets the current screen position of the face. 
    /// </summary>
    void GetScreenPosition()
    {
        Transform face = transform;
        if (facePosition)
        {
            face = facePosition;
        }
        currentScreenPos = mainCam.WorldToScreenPoint(face.position);
        //Debug.LogFormat( "{0} has a screen pos of {1} and {2}", gameObject.name , currentScreenPos.x , currentScreenPos.y);
    }

    /// <summary>
    /// Lets us know what side of the screen we're on. 
    /// </summary>
    /// <returns></returns>
    public ScreenSide GetScreenSide()
    {
        GetScreenPosition();

        ScreenSide side = ScreenSide.LeftSide;
        if (currentScreenPos.x > mainCam.pixelWidth / 2)
        {
            side = ScreenSide.RightSide;
        }

        return side;
    }

    /// <summary>
    /// Gets distance from the screen pos of face to the center. 
    /// </summary>
    /// <returns></returns>
    public float GetDistanceFromCenter()
    {
        GetScreenPosition();
        //Get center screen
        Vector2 center = new Vector2(mainCam.pixelWidth / 2, mainCam.pixelHeight / 2);
        float distFromCenter = Vector2.Distance(currentScreenPos, center);
        
        Debug.LogFormat( "{0} has a dist from center of {1}", gameObject.name , distFromCenter);
        return distFromCenter;
    }
    
    
    /// <summary>
    /// Gets distance from the screen pos of face to a rect transform.
    /// </summary>
    /// <returns></returns>
    public float GetDistanceFromRectTrans(RectTransform rectTransform)
    {
        GetScreenPosition();
        //Get anchored pos screen
        Vector2 screenPoint = rectTransform.anchoredPosition;
        float distFromCenter = Vector2.Distance(currentScreenPos, screenPoint);
        
        //Debug.LogFormat( "{0} has a dist from center of {1}", gameObject.name , distFromCenter);
        return distFromCenter;
    }
    //TODO WHILE Visible - convert position of face from World to Screen Point and check distance from player cursor/subtitle?
}

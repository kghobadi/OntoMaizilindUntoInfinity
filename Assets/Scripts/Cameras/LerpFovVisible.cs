using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

/// <summary>
/// Lerps FOV when this object becomes visible/invisible
/// </summary>
public class LerpFovVisible : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private bool lerping;
    [SerializeField] private float lerpVal;
    [SerializeField] private float endValue;
    [SerializeField] private Vector2 fovRange = new Vector2(40f, 60f);

    //lerp timing
    private float lerpStartTime;
    private float lerpEnd;
    [SerializeField] private float lerpDuration = 0.35f;
    private void FixedUpdate()
    {
        if (lerping)
        {
            //get t val by current time from start / total duration
            float tVal = (Time.fixedTime - lerpStartTime) / lerpDuration;
            //get current lerp value
            lerpVal = Mathf.Lerp(cam.m_Lens.FieldOfView, endValue, tVal);
            //apply it 
            cam.m_Lens.FieldOfView = lerpVal;
            //check for end by time
            if (Time.fixedTime > lerpEnd)
            {
                //force fov 
                cam.m_Lens.FieldOfView = endValue;
            }
        }
    }

    void LerpFov(float fov)
    {
        endValue = fov;
        lerpStartTime = Time.fixedTime;
        lerpEnd = lerpStartTime + lerpDuration;
        lerping = true;
    }

    private void OnBecameVisible()
    {
        LerpFov(fovRange.y);
    }

    private void OnBecameInvisible()
    {
        LerpFov(fovRange.x);
    }
}

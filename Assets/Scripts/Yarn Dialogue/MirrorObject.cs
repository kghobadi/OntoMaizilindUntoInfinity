using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mirrors the offset an object has from a hallucination camera as a child of the Main Cam. 
/// </summary>
public class MirrorObject : MonoBehaviour
{
    [Tooltip("obj looking to be mirrored")]
    public Transform mirrorObject;
    [Tooltip("EX: camera looking at mirror obj")]
    public Transform mirrorCam;

    public bool lockY;
   
    void Start()
    {
        CalculateOffset();
    }

    void Update()
    {
        CalculateOffset();
    }

    void CalculateOffset()
    {
        Vector3 offsetPos = mirrorCam.position - mirrorObject.position;
        if (lockY)
        {
            offsetPos = new Vector3(offsetPos.x, transform.localPosition.y, offsetPos.z);
        }
        transform.localPosition = offsetPos;
    }
}
